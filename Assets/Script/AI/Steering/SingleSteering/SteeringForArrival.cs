using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 到达
      /// </summary>
      public class SteeringForArrival : Steering
      { 
            public float slowdownDistance = 5;//减速区半径
            public float arrivalDistance = 2;//
            public override Vector3 GetForce()
            {
                  //减速区外默认速度
                  float realSpeed = speed;
                  var distance = Vector3.Distance(transform.position, target.position);
                  //到达区内：控制力为零
                  if (distance <= arrivalDistance)
                        return Vector3.zero;
                  //减速区内 速度递减 且 最低速度为一
                  if (distance<=slowdownDistance)
                  {
                        realSpeed = distance / (slowdownDistance - arrivalDistance) * speed;
                        realSpeed = realSpeed < 1 ? 1 : realSpeed;
                  }
                  
                  expectForce = (target.position - transform.position).normalized * realSpeed;      
                  var realForce = (expectForce - vehicle.currentForce) * weight;
                  return realForce;
            }
      }
}
