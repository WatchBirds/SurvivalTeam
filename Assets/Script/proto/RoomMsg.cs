using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//游戏房间相关协议

/// <summary>/// 房间信息/// </summary>
[Serializable]
public class RoomInfo
{
      //房间id
      public int id;
      //人数
      public int count;
      //状态0-准备中 1-游戏中
      public int status;
      //房间头像
      public string sprite;
      /// <summary>
      /// 房主名称
      /// </summary>
      public string owerName;
}
/// <summary>
/// 请求房间列表协议
/// </summary>
public class MsgGetRoomList:MsgBase
{
      public MsgGetRoomList()
      {
            protoName = "MsgGetRoomList";
      }
      //服务端回
      public RoomInfo[] rooms;
}
/// <summary>
/// 创建房间协议
/// </summary>
public class MsgCreateRoom:MsgBase
{
      public MsgCreateRoom()
      {
            protoName = "MsgCreateRoom";
      }
      //服务端回0代表成功
      public int result = 0;
}
/// <summary>
/// 进入房间协议
/// </summary>
public class MsgEnterRoom:MsgBase
{
      public MsgEnterRoom()
      {
            protoName = "MsgEnterRoom";
      }
      //客户端发要进入的房间id
      public int id = 0;
      //服务端回0代表成功
      public int result = 0;
}
/// <summary>
/// 房间内玩家信息
/// </summary>
[Serializable]
public class PlayerInfo
{
      public string name = "player";//名称
      public int isOwner = 0;//是否是房主1代表是
      public int posId = 0;//房间内位置id
      public string gunName;//玩家选择的枪名称
      public int show = 0;//0代表隐藏1代表显示
      public PlayerFriend friend;
}
/// <summary>
/// 获取房间内信息协议
/// </summary>
public class MsgGetRoomInfo:MsgBase
{
      public MsgGetRoomInfo()
      {
            protoName = "MsgGetRoomInfo";
      }
      //服务端回
      public PlayerInfo[] players;
}
/// <summary>
/// 离开房间协议（发送该协议服务端处理时如果房间为战斗状态会发送离开战斗协议）
/// </summary>
public class MsgLeaveRoom:MsgBase
{
      public MsgLeaveRoom()
      {
            protoName = "MsgLeaveRoom";    
      }
      //服务端回
      public int result = 0;//0代表成功1代表失败
      public PlayerInfo playerInfo;
}
/// <summary>
/// 开始战斗协议
/// </summary>
public class MsgStartBattle:MsgBase
{
      public MsgStartBattle()
      {
            protoName = "MsgStartBattle";
      }
      //服务端回
      public int result = 0;
}
//玩家准备协议
public class MsgReady:MsgBase
{
      public MsgReady()
      {
            protoName = "MsgReady";
      }
      public PlayerInfo playerInfo;
      public int result = 0;
      public int show = 0;
}
//获取玩家信息协议
public class MsgGetPlayerData:MsgBase
{
      public MsgGetPlayerData()
      {
            protoName = "MsgGetPlayerData";
      }
      //服务端回
      public int result = 0;
      public string id;
      public PlayerData playerData;
}
//保存玩家信息协议
public class MsgSavePlayerData:MsgBase
{
      public MsgSavePlayerData()
      {
            protoName = "MsgSavePlayerData";
            playerData = new PlayerData();
      }
      //客户端发
      //信息名称
      public string dataName;
      //信息内容
      public PlayerData playerData;
      //服务端回
      public int result = 0;
      public string id;
}
//选择枪协议
public class MsgChoseGun:MsgBase
{
      public MsgChoseGun()
      {
            protoName = "MsgChoseGun";
      }
      public int result;//0代表失败，1代表成功
      //服务端回
      public PlayerInfo player;
      //客户端发
      public string gunname;
}
//发送消息协议
public class MsgTalk:MsgBase
{
      public MsgTalk()
      {
            protoName = "MsgTalk";
      }
      public string id;//发送者id服务端补充
      public string msg;//消息客户端补充
}


