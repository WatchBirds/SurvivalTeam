using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 避开障碍物
      /// </summary>
      public class SteeringForEvadeObstacle : Steering
      {
            //1障碍物标志
            public string obstacleTag = "obstacle";
            //2探头位置
            public Transform probePos;
            //3探头长度
            public float probeLength = 15;
            //4最小推力
            public float minPushForce = 30;
            public override Vector3 GetForce()
            {
                  if (Physics.Raycast(probePos.position, probePos.forward, out RaycastHit hit, probeLength) && hit.collider.tag == obstacleTag)
                  {
                        //由碰撞点返回一个推力
                        expectForce = hit.point - hit.transform.position;
                        if (expectForce.magnitude < minPushForce)
                        {
                              expectForce = expectForce.normalized * minPushForce;
                        }
                        return expectForce * weight;
                  }
                  return Vector3.zero;
            }
      }
}
