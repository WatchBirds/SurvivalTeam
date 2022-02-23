using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager
{
      /// <summary>
      /// 键为物体名，值为物体完整路径
      /// </summary>
      private static Dictionary<string, string> dic = new Dictionary<string, string>();
      static ResourceManager()
      {
            ReadConfig();
      }
      //读取配置文件内容存入字典
      private static void ReadConfig()
      {
            string cfText = Resources.Load<TextAsset>("ResMap").text;
            string line = null;
            StringReader reader = new StringReader(cfText);
            while ((line = reader.ReadLine())!=null)
            {
                  string[] keyValue = line.Split('=');
                  dic.Add(keyValue[0], keyValue[1]);
            }
      }
      //通过键返回预制体路径
      public static T Load<T>(string fabName)where T:Object
      {
            if (dic.ContainsKey(fabName))
                  return Resources.Load<T>(dic[fabName]);
            return null;
      }
}
