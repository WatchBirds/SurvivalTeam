using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 逃避 
      /// </summary>
      
      public class SteeringForEscape : Steering
      {
            public override Vector3 GetForce()
            {
                  //与目标角度角度小于20°或者大于160°逃避否则进行远离
                  var toSelf =  transform.position-target.position;
                  var angle = Vector3.Angle(transform.forward, toSelf);
                  if (angle < 20 || angle > 160)
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
                        expectForce = ( transform.position-icPos).normalized * speed;
                        
                  }
                  else
                  { expectForce = toSelf.normalized * speed; }
                  var realForce = (expectForce - vehicle.currentForce) * weight;
                  return realForce;
            }
      
      }
}
