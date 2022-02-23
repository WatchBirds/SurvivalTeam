using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IconValue
{
      public string iconName;
      public float x;
      public float y;
      public float z;
      public float A;
      public float scale;
}
[Serializable]
public class IconValueConfig
{
      public IconValue[] iconValues;
}
public class SetInputPanel : BasePanel
{
      private Transform layer1;
      private RectTransform firBt;
      private RectTransform joy;
      private RectTransform airBt;
      private RectTransform relaodBt;
      private RectTransform jumpBt;
      private RectTransform firBt2;

      private Slider scaleSlider;
      private Slider aSlider;
      private Button backBt;
      private Button saveBt;

      IconValueConfig IValueConfig = new IconValueConfig();

      private RectTransform currentRect;

      public static event Action OnSaveIconConfigHandle;
      public override void OnInit()
      {
            skinPath = "SetInputPanel";
            layer = PanelManager.Layer.Panel;
      }

      public override void OnShow(params object[] para)
      {
            //查找组件
            layer1 = TransformHelper.FindChiled(skin.transform, "Layer1");
            firBt = TransformHelper.FindChiled(layer1, "FirBt").rectTransform();
            joy = TransformHelper.FindChiled(layer1, "Joy").rectTransform();
            airBt = TransformHelper.FindChiled(layer1, "AirBt").rectTransform();
            relaodBt = TransformHelper.FindChiled(layer1, "RelaodBt").rectTransform();
            jumpBt = TransformHelper.FindChiled(layer1, "JumpBt").rectTransform();
            firBt2 = TransformHelper.FindChiled(layer1, "FirBt2").rectTransform();

            Transform setparent = TransformHelper.FindChiled(skin.transform, "SaveView");
            scaleSlider = TransformHelper.FindChiled(setparent, "ScaleSlider").GetComponent<Slider>();
            aSlider = TransformHelper.FindChiled(setparent, "ASlider").GetComponent<Slider>();
            backBt = TransformHelper.FindChiled(setparent, "BackButton").GetComponent<Button>();
            saveBt = TransformHelper.FindChiled(setparent, "SureButton").GetComponent<Button>();
            //添加事件
            firBt.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;
            joy.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;
            airBt.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;
            relaodBt.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;
            jumpBt.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;
            firBt2.GetComponent<ItemDrag>().OnPointDownHandle += SetInputPanel_OnPointDownHandle;

            scaleSlider.onValueChanged.AddListener(ScaleSliderListener);
            aSlider.onValueChanged.AddListener(ASliderListener);
            saveBt.onClick.AddListener(()=>{ SaveConfig();Close(); });
            backBt.onClick.AddListener(Close);
            //读取配置文件
            IValueConfig = (IconValueConfig)JsonUtility.FromJson(File.ReadAllText(GameMain.inputConfigPath), typeof(IconValueConfig));
            if(IValueConfig == null)
            { return; }
            for(int i = 0;i< IValueConfig.iconValues.Length; i++)
            {
                  SetIcon(IValueConfig.iconValues[i]);
            }
      }
      private void SetIcon(IconValue iconValue)
      {
            RectTransform rect = TransformHelper.FindChiled(layer1, iconValue.iconName).rectTransform();
            if(rect == null)
            { return; }
            rect.anchoredPosition = new Vector2(iconValue.x, iconValue.y);
            rect.localScale = new Vector3(iconValue.scale, iconValue.scale, iconValue.scale);
            Image image = rect.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, iconValue.A);
      }
      private void SetInputPanel_OnPointDownHandle(RectTransform arg1, UnityEngine.EventSystems.PointerEventData arg2)
      { 
            currentRect = arg1;
            scaleSlider.value = arg1.localScale.x;
            aSlider.value = arg1.GetComponent<Image>().color.a;    
      }
      private void ScaleSliderListener(float value)
      {
            if(currentRect==null)
            { return; }
            currentRect.localScale = new Vector3(value, value, value);
      }
      private void ASliderListener(float value)
      {
            if (currentRect == null)
            { return; }
            Image image = currentRect.GetComponent<Image>();
           image.color = new Color(image.color.r, image.color.g, image.color.b, value);
      }
      private void SaveConfig()
      {
            IconValue[] valueConfigs = new IconValue[layer1.childCount];
           for (int i = 0;i<layer1.childCount;i++)
            {
                  RectTransform rect = layer1.GetChild(i).rectTransform();
                  IconValue valueConfig = new IconValue();
                  valueConfig.iconName = rect.name;
                  valueConfig.x = rect.anchoredPosition.x;
                  valueConfig.y = rect.anchoredPosition.y;
                  valueConfig.scale = rect.localScale.x;
                  valueConfig.A = rect.GetComponent<Image>().color.a;
                  valueConfigs[i] = valueConfig;
            }
            IconValueConfig iconValueConfig = new IconValueConfig();
            iconValueConfig.iconValues = valueConfigs;
            string contents = JsonUtility.ToJson(iconValueConfig);
            File.WriteAllText(GameMain.inputConfigPath, contents);
            OnSaveIconConfigHandle?.Invoke();
      }
      public override void OnClose()
      {
           
      }
}

