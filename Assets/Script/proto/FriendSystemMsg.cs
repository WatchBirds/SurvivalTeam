using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgAddFriend:MsgBase
{
      public MsgAddFriend()
      {
            protoName = "MsgAddFriend";
      }
      /// <summary>
      /// 发起人名称
      /// </summary>
      public string name;
      /// <summary>
      /// 目标id
      /// </summary>
      public string targetName;
}
public class MsgFriendResult:MsgBase
{
      public MsgFriendResult()
      {
            protoName = "MsgFriendResult";
      }
      public string name;
      /// <summary>
      /// 目标id
      /// </summary>
      public string targetName;
      /// <summary>
      /// o代表不同意，1代表同意
      /// </summary>
      public int result;
      public FriendData friendData;
}
[Serializable]
public class FriendData
{
      /// <summary>
      /// 好友名称
      /// </summary>
      public string name;
      /// <summary>
      /// 好友状态 0代表再线 -1代表离线 1代表房间内
      /// </summary>
      public int statu;
      /// <summary>
      /// 好友头像
      /// </summary>
      public string sprite;
}
public class MsgGetFriendsData : MsgBase
{
      public MsgGetFriendsData()
      {
            protoName = "MsgGetFriendsData";
      }
      public string name;
      public FriendData[] friends;
}
/// <summary>
/// 更新好友信息
/// </summary>
public class MsgUpdateFriendData : MsgBase
{
      public MsgUpdateFriendData()
      {
            protoName = "MsgUpdateFriendData";
      }
      /// <summary>
      /// 更新的好友名称
      /// </summary>
      public string name;
      /// <summary>
      /// 更新的信息名称
      /// </summary>
      public string dataName;
      /// <summary>
      /// 更新的值
      /// </summary>
      public string dataValue;
}
public class MsgFriendTalk:MsgBase
{
      public MsgFriendTalk()
      {
            protoName = "MsgFriendTalk";
      }
      /// <summary>
      /// 发送者
      /// </summary>
      public string name;
      /// <summary>
      /// 接收者
      /// </summary>
      public string targetName;
      /// <summary>
      /// 消息纪录
      /// </summary>
      public string fullstr;
      /// <summary>
      /// 当前消息
      /// </summary>
      public string str;
}
/// <summary>
/// 获取和好友的聊天纪录
/// </summary>
public class MsgGetFriendTalk:MsgBase
{
      public MsgGetFriendTalk ()
      {
            protoName = "MsgGetFriendTalk";
      }
      /// <summary>
      /// 目标名称
      /// </summary>
      public string targetName;
      public string fullstr;
}
/// <summary>
/// 邀请好友协议
/// </summary>
public class MsgInviteFriend:MsgBase
{
      public MsgInviteFriend()
      {
            protoName = "MsgInviteFriend";
      }
      /// <summary>
      /// 邀请者名称
      /// </summary>
      public string name;
      /// <summary>
      /// 被邀请者名称
      /// </summary>
      public string targetName;
      /// <summary>
      /// 房间id
      /// </summary>
      public int roomId;
      /// <summary>
      /// 邀请者信息
      /// </summary>
      public FriendData friendData;
}
