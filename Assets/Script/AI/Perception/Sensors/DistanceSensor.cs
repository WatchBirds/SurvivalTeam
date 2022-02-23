using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Perception
{
      public class DistanceSensor : AbstractSensor
      {
            [Tooltip("检测最远距离")]
            public float sightDistance = 1;
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
                  if (trigger.triggerType != TriggerType.Distance)
                        return false;
                  var tempTrigger = trigger as DistanceTrigger;
                  //距离
                  var dir = tempTrigger.recievePos.position - sendPos.position;
                  var result = dir.magnitude < sightDistance;
                  //遮挡 1射中物体 2射中的是否是触发器
                  //if (enableRay)
                  //{
                  //      RaycastHit hit;
                  //      bool b1 = Physics.Raycast(sendPos.position, dir, out hit, sightDistance) && hit.collider.gameObject == trigger.gameObject;
                  //      result = b1 && result;
                  //}
                  return result;
            }
      }
}
