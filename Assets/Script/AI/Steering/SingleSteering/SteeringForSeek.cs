using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Steering
{
      public class SteeringForSeek : Steering
      {
            /// <summary>
            /// 靠近
            /// </summary>
            public override Vector3 GetForce()
            {
                  //期望力
                  expectForce = (target.position - transform.position).normalized * speed;
                  //实际力 = （期望力-当前力）* 权重
                  var realForce = (expectForce - vehicle.currentForce) * weight;
                  return realForce;
            }
      }
     
}