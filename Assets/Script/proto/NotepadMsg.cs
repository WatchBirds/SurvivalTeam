using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 获取笔记本内容
/// </summary>
public class MsgGetText:MsgBase
{
      public MsgGetText()
      {
            protoName = "MsgGetText";
      }
      //服务端回
      public string text = "";
}
/// <summary>
/// 保存笔记本内容
/// </summary>
public class MsgSaveText:MsgBase
{
      public MsgSaveText()
      {
            protoName = "MsgSaveText";
      }
      //客户端发
      public string text = "";
      //服务端回（0-成功，1-失败）
      public int result = 0;
}
