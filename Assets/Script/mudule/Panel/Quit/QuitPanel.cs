using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : BasePanel
{
      public Button yesBt;
      public Button backBt;
      public override void OnClose()
      {
         
      }

      public override void OnInit()
      {
            skinPath = "QuitPanel";
            layer = PanelManager.Layer.Tip;
      }

      public override void OnShow(params object[] para)
      {
            //查找组件
            yesBt = TransformHelper.FindChiled(skin.transform, "YesBt").GetComponent<Button>();
            backBt = TransformHelper.FindChiled(skin.transform, "BackBt").GetComponent<Button>();
            if(para!=null&&para.Length>0)
            { backBt.gameObject.SetActive(false); };
            //添加事件
            backBt.onClick.AddListener(Close);
            yesBt.onClick.AddListener(() => { Application.Quit(); });
      }
}
