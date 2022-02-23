using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 路径跟随： 和巡逻类似
      /// </summary>
      class SteeringForFollowPath : Steering
      {
            /// <summary>
            /// 跟随模式
            /// </summary>
            public enum PartolMode
            {
                  Once,
                  Loop,
                  PingPong
            }
            //路点
            public Transform[] wayPoints;
            //当前路点 索引
            private int currentPIndex = 0;
            public PartolMode partolMode;
            //巡逻到达距离
            public float partolArriveDistance;
            public override Vector3 GetForce()
            {
                  //1 判断是否到达当前路点
                  if (Vector3.Distance(transform.position, wayPoints[currentPIndex].position) <= partolArriveDistance)
                  {
                        //2 是否是最后一个路点
                        if (currentPIndex == wayPoints.Length - 1)
                        {
                              //3根据模式选择下一个路点
                              switch (partolMode)
                              {
                                    case PartolMode.Once:
                                          return Vector3.zero;
                                    case PartolMode.PingPong:
                                          Array.Reverse(wayPoints);
                                          currentPIndex += 1;
                                          break;

                              }
                        }
                        currentPIndex = (currentPIndex + 1) % wayPoints.Length;
                  }
                  expectForce = (wayPoints[currentPIndex].position - transform.position).normalized * speed;
                  return (expectForce - vehicle.currentForce) * weight;
            }
           
      }
}
