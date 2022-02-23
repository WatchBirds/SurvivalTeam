using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemSetPanel : BasePanel
{
      private Button setPosBt;
      private Button setViewBt;
      private Button closeBt;
      public Button leaveBt;
      public override void OnClose()
      {
            
      }

      public override void OnInit()
      {
            skinPath = "SystemSetPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            setViewBt = TransformHelper.FindChiled(skin.transform, "SetViewBt").GetComponent<Button>();
            setPosBt = TransformHelper.FindChiled(skin.transform, "SetPosBt").GetComponent<Button>();
            closeBt = TransformHelper.FindChiled(skin.transform, "CloseBt").GetComponent<Button>();
            leaveBt = TransformHelper.FindChiled(skin.transform, "LeaveBt").GetComponent<Button>();

            bool b = (bool)para[0];
            leaveBt.gameObject.SetActive(b);
            if(b)
            {
                  leaveBt.onClick.AddListener(() => { MsgLeaveBattle msg = new MsgLeaveBattle(); NetManager.Send(msg); Close(); });
            }
            setPosBt.onClick.AddListener(()=> { PanelManager.Open<SetInputPanel>(); });
            setViewBt.onClick.AddListener(() => { PanelManager.Open<SetPanel>(); });
            closeBt.onClick.AddListener(Close);
      }

}
