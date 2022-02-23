using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 账号相关协议
/// </summary>
public class MsgRegister:MsgBase
{
      public MsgRegister()
      {
            protoName = "MsgRegister";
      }
      //客户端发
      public string id = "";
      public string pw = "";
      //服务端回（0-成功，1-失败）
      public int result = 0;
}
/// <summary>
/// 登陆协议
/// </summary>
public class MsgLogin:MsgBase
{
      public MsgLogin()
      {
            protoName = "MsgLogin";
      }
      //客户端发
      public string id = "";
      public string pw = "";
      //服务端回（0-成功，1-失败,-1玩家没注册名称）
      public int result = 0;
      public string version;
      //0代表游客
      public int isOther;
}
/// <summary>
/// 提下线协议（服务端发送）
/// </summary>
public class MsgKick:MsgBase
{
      public MsgKick()
      {
            protoName = "MsgKick";
      }
      //原因（0-其他人登陆同一账号）
      public int reason = 0;
}
public class MsgChangeName:MsgBase
{
      public MsgChangeName()
      {
            protoName = "MsgChangeName";
      }
      /// <summary>
      /// 玩家账号
      /// </summary>
      public string id;
      /// <summary>
      /// 名称
      /// </summary>
      public string name;
      /// <summary>
      /// 0表示失败，1代表成功
      /// </summary>
      public int result;
}
public class MsgAccountData:MsgBase
{
      public MsgAccountData()
      {
            protoName = "MsgAccountData";
      }
      public string id;
      public string accountData;
      //0代表失败，1代表成功
      public int result;
}
