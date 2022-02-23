using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiMapping :MonoBehaviour
{
      public Transform map3DTarget;
      public float xoffset;
      private float x;
      public float yoffset;
      private float y;

      protected  void OnEnable()
      {
            if (map3DTarget != null)
            {
                  map3DTarget.gameObject.SetActive(gameObject.activeSelf);
                  InvokeRepeating("Maping", 0, 0.2f);
            }
      }
      private void Maping()
      {

            if (x != xoffset || y != yoffset)
            {  
                  x = xoffset;
                  y = yoffset;
                  (transform as RectTransform).anchorMax = Vector2.zero;
                  (transform as RectTransform).anchorMin = Vector2.zero;
                  Vector2 pos = Camera.main.WorldToScreenPoint(map3DTarget.position);
                  (transform as RectTransform).anchoredPosition = new Vector2(pos.x + xoffset, pos.y + yoffset);
            }
      }
      private void OnDisable()
      {
            map3DTarget.gameObject.SetActive(gameObject.activeSelf);
            CancelInvoke("Maping");
      }
}
