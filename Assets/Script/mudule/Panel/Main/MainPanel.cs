using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MainPanel : BasePanel
{
      private Button setBt;
      //玩家id
      private Text idText;
      //头像
      private Image headImage;
      //头像列表框
      private GameObject go;
      //头像父物体
      private Transform content;
      //头像物体
      private GameObject imageObj;
      //房间列表按钮
      private Button roomListButton;
      //商店按钮
      private Button shotBt;
      //金币数量文本
      private Text coinText;
      //好友系统部分
      private Dictionary<string, float> sendTimes = new Dictionary<string, float>();//向对应玩家发送亲求的时间
      private PlayerInfo self;
      private ScrollRect friendScrollView;
      private GameObject friendObj;
      private Transform friendContent;
      private delegate void FriendDataHandle(Transform friendTra, string value);
      private Dictionary<string, FriendDataHandle> DataHandles = new Dictionary<string, FriendDataHandle>();
      private Transform fTalkPanel;
      private InputField finputFile;
      private Text ftextFile;
      private Button fsendBt;
      private Text targetText;
      private string currentTalkTarget;

      private Transform startTipPanel;
      private DataType dataType;
      private Dictionary<DataType, Action<PlayerData>> setValue = new Dictionary<DataType, Action<PlayerData>>();
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.RemoveMsgListener("MsgSavePlayerData", OnMsgSavePlayerData);
            NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.RemoveMsgListener("MsgInviteFriend", OnMsgInviteFriend);

            NetManager.RemoveMsgListener("MsgGetFriendsData", OnMsgGetFriendsData);
            NetManager.RemoveMsgListener("MsgFriendResult", OnMsgFriendResult);
            NetManager.RemoveMsgListener("MsgUpdateFriendData", OnMsgUpdateFriendData);
            NetManager.RemoveMsgListener("MsgFriendTalk", OnMsgFriendTalk);
            NetManager.RemoveMsgListener("MsgGetFriendTalk", OnMsgGetFriendTalk);
      }

      public override void OnInit()
      {
            skinPath = "MainPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            //网络监听
            NetManager.AddMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.AddMsgListener("MsgSavePlayerData", OnMsgSavePlayerData);
            NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.AddMsgListener("MsgInviteFriend", OnMsgInviteFriend);
            
            DataHandles.Add("statu", FriendStatuHandle);
            NetManager.AddMsgListener("MsgGetFriendsData", OnMsgGetFriendsData);
            NetManager.AddMsgListener("MsgFriendResult", OnMsgFriendResult);
            NetManager.AddMsgListener("MsgUpdateFriendData", OnMsgUpdateFriendData);
            NetManager.AddMsgListener("MsgFriendTalk", OnMsgFriendTalk);
            NetManager.AddMsgListener("MsgGetFriendTalk", OnMsgGetFriendTalk);
            //发送获取玩家信息协议
            MsgGetPlayerData msg = new MsgGetPlayerData();
            NetManager.Send(msg);
            //寻找组件
            imageObj = TransformHelper.FindChiled(skin.transform, "BaseHeadImage").gameObject;
            headImage = TransformHelper.FindChiled(skin.transform, "HeadImage").GetComponent<Image>();
            idText = TransformHelper.FindChiled(skin.transform, "IdText").GetComponent<Text>();
            roomListButton = TransformHelper.FindChiled(skin.transform, "RoomListButton").GetComponent<Button>();
            go = TransformHelper.FindChiled(skin.transform, "Scroll View").gameObject;
            content = TransformHelper.FindChiled(go.transform, "Content");
            shotBt = TransformHelper.FindChiled(skin.transform, "ShopBt").GetComponent<Button>();
            startTipPanel = TransformHelper.FindChiled(skin.transform, "StartTipPanel");
            coinText = TransformHelper.FindChiled(skin.transform, "CoinText").GetComponent<Text>();
            #region 好友系统
            friendScrollView = TransformHelper.FindChiled(skin.transform, "FriendScroll View").GetComponent<ScrollRect>();
            friendObj = TransformHelper.FindChiled(friendScrollView.transform, "FriendObj").gameObject;
            friendContent = TransformHelper.FindChiled(friendScrollView.transform, "Content");
            fTalkPanel = TransformHelper.FindChiled(friendScrollView.transform, "TalkPanel");
            ftextFile = TransformHelper.FindChiled(friendScrollView.transform, "TextField").GetComponent<Text>();
            finputFile = TransformHelper.FindChiled(friendScrollView.transform, "InputField").GetComponent<InputField>();
            fsendBt = TransformHelper.FindChiled(friendScrollView.transform, "SendBt").GetComponent<Button>();
            targetText = TransformHelper.FindChiled(friendScrollView.transform, "TargetText").GetComponent<Text>();
            setBt = TransformHelper.FindChiled(skin.transform, "SetButton").GetComponent<Button>();
            //发送查询好友信息协议
            MsgGetFriendsData msgGetFriends = new MsgGetFriendsData();
            NetManager.Send(msgGetFriends);
            #endregion
            //赋值
            roomListButton.onClick.AddListener(() => { PanelManager.Open<RoomListPanel>(); Close(); });
            shotBt.onClick.AddListener(() => {  PanelManager.Open<StorePanel>();Close(); });
            go.gameObject.SetActive(false);
            setValue.Add(DataType.coin, ChangeCoinText);
            setValue.Add(DataType.sprite, ChangeHeadImage);
            setBt.onClick.AddListener(()=> { PanelManager.Open<SystemSetPanel>(false); });
            fsendBt.onClick.AddListener(FriendTalkBtListener);
            //通知好友
            MsgUpdateFriendData msgUpdate = new MsgUpdateFriendData();
            msgUpdate.dataName = "statu";
            msgUpdate.dataValue = "0";
            NetManager.Send(msgUpdate);
          
            //创建头像列表
            Sprite[] sprites = Resources.LoadAll<Sprite>("Ui/HeadSprites");
            for (int i = 0; i < sprites.Length; i++)
            {
                  GameObject ig = Instantiate(imageObj);
                  ig.transform.SetParent(content);
                  ig.transform.localPosition = Vector3.zero;
                  ig.transform.localScale = Vector3.one;
                  Image image = ig.GetComponent<Image>();
                  image.sprite = sprites[i];
                  ig.GetComponent<Button>().onClick.AddListener(delegate () { OnHeadImgeClick(image); });
            }
      }
      private void OnMsgUpdateFriendData(MsgBase msgBase)
      {
            MsgUpdateFriendData msg = (MsgUpdateFriendData)msgBase;
            //获取对应名称的物体
            for (int i = 0; i < friendContent.childCount; i++)
            {
                  Transform friendTra = friendContent.GetChild(i);
                  if (friendTra.name == msg.name)
                  {
                        if (DataHandles.ContainsKey(msg.dataName))
                        {
                              //调用对应处理方法
                              DataHandles[msg.dataName].Invoke(friendTra, msg.dataValue);
                        }
                  }
            }
      }
      private void FriendStatuHandle(Transform tra, string value)
      {
            Button inviteBt = TransformHelper.FindChiled(tra, "InviteBt").GetComponent<Button>();
            Text inviteText = inviteBt.GetComponentInChildren<Text>();
            Image hdImage = TransformHelper.FindChiled(tra, "HdImage").GetComponent<Image>();
            Button talkBt = TransformHelper.FindChiled(tra, "TalklBt").GetComponent<Button>();
            Text talkText = talkBt.GetComponentInChildren<Text>();
            int valueInt = int.Parse(value);
            if (valueInt == 0)//在线状态
            {
                  hdImage.color = Color.white;
                  inviteText.text = "在线";
                  talkText.text = "聊天";
            }
            else if (valueInt == 1)//房间内
            {
                  hdImage.color = Color.white;
                  inviteText.text = "房间内";
                  talkText.text = "聊天";
            }
            else//离线状态
            {
                  hdImage.color = Color.gray;
                  inviteText.text = "玩家已离线";
                  talkText.text = "暂未开启离线留言功能";
            }
      }
      private void OnMsgFriendResult(MsgBase msgBase)
      {
            MsgFriendResult msg = (MsgFriendResult)msgBase;
            GenerateFriendData(msg.friendData);
      }
      //获取好友信息协议监听
      private void OnMsgGetFriendsData(MsgBase msgBase)
      {
            MsgGetFriendsData msg = (MsgGetFriendsData)msgBase;
            if (msg.friends == null)
            { return; }
            for (int i = 0; i < friendContent.childCount; i++)
            {
                  GameObject go = friendContent.GetChild(i).gameObject;
                  Destroy(go);
            }
            for (int i = 0; i < msg.friends.Length; i++)
            {
                  GenerateFriendData(msg.friends[i]);
            }
      }
      private void FriendTalkBtListener()
      {
            if (currentTalkTarget == null)
            { return; }
            if (finputFile.text == string.Empty)
            { return; }
            MsgFriendTalk msg = new MsgFriendTalk();
            msg.targetName = currentTalkTarget;
            msg.str = (GameMain.id + ": " + finputFile.text + "\n").ToString();
            NetManager.Send(msg);
      }
      private void OnMsgGetFriendTalk(MsgBase msgBase)
      {
            MsgGetFriendTalk msg = (MsgGetFriendTalk)msgBase;
            ftextFile.text = msg.fullstr;
      }
      private void OnMsgFriendTalk(MsgBase msgBase)
      {
           
            MsgFriendTalk msg = (MsgFriendTalk)msgBase;
            if (msg.name != GameMain.id)//消息提醒
            {
                  //查找
                  for (int i = 0; i < friendContent.childCount; i++)
                  {
                        Transform temp = friendContent.GetChild(i);
                        if (temp.name == msg.targetName)
                        {
                              Image tipImage = TransformHelper.FindChiled(temp, "TipImage").GetComponent<Image>();
                              tipImage.enabled = true;
                        }
                  }
            }
            if (msg.name == GameMain.id)
            {
                  finputFile.text = string.Empty;
            }
            //如果发信人不是当前好友不显示
            if (msg.targetName != currentTalkTarget)
            {
                  return;
            }
            ftextFile.text = msg.fullstr;
      }
      private void GenerateFriendData(FriendData friendData)
      {
            GameObject friend = Instantiate(friendObj, friendContent);
            friend.name = friendData.name;
            friend.transform.localPosition = Vector3.zero;
            friend.transform.localScale = Vector3.one;
            //获取组件
            Image hdImage = TransformHelper.FindChiled(friend.transform, "HdImage").GetComponent<Image>();
            Text nameText = TransformHelper.FindChiled(friend.transform, "NameText").GetComponent<Text>();
            Button inviteBt = TransformHelper.FindChiled(friend.transform, "InviteBt").GetComponent<Button>();
            Text inviteText = inviteBt.GetComponentInChildren<Text>();
            Button talkBt = TransformHelper.FindChiled(friend.transform, "TalklBt").GetComponent<Button>();
            Text talkText = talkBt.GetComponentInChildren<Text>();
            //赋值
            hdImage.sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + friendData.sprite);
            nameText.text = friendData.name;
            if (friendData.statu == 0)//在线状态
            {
                  hdImage.color = Color.white;
                  inviteText.text = "在线";
                  talkText.text = "聊天";
            }
            else if (friendData.statu == 1)//房间内
            {
                  hdImage.color = Color.white;
                  inviteText.text = "房间内";
                  talkText.text = "聊天";
            }
            else//离线状态
            {
                  hdImage.color = Color.gray;
                  inviteText.text = "玩家已离线";
                  talkText.text = "暂未开启离线留言功能";

            }
            //聊天按钮添加监听
            talkBt.onClick.AddListener(() =>
            {
                  //判断当前聊天对象
                  if (currentTalkTarget != friendData.name)//如果不是
                  {
                        //清除聊天面板内容
                        ftextFile.text = string.Empty;
                        //发送获取记录协议
                        MsgGetFriendTalk msg = new MsgGetFriendTalk();
                        msg.targetName = friendData.name;
                        NetManager.Send(msg);
                  }
                  fTalkPanel.gameObject.SetActive(true);
                  targetText.text = string.Format("和<color=yellow>{0}</color>的聊天", friendData.name);
                  currentTalkTarget = friendData.name;
            });
            friend.SetActive(true);
      }

      private void OnMsgInviteFriend(MsgBase msgBase)
      {
            MsgInviteFriend msg = (MsgInviteFriend)msgBase;
            if (msg.friendData == null)
            { Debug.Log("frienddata null GameMain"); return; }
            PanelManager.Open<InvitePanel>(msg);
      }
      //收到进入房间协议
      private void OnMsgEnterRoom(MsgBase msgBase)
      {
            MsgEnterRoom msgEnter = (MsgEnterRoom)msgBase;
            //成功进入房间
            if (msgEnter.result == 0)
            {
                  PanelManager.Open<RoomPanel>();
                  Close();
                  PanelManager.Close("SystemSetPanel");
            }
            else
            {
                  PanelManager.Open<TipPanel>("进入房间失败");
            }
      }
      private void OnMsgGetPlayerData(MsgBase msgBase)
      {
            MsgGetPlayerData msg = (MsgGetPlayerData)msgBase;
            idText.text = msg.id;
            GameMain.id = msg.id;
            coinText.text = msg.playerData.coin.ToString();
            Sprite sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + msg.playerData.sprite);
            if (sprite != null)
            {
                  headImage.sprite = sprite;
                  GameMain.sprite = msg.playerData.sprite;
            }
            //如果玩家是新注册
            if (msg.playerData.guns == null || msg.playerData.guns.Length == 0)
            {
                  startTipPanel.gameObject.SetActive(true);
                  //发送保存信息
                  MsgSavePlayerData msgSave = new MsgSavePlayerData();
                  msgSave.dataName = DataType.guns.ToString();
                  msgSave.playerData.guns = new[] { "assault_rifle_01" };
                  NetManager.Send(msgSave);
            }
            else
            { startTipPanel.gameObject.SetActive(false); }
      }
      //当点击列表内头像
      private void OnHeadImgeClick(Image image)
      {
            go.gameObject.SetActive(false);
            //发送保存玩家信息协议
            MsgSavePlayerData msg = new MsgSavePlayerData();
            msg.id = GameMain.id;
            string dataName = DataType.sprite.ToString();
            msg.dataName = dataName;
            msg.playerData.sprite = image.sprite.name;
            NetManager.Send(msg);
      }
      //当收到保存玩家信息协议
      private void OnMsgSavePlayerData(MsgBase msgBase)
      {
            MsgSavePlayerData msg = (MsgSavePlayerData)msgBase;
            dataType = (DataType)Enum.Parse(typeof(DataType), msg.dataName);
            if (!setValue.ContainsKey(dataType))
            { return; }
            if (msg.result == 1)
            {
                  setValue[dataType]?.Invoke(msg.playerData);
            }
      }
      private void ChangeHeadImage(PlayerData playerData)
      {
            Sprite sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + playerData.sprite);
            if (sprite == null)
            { return; }
            headImage.sprite = sprite;
            GameMain.sprite = playerData.sprite;
      }
      private void ChangeCoinText(PlayerData playerData)
      {
            coinText.text = playerData.coin.ToString();
      }
}
