using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class GenereateResConfig :Editor
{
      [MenuItem("ResConfig/ResourcesConfig")]
      public static void Generate()
      {
            //获取Resources完整路径
            string path = Path.Combine(Application.dataPath, "Resources/");
            //获取Resource文件夹和其子文件夹中所有的预制体文件的完整路劲
            string[] resFiles = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            //生成键值对信息, 键为预制体名称，值为在Resources下的相对路径
            for (int i = 0; i < resFiles.Length; i++)
            {
                  string key = Path.GetFileNameWithoutExtension(resFiles[i]);
                  string value = resFiles[i].Replace(path, "").Replace(".prefab", "");
                  resFiles[i] = key + "=" + value; 
            }
            ////在资源根目录创建Config文件夹
            //string dirPath = Application.dataPath + "/Config";
            //if (!Directory.Exists(dirPath))
            //      Directory.CreateDirectory(dirPath);
            if (File.Exists(path+ "ResMap.txt"))
            {
                  if (EditorUtility.DisplayDialog("保存配置", path + "/ResMap.txt" + "已存在是否覆盖", "确认", "取消"))
                        File.WriteAllLines(Path.Combine(path, "ResMap.txt"), resFiles);
            }
            else 
            File.WriteAllLines(Path.Combine(path, "ResMap.txt"), resFiles);
            AssetDatabase.Refresh();
      }
}
