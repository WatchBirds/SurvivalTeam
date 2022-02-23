using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//同步玩家相关协议
public class MsgSyncPlayer:MsgBase
{
      /// <summary>
      /// 角色移动协议
      /// </summary>
      public MsgSyncPlayer()
      {
            protoName = "MsgSyncPlayer";    
      }
      public float x = 0f;
      public float y = 0f;
      public float z = 0f;
      public float ex = 0f;
      public float ey = 0f;
      public float ez = 0f;
      public float time;
      //动画参数的值
      public float foward;
      public float right;
      public float hand;
      //服务端补充
      public string id = "";//表示那个玩家
}

/// <summary>
/// 角色浮点型动画协议
/// </summary>
public class MsgMotion:MsgBase
{
      public MsgMotion()
      {
            protoName = "MsgMotion";
      }
      public string id;
      //浮点型的动画参数名称
      public string[] floatparaName;
      public float[] value;
}
/// <summary>
/// 角色bool型动画协议
/// </summary>
public class MsgMotionBool:MsgBase
{
      public MsgMotionBool()
      {
            protoName = "MsgMotionBool";
      }
      public string id;
      //string类型动画参数名
      public string paraName;
      public int result;//0代表true,1代表false
}
public class MsgJump:MsgBase
{
      public MsgJump()
      {
            protoName = "MsgJump";
      }
      public string id;
      /// <summary>
      /// 0代表跳跃状态1代表落地
      /// </summary>
      public int state;
}


