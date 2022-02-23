using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class NoticPanel : MonoBehaviour
{
      #region 公告栏

      [Tooltip("翻页速度")][Range(0,1)]
      public float speed = 0.2f;
      [Tooltip("翻页间隔时间")]
      public float time = 1.5f;//翻页间隔时间
      [Tooltip("是否启用自动翻页")]
      public bool autoActivity = true;
      [Tooltip("scrollrect")]
      public ScrollRect noticeBoard;
      [Tooltip("图片父物体")]
      public Transform noticContent;
      private int index = 0;//当前页数
      private int count;//页数
      private float upValue;//每一页间隔值 
      int lastIndex = 0;
      private Coroutine coroutine;
      public bool pointDown;
      private Dictionary<float, int> paperValue = new Dictionary<float, int>(); 
      private void Start()
      {
            count = noticContent.childCount;
            upValue = 1 / (float)(count - 1);
            for (int i = 0;i<count;i++)
            {
                  paperValue[upValue * i] = i ;
            }
            StartCoroutine(AutoNotic());
      }
      private IEnumerator AutoNotic()
      {
            Coroutine coroutineInside = null;
            float tempTime = 0;
            while (true)
            {
                  yield return null;
                  noticeBoard.horizontalNormalizedPosition = Mathf.Clamp(noticeBoard.horizontalNormalizedPosition, 0, 1);//限制范围
                  if (pointDown&&coroutineInside!=null)
                  {
                        StopCoroutine(coroutineInside);
                        tempTime = 0;
                  }
                  if (autoActivity)
                  {
                        tempTime += Time.deltaTime; 
                        if (tempTime >= time)
                        {
                              tempTime = 0;
                              yield return coroutineInside = StartCoroutine(Delay(index * upValue)); 
                        }
                  }
            }
      }
      IEnumerator Delay(float targetVlue)
      {
            while (Mathf.Abs(noticeBoard.horizontalNormalizedPosition-targetVlue)>0.01f)
            {
                  yield return null;
                  noticeBoard.horizontalNormalizedPosition= Mathf.Lerp(noticeBoard.horizontalNormalizedPosition, targetVlue, speed);
            }
            lastIndex = index;
            index = (index + 1) % count;//下一次的页数
            noticeBoard.horizontalNormalizedPosition = targetVlue;
      }
      private Vector2 downPos;
      private Vector2 upPos;
      public void PointDown(BaseEventData eventData)
      {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            downPos = pointerEvent.position;//记录位置
            pointDown = true;
            autoActivity = false;//停止自动翻页
            if(coroutine!=null)
            { 
                  StopCoroutine(coroutine);//停止协程
            }
            
      }
      public void PointUp(BaseEventData eventData)
      {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            upPos = pointerEvent.position;//记录位置
            autoActivity = true;//开启自动翻页
            pointDown = false;
            float pos = noticeBoard.horizontalNormalizedPosition;//记录位置
            if (upPos.x == downPos.x)
            {
                  return;
            }
            //找出最近的位置id
            List<float> valus = new List<float>(paperValue.Keys);
            float value = ArrayHelper.Min(valus.ToArray(),a=>Mathf.Abs(a-pos) );
            index = paperValue[value];//更新id
            coroutine = StartCoroutine(Delay(value));//开启协程
      }
      #endregion
}
