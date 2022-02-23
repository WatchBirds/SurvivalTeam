using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMapp : MonoBehaviour
{
      private  float maxX = Screen.width-10;
      private float maxY = Screen.height-10;
      private  float minX = 10;
      private  float minY = 10;

      public void Mapping(Vector3 pos)
      {
            Vector3 temppos = Camera.main.WorldToScreenPoint(pos);
            if (temppos.z <= 0)
            {
                  if ((maxX - temppos.x) < 1)
                  {
                        minX = maxX;
                  }
                  else if ((temppos.x - minX) < 1)
                  {
                        maxX = minX;
                  }
                  if ((maxY - temppos.y) < 1)
                  {
                        minY = maxY;
                  }
                  else if ((temppos.y - minY) < 1)
                  {
                        maxY = minY;
                  }
            }
            else
            {
                  maxX = Screen.width - 10;
                  maxY = Screen.height - 10;
                  minX = 10;
                  minY = 10;
            }
            float x = Mathf.Clamp(temppos.x, minX,maxX );
            float y = Mathf.Clamp(temppos.y, minY,maxY );
            Vector2 newpos = new Vector2(x, y);
            (transform as RectTransform).anchoredPosition = newpos;
      }
}
