using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeItem : MonoBehaviour
{
      [Tooltip("瞄准时枪相机的视野")]
      public float aimGunCamerView;
      [Tooltip("瞄准时主相机的视野")]
      public float aimMainCamerView;
      [Tooltip("瞄准时枪相机时是正交的（true），是透视的（false）？")]
      public bool orthographic;
      [Tooltip("动画参数scopeType的值")]
      public float value;
      public Transform dotFX;
      public void Hiding()
      {
            gameObject.SetActive(false);
            dotFX.gameObject.SetActive(false);
      }
      public void Show()
      {
            gameObject.SetActive(true);
            dotFX.gameObject.SetActive(true);
      }
}
