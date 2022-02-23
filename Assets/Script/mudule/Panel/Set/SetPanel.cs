using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetPanel : BasePanel
{
      private Button saveBt;
      private Button cancleBt;
      private Slider nomaleSlider;
      private Slider sniperSlider;
      public static event Action OnSaveSliderData;
      public override void OnInit()
      {
            skinPath = "SetPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            saveBt = TransformHelper.FindChiled(skin.transform, "SaveBt").GetComponent<Button>();
            cancleBt = TransformHelper.FindChiled(skin.transform, "CancleBt").GetComponent<Button>();
            nomaleSlider = TransformHelper.FindChiled(skin.transform, "NomaleSlider").GetComponent<Slider>();
            sniperSlider = TransformHelper.FindChiled(skin.transform, "SniperSlider").GetComponent<Slider>(); 
            
            
            if (PlayerPrefs.HasKey("nomale"))
            {
                  nomaleSlider.value = PlayerPrefs.GetFloat("nomale");
            }
            if(PlayerPrefs.HasKey("sniper"))
            {
                  sniperSlider.value = PlayerPrefs.GetFloat("sniper");
            }
            //绑定事件
            cancleBt.onClick.AddListener(() => { Close(); });
            saveBt.onClick.AddListener(() => { SaveSliderValue(); OnSaveSliderData?.Invoke(); Close(); });
      } 
      private void SaveSliderValue()
      {
            PlayerPrefs.SetFloat("nomale", nomaleSlider.value);
            PlayerPrefs.SetFloat("sniper", sniperSlider.value);
            PlayerPrefs.Save();
      }
      public override void OnClose()
      {
            
      }
}
