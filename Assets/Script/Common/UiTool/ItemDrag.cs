using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemDrag : MonoBehaviour,IDragHandler,IEndDragHandler,IPointerDownHandler
{
      public event Action<RectTransform,PointerEventData> OnDragHandle;
      public event Action<RectTransform, PointerEventData> OnEndDragHandle;
      public event Action OnDrop;
      public event Action<RectTransform, PointerEventData> OnPointDownHandle;
      public void OnDrag(PointerEventData eventData)
      {

            Vector3 pos;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out pos);
            transform.position = pos;
            OnDragHandle?.Invoke(transform.rectTransform(),eventData);
      }

      public void OnEndDrag(PointerEventData eventData)
      {
            OnEndDragHandle?.Invoke(transform.rectTransform(), eventData);
      }
      public void OnDestroy()
      {
            
      }
      public void Drop()
      {
            OnDrop?.Invoke();
      }

      public void OnPointerDown(PointerEventData eventData)
      {
            OnPointDownHandle?.Invoke(transform.rectTransform(), eventData);
      }
}
