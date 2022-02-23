using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
      public Transform target;
      public float speed;
      [Header("自动复位")]
      [Tooltip("是否启用自动复位")]
      public bool reBack;
      //延迟时间
      public float delayTime;
      [Header("自动旋转")]
      [Tooltip("是否启用自动旋转")]
      public bool autoRotate;
      public float autoSpeed;
      public Vector3 foward;
      private bool ifDrag;
      [Tooltip("是否冻结z轴")]
      public bool freezeZ;
      private void Start()
      {
            if(target==null)
            { target = transform; }
            foward = target.forward;
      }
      private float OffsetX;
      private float OffsetY;

      private void OnMouseDrag()
      {
            OffsetX = Input.GetAxis("Mouse X");//获取鼠标x轴的偏移量
            OffsetY = Input.GetAxis("Mouse Y");//获取鼠标y轴的偏移量
            if(freezeZ == true)
            { OffsetY = 0; }
            Vector3 vector = new Vector3(0, -OffsetX, OffsetY);
            target.Rotate(vector* speed, Space.World);
            ifDrag = true;
      }
      private void OnMouseUp()
      {
            ifDrag = false;
      }
      private void FixedUpdate()
      {
            if(autoRotate)
            { AutoRotate(); }
            if(reBack&&!ifDrag)
            {
                  ReBack();
            }
      }
      private void AutoRotate()
      {
            target.Rotate(0, 1 * autoSpeed, 0);
      }
      private Vector3 temp;
      private void ReBack()
      {
            if(target.forward!=foward)
            {
                  Quaternion quaternion = Quaternion.LookRotation(foward);
                  target.rotation = Quaternion.Lerp(target.rotation, quaternion, delayTime);
            }
      }
}
