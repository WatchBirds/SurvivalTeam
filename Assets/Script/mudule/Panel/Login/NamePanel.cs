using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NamePanel : BasePanel
{
      private InputField inputField;
      private Button okButton;
      public override void OnInit()
      {
            skinPath = "NamePanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            inputField = TransformHelper.FindChiled(skin.transform, "InputField").GetComponent<InputField>();
            okButton = TransformHelper.FindChiled(skin.transform, "OkButton").GetComponent<Button>();

            NetManager.AddMsgListener("MsgChangeName", OnMsgChangeName);

            okButton.onClick.AddListener(() =>
            {
                  if (inputField.text == string.Empty || inputField.text.Length <= 2)
                  { PanelManager.Open<TipPanel>("名称太短或不正确"); return; }
                  MsgChangeName msg = new MsgChangeName(); msg.id = para[0].ToString(); msg.name = inputField.text; NetManager.Send(msg);
            });
      }
      private void OnMsgChangeName(MsgBase msgBase)
      {
            MsgChangeName msg = (MsgChangeName)msgBase;
            if (msg.result == 0)
            {
                  PanelManager.Open<TipPanel>("创建名称失败");
                  return;
            }
            else if(msg.result == 1)//创建成功
            {
                  PanelManager.Open<MainPanel>();
                  Close();
            }
            
      }
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgChangeName", OnMsgChangeName);
      }
}
