using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Perception
{
      class SightSensor : AbstractSensor
      {
            //视距
            public float sightDistance;
            [Tooltip("视角")]
            public float sightAngle;
            [Tooltip("启用角度检查")]
            public bool enableAngle;
            [Tooltip("启用遮挡检查")]
            public bool enableRay;
            //发射点
            public Transform sendPos;
            public override void Init()
            {
                  if (sendPos == null)
                        sendPos = transform;
            } 
            protected override bool TestTrigger(AbstractTrigger trigger)
            {
                  if (trigger.triggerType != TriggerType.Sight)
                        return false;
                  var tempTrigger = trigger as SightTrigger;
                  //距离
                  var dir = tempTrigger.recievePos.position - sendPos.position;
                  var result = dir.magnitude < sightDistance;
                  //角度
                  if (enableAngle)
                  {
                        bool b1 = Vector3.Angle(transform.forward, dir) < sightAngle / 2;
                        result = b1 && result;
                  }
                  //遮挡 1射中物体 2射中的是否是触发器
                  if (enableRay)
                  {
                        RaycastHit hit;
                        bool b1 = Physics.Raycast(sendPos.position, dir, out hit, sightDistance)&&hit.collider.gameObject == trigger.gameObject;
                        result = b1 && result;
                  }
                  return result;
            }
      }
}
