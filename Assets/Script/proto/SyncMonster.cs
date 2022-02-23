using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgSyncMonster : MsgBase
{
      public MsgSyncMonster()
      {
            protoName = "MsgSyncMonster";
      }
      //客户端房主补充
      public float x = 0f;
      public float y = 0f;
      public float z = 0f;
      public float ex = 0f;
      public float ey = 0f;
      public float ez = 0f;
      public string id = "";//表示那个敌人
}

