using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgFire:MsgBase
{
      public MsgFire()
      {
            protoName = "MsgFire";
      }
      //开枪的玩家id服务端补充
      public string id;
      /// <summary>
      /// 开枪状态0代表在开枪1代表停止开枪
      /// </summary>
      public int state;
}
public class MsgReload:MsgBase
{
      public MsgReload()
      {
            protoName = "MsgReload";
      }
      public string id;
      public int state;
}
public class MsgChangeMold:MsgBase
{
      public MsgChangeMold()
      {
            protoName = "MsgChangeMold";
      }
      public string id;
      /// <summary>
      /// 0代表自动1代表单发
      /// </summary>
      public string mold;
}
      
