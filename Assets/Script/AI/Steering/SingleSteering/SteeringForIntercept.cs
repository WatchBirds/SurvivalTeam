using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 拦截
      /// </summary>
      class SteeringForIntercept : Steering
      {
            private Vector3 tempPos;
            public void OnDrawGizmos()
            {
                  Gizmos.color = Color.red;
                  Gizmos.DrawSphere(tempPos, 1);
            }
            public override Vector3 GetForce()
            {
                  //在自己前方一定角度才拦截 大于20°小于160°否则进行靠近
                  var toTarget = target.position - transform.position;
                  var angle = Vector3.Angle(transform.forward, toTarget);
                  if (angle > 20 && angle < 160)
                  {
                        //1目标与运动体距离
                        var distance = (transform.position - target.position).magnitude;
                        //2时间
                        float targetSpeed = target.GetComponent<Vehicle>().currentForce.magnitude;
                        var time = distance / (targetSpeed + speed);
                        //推断的目标移动距离
                        var targetmoveDistance = targetSpeed * time;
                        //拦截点位置
                        var icPos = target.position + target.forward * targetmoveDistance;
                  tempPos = icPos;
                        expectForce = (icPos - transform.position).normalized * speed;
                  }
                  else
                  { expectForce = toTarget.normalized * speed; }
                  var realForce = (expectForce - vehicle.currentForce) * weight;
                  return realForce;
            }
      }
}
