using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class RoomPanel : BasePanel
{
      //开始/准备按钮
      private Button startButton;
      //玩家列表容器
      private Transform content;
      //玩家信息预制体
      private GameObject playerObj;
      //玩家模型预制体
      private GameObject prefab;
      //枪列表
      private Transform guncontent;
      //玩家拥有的枪的id
      private string[] guns;
      //聊天部分
      private Transform TalkPanel;
      private InputField inputFile;
      private Text textFile;
      private Button sendBt;
      //好友系统部分
      private Dictionary<string, float> sendTimes = new Dictionary<string, float>();//向对应玩家发送亲求的时间
      private PlayerInfo self;
      private ScrollRect friendScrollView;
      private GameObject friendObj;
      private Transform friendContent;
      private delegate void FriendDataHandle(Transform friendTra,string value);
      private Dictionary<string, FriendDataHandle> DataHandles = new Dictionary<string, FriendDataHandle>();
      private Transform fTalkPanel;
      private InputField finputFile;
      private Text ftextFile;
      private Button fsendBt;
      private Text targetText;
      private string currentTalkTarget;
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
            NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
            NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
            NetManager.RemoveMsgListener("MsgReady", OnMsgReady);
            NetManager.RemoveMsgListener("MsgChoseGun", OnMsgChoseGun);
            NetManager.RemoveMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.RemoveMsgListener("MsgTalk", OnMsgTalk);
            NetManager.RemoveMsgListener("MsgGetFriendsData", OnMsgGetFriendsData);
            NetManager.RemoveMsgListener("MsgFriendResult", OnMsgFriendResult);
            NetManager.RemoveMsgListener("MsgUpdateFriendData", OnMsgUpdateFriendData);
            NetManager.RemoveMsgListener("MsgFriendTalk", OnMsgFriendTalk);
            NetManager.RemoveMsgListener("MsgGetFriendTalk", OnMsgGetFriendTalk);
      }

      public override void OnInit()
      {
            skinPath = "RoomPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            //寻找组件
            startButton = TransformHelper.FindChiled(skin.transform, "StartButton").GetComponent<Button>();
            content = TransformHelper.FindChiled(skin.transform, "Content").transform;
            playerObj = TransformHelper.FindChiled(skin.transform, "Data").gameObject;
            prefab = ResourceManager.Load<GameObject>("UiMold");//加载模型
            guncontent = TransformHelper.FindChiled(skin.transform, "ChoseWeapenPanel/Scroll View/Viewport/Content");
            TalkPanel = TransformHelper.FindChiled(transform, "TalkPanel");
            textFile = TransformHelper.FindChiled(TalkPanel, "TextField").GetComponent<Text>();
            inputFile = TransformHelper.FindChiled(TalkPanel, "InputField").GetComponent<InputField>();
            sendBt = TransformHelper.FindChiled(TalkPanel, "SendBt").GetComponent<Button>();

            #region 好友系统
            friendScrollView = TransformHelper.FindChiled(skin.transform, "FriendScroll View").GetComponent<ScrollRect>();
            friendObj = TransformHelper.FindChiled(friendScrollView.transform, "FriendObj").gameObject;
            friendContent = TransformHelper.FindChiled(friendScrollView.transform, "Content");
            fTalkPanel = TransformHelper.FindChiled(friendScrollView.transform, "TalkPanel");
            ftextFile = TransformHelper.FindChiled(friendScrollView.transform, "TextField").GetComponent<Text>();
            finputFile = TransformHelper.FindChiled(friendScrollView.transform, "InputField").GetComponent<InputField>();
            fsendBt = TransformHelper.FindChiled(friendScrollView.transform, "SendBt").GetComponent<Button>();
            targetText = TransformHelper.FindChiled(friendScrollView.transform, "TargetText").GetComponent<Text>();
            #endregion
            //绑定事件
            sendBt.onClick.AddListener(() =>
            {
                  if (string.IsNullOrEmpty(inputFile.text)) { return; }
                  MsgTalk msg = new MsgTalk();
                  msg.msg = inputFile.text; NetManager.Send(msg);
            }
            );
            fsendBt.onClick.AddListener(FriendTalkBtListener);
            //协议监听
            NetManager.AddMsgListener("MsgTalk", OnMsgTalk);
            NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
            NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
            NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
            NetManager.AddMsgListener("MsgReady", OnMsgReady);
            NetManager.AddMsgListener("MsgChoseGun", OnMsgChoseGun);
            NetManager.AddMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.AddMsgListener("MsgGetFriendsData", OnMsgGetFriendsData);
            NetManager.AddMsgListener("MsgFriendResult", OnMsgFriendResult);
            NetManager.AddMsgListener("MsgUpdateFriendData", OnMsgUpdateFriendData);
            NetManager.AddMsgListener("MsgFriendTalk", OnMsgFriendTalk);
            NetManager.AddMsgListener("MsgGetFriendTalk", OnMsgGetFriendTalk);
            DataHandles.Add("statu", FriendStatuHandle);
            //发送查询房间协议
            MsgGetRoomInfo getRoomInfo = new MsgGetRoomInfo();
            NetManager.Send(getRoomInfo);
            //发送请求玩家信息协议
            MsgGetPlayerData msgGet = new MsgGetPlayerData();
            NetManager.Send(msgGet);
            //发送查询好友信息协议
            MsgGetFriendsData msgGetFriends = new MsgGetFriendsData();
            NetManager.Send(msgGetFriends);
      }
      private void OnMsgUpdateFriendData(MsgBase msgBase)
      {
            MsgUpdateFriendData msg = (MsgUpdateFriendData)msgBase;
            //获取对应名称的物体
            for(int i = 0;i<friendContent.childCount;i++)
            {
                  Transform friendTra = friendContent.GetChild(i);
                  if(friendTra.name == msg.name)
                  {
                        if(DataHandles.ContainsKey(msg.dataName))
                        {
                              //调用对应处理方法
                              DataHandles[msg.dataName].Invoke(friendTra, msg.dataValue);
                        }
                  }
            }
      }
      private void FriendStatuHandle(Transform tra,string value)
      {
            Button inviteBt = TransformHelper.FindChiled(tra, "InviteBt").GetComponent<Button>();
            Text inviteText = inviteBt.GetComponentInChildren<Text>();
            Image hdImage = TransformHelper.FindChiled(tra, "HdImage").GetComponent<Image>();
            Button talkBt = TransformHelper.FindChiled(tra, "TalklBt").GetComponent<Button>();
            Text talkText = talkBt.GetComponentInChildren<Text>();
            int valueInt = int.Parse(value);
            if (valueInt == 0)//在线状态
            {
                  inviteText.name = "邀请";
                  hdImage.color = Color.white;
                  inviteText.text = "邀请";
                  talkText.text = "聊天";
                  inviteBt.enabled = true;
            }
            else if (valueInt == 1)//房间内
            {
                  inviteText.name = "房间内";
                  hdImage.color = Color.white;
                  inviteText.text = "房间内";
                  talkText.text = "聊天";
                  inviteBt.enabled = false;
            }
            else//离线状态
            {
                  inviteText.name = "玩家已离线";
                  hdImage.color = Color.gray;
                  inviteText.text = "玩家已离线";
                  talkText.text = "暂未开启离线留言功能";
                  inviteBt.enabled = false;
            }
      }

      // 添加好友结果协议监听
      private void OnMsgFriendResult(MsgBase msgBase)
      {
            MsgFriendResult msg = (MsgFriendResult)msgBase;
            GenerateFriendData(msg.friendData);
            //消除房间内添加好友按钮
            for(int i = 0;i<content.childCount;i++)
            {
                  Transform player = content.GetChild(i);
                  if (player.name == msg.targetName)
                  {
                        Transform addButton = TransformHelper.FindChiled(player, "AddButton");
                        addButton.gameObject.SetActive(false);
                  }
            }
      }
      //获取好友信息协议监听
      private void OnMsgGetFriendsData(MsgBase msgBase)
      {
            MsgGetFriendsData msg = (MsgGetFriendsData)msgBase;
            if(msg.friends == null)
            { return; }
            for(int i = 0;i<friendContent.childCount;i++)
            {
                  GameObject go = friendContent.GetChild(i).gameObject;
                  Destroy(go);
            }
            for(int i = 0;i<msg.friends.Length;i++)
            {
                  GenerateFriendData(msg.friends[i]);
            }
      }
      //好友消息发送按钮事件
      private void FriendTalkBtListener()
      {
            if(currentTalkTarget==null)
            { return; }
            if(finputFile.text == string.Empty)
            { return; }
            MsgFriendTalk msg = new MsgFriendTalk();
            msg.targetName = currentTalkTarget;
            msg.str =(GameMain.id+": "+ finputFile.text+"\n").ToString();
            NetManager.Send(msg);
      }
      //获取聊天记录协议监听
      private void OnMsgGetFriendTalk(MsgBase msgBase)
      {
            MsgGetFriendTalk msg = (MsgGetFriendTalk)msgBase;
            ftextFile.text = msg.fullstr;
      }
      private void OnMsgFriendTalk(MsgBase msgBase)
      {
            
            MsgFriendTalk msg = (MsgFriendTalk)msgBase;
            if (msg.name!=GameMain.id)//消息提醒
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
            if(msg.targetName!=currentTalkTarget)
            {
                  return;
            }
            ftextFile.text = msg.fullstr;
      }
      IEnumerator TimeCount(Button button)
      {
            button.enabled = false;
            float counttime = 10;
            Text inviteText = button.GetComponentInChildren<Text>();
            while(counttime>=0)
            { 
                  inviteText.text = string.Format("<color=yellow>{0}</color>秒后可邀请", counttime.ToString("0"));
                  counttime -= Time.deltaTime;
                  yield return null;
            }
            inviteText.text = inviteText.name;
            button.enabled = true;
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
            hdImage.sprite = Resources.Load<Sprite>("Ui/HeadSprites/"+friendData.sprite);
            nameText.text = friendData.name;
            inviteBt.onClick.AddListener(() => {
                  MsgInviteFriend msg = new MsgInviteFriend();
                  msg.targetName = friendData.name;
                  NetManager.Send(msg);
                  StartCoroutine(TimeCount(inviteBt));
            });
            if (friendData.statu==0)//在线状态
            {
                  inviteText.name = "邀请";
                  hdImage.color = Color.white;
                  inviteText.text = "邀请";
                  talkText.text = "聊天";
                  inviteBt.enabled = true;
            }
            else if(friendData.statu == 1)//房间内
            {
                  inviteText.name = "房间内";
                  hdImage.color = Color.white;
                  inviteText.text = "房间内";
                  talkText.text = "聊天";
                  inviteBt.enabled = false;
            }
            else//离线状态
            {
                  inviteText.name = "玩家已离线";
                  hdImage.color = Color.gray;
                  inviteText.text = "玩家已离线";
                  talkText.text = "暂未开启离线留言功能";
                  inviteBt.enabled = false;
            }
            //聊天按钮添加监听
            talkBt.onClick.AddListener(() =>
            {
                  //判断当前聊天对象
                  if(currentTalkTarget!=friendData.name)//如果不是
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
      //消息信息监听
      private void OnMsgTalk(MsgBase msgBase)
      {
           
            MsgTalk msg = (MsgTalk)msgBase;
            if (msg.id == GameMain.id)
            {
                  inputFile.text = string.Empty;
            }
            string fullMsg = (msg.id + ": " + msg.msg + "\n").ToString();
            textFile.text += fullMsg;
      }
      //获取玩家信息协议
      private void OnMsgGetPlayerData(MsgBase msgBase)
      {
            MsgGetPlayerData msgGet = (MsgGetPlayerData)msgBase;
             guns = msgGet.playerData.guns;
            //按钮绑定事件
            for (int i = 0; i < guncontent.childCount; i++)
            {
                  //如果玩家没有该枪
                  if(Array.Find(guns,a=>a == guncontent.GetChild(i).name)==null)
                  {
                        guncontent.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                        continue;
                  }
                  Button bt = guncontent.GetChild(i).GetComponent<Button>();
                  //添加事件发送选择枪协议
                  bt.onClick.AddListener(() => {  MsgChoseGun msg = new MsgChoseGun(); msg.gunname = bt.gameObject.name; NetManager.Send(msg); });
            }
      }
      //收到房间内信息协议
      private void OnMsgGetRoomInfo(MsgBase msgBase)
      {
            startButton.onClick.RemoveAllListeners();
            //删除房间内信息
            MsgGetRoomInfo getRoomInfo = (MsgGetRoomInfo)msgBase;
            for (int i = 0; i <content.childCount; i++)
            {
                  content.GetChild(i).name = ("Player" + i).ToString();
                  if (content.GetChild(i).childCount > 0)
                  {
                        Transform trans = content.GetChild(i);
                        GameObject go = trans.GetChild(0).gameObject;
                        GameObject gos = trans.GetComponent<UiMapping>().map3DTarget.transform.GetChild(0).gameObject;
                        Destroy(gos);
                        Destroy(go);
                  }
            }
            //先生成自己
            GeneratePlayerInfo(ArrayHelper.Find(getRoomInfo.players, a => a.name == GameMain.id));
            for (int i = 0; i < getRoomInfo.players.Length; i++)
            {
                  if (getRoomInfo.players[i].name != GameMain.id)
                  {
                        GeneratePlayerInfo(getRoomInfo.players[i]);
                  }
            }
      }
      //创建一个玩家信息单元
      private void GeneratePlayerInfo(PlayerInfo playerInfo)
      {
            //创建物体
            GameObject go = Instantiate(playerObj);
            GameObject gos = Instantiate(prefab);//模型
            Transform parent = content.GetChild(playerInfo.posId);
            Transform modpa = parent.GetComponent<UiMapping>().map3DTarget;
            parent.name = playerInfo.name;
            gos.transform.SetParent(modpa, false);
            gos.transform.localPosition = Vector3.zero;
            go.transform.SetParent(parent);
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            Transform gunPos = TransformHelper.FindChiled(gos.transform, "BigGunPos");
            HandGun(playerInfo.gunName, gunPos);
            //获取组件
            Transform trans = go.transform;
            Text idtext = TransformHelper.FindChiled(trans, "IdText").GetComponent<Text>();
            Image statuImage = TransformHelper.FindChiled(trans, "StatusImage").GetComponent<Image>();
            Button closeButton = TransformHelper.FindChiled(trans, "CloseButton").GetComponent<Button>();
            Button addButton = TransformHelper.FindChiled(trans, "AddButton").GetComponent<Button>();
            //填充信息
            idtext.text = playerInfo.name;
            //如果是房主
            if (playerInfo.isOwner == 1)
            {
                  idtext.color = Color.red;
                  statuImage.enabled = false;
            }
            else
            {
                  statuImage.enabled = playerInfo.show.Equals(1);
            }
            //判断是自己
            if (playerInfo.name == GameMain.id)
            {
                  self = playerInfo;
                  addButton.gameObject.SetActive(false);
                  closeButton.gameObject.SetActive(true);
                  //给开始按钮绑定事件
                  startButton.onClick.AddListener(() => { OnStartClick(playerInfo); });
                  //给关闭按钮绑定事件
                  closeButton.onClick.AddListener(OnCloseClick);
                  Text text = startButton.GetComponentInChildren<Text>();
                  if (playerInfo.isOwner == 1)
                  {
                        text.text = "开始游戏";
                  }
                  else
                  {
                        text.text = "准备";
                  }

            }
            else
            {
                  if (!sendTimes.ContainsKey(playerInfo.name))
                  {
                        sendTimes[playerInfo.name] = 0;
                  }
                  closeButton.gameObject.SetActive(false);
                  FriendDbData otherPlayer = null;
                  if (self.friend.friends != null)
                  {
                        otherPlayer = ArrayHelper.Find(self.friend.friends, a => a.name == playerInfo.name);
                  }
                  if (otherPlayer != null)
                  {
                        addButton.gameObject.SetActive(false);
                  }
                  else
                  {
                        addButton.gameObject.SetActive(true);
                        //给添加好友按钮添加事件
                        addButton.onClick.AddListener(() =>
                        {
                              if (Time.time - sendTimes[idtext.text] >= 5)
                              {
                                    sendTimes[idtext.text] = Time.time;
                                    MsgAddFriend msg = new MsgAddFriend();
                                    msg.targetName = idtext.text;
                                    NetManager.Send(msg);
                              };
                        });
                  }
            }
      }
      //开始/准备按钮监听· 
      private void OnStartClick(PlayerInfo playerInfo)
      {
            //如果是房主发送开始战斗协议
            if(playerInfo.isOwner == 1)
            {
                  MsgStartBattle msgStart = new MsgStartBattle();
                  NetManager.Send(msgStart);
            }
            else
            {
                  MsgReady msgReady = new MsgReady();
                  NetManager.Send(msgReady);
            }
      }
      //离开按钮监听
      private void OnCloseClick()
      {
            MsgLeaveRoom msg = new MsgLeaveRoom();
            NetManager.Send(msg);
      }
      //收到开始协议
      private void OnMsgStartBattle(MsgBase msgBase)
      {
            MsgStartBattle msgStart = (MsgStartBattle)msgBase;
            if(msgStart.result == 0)
            {
                  Debug.Log("开始游戏");
            }
            else
            {
                  PanelManager.Open<TipPanel>("有玩家未准备");
            }
      }
      //收到玩家准备协议
      private void OnMsgReady(MsgBase msgBase)
      {
            MsgReady msgReady = (MsgReady)msgBase;
            if(msgReady.result == 1)
            {
                  return;
            }
            int posid = msgReady.playerInfo.posId;
            Transform trans = content.GetChild(posid);
            Image statusImage = TransformHelper.FindChiled(trans, "StatusImage").GetComponent<Image>();
            if(msgReady.show == 0)
            {
                  statusImage.enabled = false;
                  if(msgReady.playerInfo.name == GameMain.id)//如果是自己
                  {
                        startButton.GetComponentInChildren<Text>().text = "准备";
                  }
            }
            else
            {
                  statusImage.enabled = true;
                  if (msgReady.playerInfo.name == GameMain.id)//如果是自己
                  {
                        startButton.GetComponentInChildren<Text>().text = "已准备";
                  }
            }

      }
      //当收到选择枪协议
      private void OnMsgChoseGun(MsgBase msgBase)
      {
            MsgChoseGun msg = (MsgChoseGun)msgBase;
            string gunName = msg.gunname;
            int posId = msg.player.posId;
            Transform trans = content.GetChild(posId);
            //模型父物体
            Transform moldPa = trans.GetComponent<UiMapping>().map3DTarget;
            //模型枪的挂载点
            Transform gunPos = TransformHelper.FindChiled(moldPa.GetChild(0), "BigGunPos");
            HandGun(gunName, gunPos);
      }
      //给模型挂载枪
      private void HandGun(string gunName,Transform trans)
      {
            GameObject gunPrefab = ResourceManager.Load<GameObject>(gunName);
            GameObject gun = Instantiate(gunPrefab);

            if (trans.childCount > 0)
            { Destroy(trans.GetChild(0).gameObject); }
            gun.transform.SetParent(trans);
            gun.transform.localPosition = Vector3.zero;
            gun.transform.localEulerAngles = Vector3.zero;
            gun.transform.localScale = Vector3.one;
      }
      //当收到离开房间协议
      private void OnMsgLeaveRoom(MsgBase msgBase)
      {
            MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
            //如果离开成功
            if (msg.result == 0)
            {
                  PanelManager.Open<RoomListPanel>();
                  Close();
            }
      }
}

