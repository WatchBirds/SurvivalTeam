using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class MyTouch
{
      /// <summary>  
      /// 定义的一个手指类  
      /// </summary>  
      public class MyFinger
      {
            public int id = -1;
            public Touch touch;
      }

      static private List<MyFinger> fingers = new List<MyFinger>();
      /// <summary>  
      /// 手指容器  
      /// </summary>  
      static public List<MyFinger> Fingers
      {
            get
            {
                  if (fingers.Count == 0)
                  {
                        for (int i = 0; i < 5; i++)
                        {
                              MyFinger mf = new MyFinger();
                              mf.id = -1;
                              fingers.Add(mf);
                        }
                  }
                  return fingers;
            }
      }
      private static void UpdateFinger()
      {
            Touch[] touches = Input.touches;
            // 遍历所有的已经记录的手指  
            // --掦除已经不存在的手指  
            foreach (MyFinger mf in Fingers)
            {
                  if (mf.id == -1)
                  {
                        continue;
                  }
                  bool stillExit = false;
                  foreach (Touch t in touches)
                  {
                        if (mf.id == t.fingerId)
                        {
                              stillExit = true;
                              break;
                        }
                  }
                  // 掦除  
                  if (stillExit == false)
                  {
                        mf.id = -1;
                  }
            }
            // 遍历当前的touches  
            // --并检查它们在是否已经记录在AllFinger中  
            // --是的话更新对应手指的状态，不是的放放加进去  
            foreach (Touch t in touches)
            {
                  bool stillExit = false;
                  // 存在--更新对应的手指  
                  foreach (MyFinger mf in MyTouch.Fingers)
                  {
                        if (t.fingerId == mf.id)
                        {
                              stillExit = true;
                              mf.touch = t;
                              break;
                        }
                  }
                  // 不存在--添加新记录  
                  if (!stillExit)
                  {
                        foreach (MyFinger mf in MyTouch.Fingers)
                        {
                              if (mf.id == -1)
                              {
                                    mf.id = t.fingerId;
                                    mf.touch = t;
                                    break;
                              }
                        }
                  }
            }
      }
      public static Touch GetLastTouch()
      {
            UpdateFinger();

            MyFinger[] figs = ArrayHelper.FindAll(Fingers.ToArray(), a => (a.id != -1)&&(Camera.main.ScreenToViewportPoint(a.touch.position).x>0.5));
            return ArrayHelper.Max(figs, a => a.id).touch;
      }
}

