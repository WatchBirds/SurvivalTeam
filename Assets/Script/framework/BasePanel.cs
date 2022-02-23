using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
      //皮肤路径
      public string skinPath;
      //皮肤
      public GameObject skin;
      //层级
      public PanelManager.Layer layer;
      /// <summary>
      /// 初始化
      /// </summary>
      public  void Init()
      {
            //皮肤
            GameObject skinPrefab = ResourceManager.Load<GameObject>(skinPath);
            skin = Instantiate(skinPrefab);
      }
      //关闭
      public void Close()
      {
            string name = this.GetType().ToString();
            PanelManager.Close(name);
      }
      /// <summary>
      /// 初始化时
      /// </summary>
      public abstract void OnInit();

      /// <summary>
      /// 显示时
      /// </summary>
      public abstract void OnShow(params object[] para);
      /// <summary>
      /// 关闭时
      /// </summary>
      public abstract void OnClose();
}
