using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class TipPanel :BasePanel
{
      //提示文本
      private Text text;
      //确认按钮
      private Button okBtn;

      public override void OnClose()
      {
           
      }

      public override void OnInit()
      {
            skinPath = "TipPanel";
            layer = PanelManager.Layer.Tip;
      }

      public override void OnShow(params object[] para)
      {
            text = TransformHelper.FindChiled(skin.transform, "Text").GetComponent<Text>();
            okBtn = TransformHelper.FindChiled(skin.transform, "OkBtn").GetComponent<Button>();
            //添加okBtn监听
            okBtn.onClick.AddListener(OnOkClick);
            //提示语句
            if(para.Length == 1)
            {
                  text.text = para[0].ToString();
            }
      }
      //当按下ok按钮时
      private void OnOkClick()
      {
            Close();
      }
}
