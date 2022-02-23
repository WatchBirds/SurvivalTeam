using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using ConfigDic = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

public  static class AIConfigurationReader
{
      //把 配置文件中的信息 加载到 字典中
      public static ConfigDic Load(string aiConfigFile)
      {
            
            //aiConfigFile = Path.Combine(Application.streamingAssetsPath, aiConfigFile);
            string text = Resources.Load<TextAsset>(aiConfigFile).text;
            
            //if (Application.platform!=RuntimePlatform.Android)
            //{
            //      aiConfigFile = "file://" + aiConfigFile;
            //}
            //WWW www = new WWW(aiConfigFile);
            //while (true)
            //{
            //      if (!string.IsNullOrEmpty(www.error))
            //            throw new Exception("AI配置文件读取异常");
            //      if (www.isDone)
            //      {
            return BuildDic(text);
            //}

      }
      private static ConfigDic BuildDic(string lines)
      {
            ConfigDic dic = new ConfigDic();
            string mainKey = null;
            string subKey = null;
            string subValue = null;
            string line = null;
            StringReader reader = new StringReader(lines);
            while((line = reader.ReadLine())!=null)
            {
                  line = line.Trim();
                  if (!string.IsNullOrEmpty(line))
                  { //取主键
                        if(line.StartsWith("["))
                        {
                              mainKey = line.Substring(1, line.IndexOf("]") - 1);
                              dic.Add(mainKey, new Dictionary<string, string>());
                        }
                  //取子键
                        else
                        {
                              var configValue = line.Split('>');
                              subKey = configValue[0].Trim();
                              subValue = configValue[1].Trim();
                              if (mainKey != null)
                                    dic[mainKey].Add(subKey, subValue);
                              else
                                    throw new Exception("主键为空");
                        }
                  }
            };
            return dic;
      }
}
