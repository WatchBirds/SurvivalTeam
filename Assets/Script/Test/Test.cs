using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
      public Slider slider;

      public delegate bool handle();
      private void Start()
      {
            slider.onValueChanged.AddListener((float value) => { Debug.Log(value); });
            slider.onValueChanged.AddListener(delegate(float value) { Debug.Log(value); });

            //查找  名称为 SB 且 父物体 为 2B 且 挂了 Text的子物体
            FindchildWithCondition.Instance.FindChild(transform, "SB", (Transform traget) => traget.parent.name == "2B" &&  traget.GetComponent<Text>() != null);

            FindchildWithCondition.Instance.FindChild(transform, "SB", Mycondition);
      }
      private bool Mycondition(Transform traget)
      {
            if (traget.parent.name == "2B" && traget.GetComponent<Text>() != null)
            { return true; }
            return false;
      }
}
