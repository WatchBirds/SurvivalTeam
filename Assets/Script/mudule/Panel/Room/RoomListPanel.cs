using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomListPanel : BasePanel
{
      //账号文本
      private Text idText;
      //玩家头像
      private Image hdImage;
      //创建房间按钮
      private Button creatButton;
      //刷新按钮列表
      private Button reflashButton;
      //房间列表容器
      private Transform content;
      //房间物体
      private GameObject roomObj;
      private Button closeButton;

      public override void OnClose()
      {
            //移除协议监听
            NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
            NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
            NetManager.RemoveMsgListener("MsgInviteFriend", OnMsgInviteFriend);
      }

      public override void OnInit()
      {
            skinPath = "RoomListPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            idText = TransformHelper.FindChiled(skin.transform, "InfoPanel/IdText").GetComponent<Text>();
            hdImage = TransformHelper.FindChiled(skin.transform, "InfoPanel/HdImage").GetComponent<Image>();
            creatButton = TransformHelper.FindChiled(skin.transform, "CtrlPanel/CreatButton").GetComponent<Button>();
            reflashButton = TransformHelper.FindChiled(skin.transform, "CtrlPanel/ReflashButton").GetComponent<Button>();
            content = TransformHelper.FindChiled(skin.transform, "ListPanel/Scroll View/Viewport/Content");
            roomObj = TransformHelper.FindChiled(skin.transform, "Room").gameObject;
            closeButton = TransformHelper.FindChiled(skin.transform, "BackButton").GetComponent<Button>();
            //按钮事件
            creatButton.onClick.AddListener(OnCreateClick);
            reflashButton.onClick.AddListener(OnReflashClick);
            closeButton.onClick.AddListener(() => { PanelManager.Open<MainPanel>(); Close(); });
            roomObj.SetActive(false);
            //显示id头像
            idText.text = GameMain.id;
            hdImage.sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + GameMain.sprite);
            //协议监听
            NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
            NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
            NetManager.AddMsgListener("MsgInviteFriend", OnMsgInviteFriend);
            //发送协议
            MsgGetRoomList getRoomList = new MsgGetRoomList();
            NetManager.Send(getRoomList);

      }
      private void OnMsgInviteFriend(MsgBase msgBase)
      {
            MsgInviteFriend msg = (MsgInviteFriend)msgBase;
            if (msg.friendData == null)
            { Debug.Log("frienddata null GameMain"); return; }
            PanelManager.Open<InvitePanel>(msg);
      }
      //当点击刷新按钮
      private void OnReflashClick()
      {
            //发送获取房间列表协议
            MsgGetRoomList getRoomList = new MsgGetRoomList();
            NetManager.Send(getRoomList);
      }
      //当点击加入房间
      private void OnJoinClick(string id)
      {
            //发送进入房间协议
            MsgEnterRoom msgEnter = new MsgEnterRoom();
            msgEnter.id = int.Parse(id);
            NetManager.Send(msgEnter);
      }
      //当点击新建房间按钮
      private void OnCreateClick()
      {
            //发送新建房间协议
            MsgCreateRoom msgCreate = new MsgCreateRoom();
            NetManager.Send(msgCreate);
      }
      //收到房间列表协议
      private void OnMsgGetRoomList(MsgBase msgBase)
      {
            MsgGetRoomList getRoomList = (MsgGetRoomList)msgBase;
            //清除房间列表
            for (int i = content.childCount; i > 0; i--)
            {
                  GameObject go = content.GetChild(i - 1).gameObject;
                  Destroy(go);
            }
            //如果返回的房间数为空返回
            if (getRoomList.rooms == null)
            { return; }
            for (int i = 0; i < getRoomList.rooms.Length; i++)
            {
                  GenerateRoom(getRoomList.rooms[i]);
            }
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
            }
            else
            {
                  PanelManager.Open<TipPanel>("进入房间失败");
            }
      }
      //收到创建房间协议
      private void OnMsgCreateRoom(MsgBase msgBase)
      {
            MsgCreateRoom msgCreate = (MsgCreateRoom)msgBase;
            //创建房间成功
            if (msgCreate.result == 0)
            {
                  PanelManager.Open<TipPanel>("创建成功");
                  PanelManager.Open<RoomPanel>();
                  Close();
                  return;
            }
            //创建房间失败
            PanelManager.Open<TipPanel>("创建失败");
      }
      //创建一个房间单元
      private void GenerateRoom(RoomInfo roomInfo)
      {
            //创建物体
            GameObject go = Instantiate(roomObj);
            go.transform.SetParent(content);
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            go.transform.localScale = Vector3.one;
            //获取组件
            Transform trans = go.transform;
            Text idText = TransformHelper.FindChiled(trans, "IdText").GetComponent<Text>();
            Text countText = TransformHelper.FindChiled(trans, "CountText").GetComponent<Text>();
            Text statusText = TransformHelper.FindChiled(trans, "StatusText").GetComponent<Text>();
            Image hdImage = TransformHelper.FindChiled(trans, "HdImage").GetComponent<Image>();
            Button enterBt = go.GetComponent<Button>();
            Text nameText = TransformHelper.FindChiled(trans, "NameText").GetComponent<Text>();
            //填充信息
            idText.text = roomInfo.id.ToString();
            countText.text = roomInfo.count.ToString();
            hdImage.sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + roomInfo.sprite);
            nameText.text = roomInfo.owerName;
            if (roomInfo.status == 0)
            {
                  statusText.text = "准备中";
            }
            else
            {
                  statusText.text = "游戏中";
            }
            //按钮事件
            enterBt.onClick.AddListener(() => { OnJoinClick(idText.text); });
      }
}
