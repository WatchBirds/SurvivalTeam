using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 逃离
      /// </summary>
      class SteeringForFlee : Steering
      {
            public float safeDistance = 10;//离开最大距离
            public override Vector3 GetForce()
            {
                  if (target == null)
                        return Vector3.zero;
                  var dis = transform.position - target.position;
                  if (dis.magnitude < safeDistance)
                  {
                        //期望力(自身位置-目标位置 )
                        expectForce = dis.normalized * speed;
                        //实际力 = （期望力-当前力）* 权重
                        var realForce = (expectForce - vehicle.currentForce) * weight;
                        return realForce;
                  }
                  return Vector3.zero;
            }
      }
}
