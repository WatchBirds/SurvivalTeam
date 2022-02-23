using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 聚集 
      /// </summary>
      public class SteeringForCollect : Steering
      {
            public Rader rader = new Rader();
            public float nearDistance;
            //与中心点最近的距离
            public override Vector3 GetForce()
            {
                  var center = Vector3.zero;
                  //得到所有邻居
                  var allNeighbor = rader.SanNeighBors(transform.position);
                  //算出所有自己与所有邻居的中心点
                  for( int i = 0;i<allNeighbor.Length;i++)
                  {
                        center += allNeighbor[i].transform.position;
                  }
                  center /= allNeighbor.Length;
                  //向中心点靠近
                  if(Vector3.Distance(transform.position,center)>nearDistance)
                  {
                        expectForce = (center - transform.position).normalized * speed;
                        return (expectForce - vehicle.currentForce) * weight;
                  }
                  return Vector3.zero;
            }
      }
}