using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PanelManager 
{

      public enum Layer
      {
            Panel,
            Tip,
      }
      //层级列表
      private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
      //面板列表
      private static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
      //结构
      public static Transform root;
      public static Transform canvas;
      //初始化
      public static void Init()
      {
            root = GameObject.Find("Root").transform;
            canvas = TransformHelper.FindChiled(root, "Canvas");
            Transform panel = TransformHelper.FindChiled(canvas, "Panel");
            Transform tip = canvas.Find("Tip");
            layers.Add(Layer.Panel, panel);
            layers.Add(Layer.Tip, tip);
      }
      //打开面板
      public static void Open<T>(params object[] para)where T:BasePanel
      {
            //已经打开
            string name = typeof(T).ToString();
            if (panels.ContainsKey(name))
            { return; }
            //组件
            BasePanel panel = root.gameObject.AddComponent<T>();
            panel.OnInit();
            panel.Init();
            //父容器
            Transform layer = layers[panel.layer];
            panel.skin.transform.SetParent(layer,false);
            //列表
            panels.Add(name, panel);
            //OnShow
            panel.OnShow(para);
      }
      //关闭面板
      public static void Close(string name)
      {
            if (!panels.ContainsKey(name))
            {
                  return;
            }
            BasePanel panel = panels[name];
            //OnClose
            panel.OnClose();
            //移除
            panels.Remove(name);
            GameObject.Destroy(panel.skin);
            Component.Destroy(panel);
      }
      public static void CloseAllPanel()
      {
            List<string> names = new List<string>(0);
            foreach(var name in panels.Keys)
            {
                  names.Add(name);
            }
            foreach(var na in names)
            {
                  Close(na);
            }
      }
}
