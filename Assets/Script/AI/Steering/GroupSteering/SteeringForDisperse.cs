using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 分散
      /// </summary>
      public class SteeringForDisperse : Steering
      {
            public Rader rader = new Rader();
            //分散后的最小距离
            public float nearDistance = 5; 
            public override Vector3 GetForce()
            {
                  //得到所有邻居
                  var allNeighbor = rader.SanNeighBors(transform.position);
                  //算出所有自己与所有邻居产生的排斥力的总和
                  expectForce = Vector3.zero;
                  for (int i = 0; i < allNeighbor.Length; i++)
                  {
                        var dir = transform.position - allNeighbor[i].transform.position;
                        if (dir.magnitude<nearDistance&&gameObject!=allNeighbor[i].gameObject)
                        {
                              expectForce += dir.normalized;//不考虑力大小
                        }
                  }
                  if (expectForce == Vector3.zero)
                  { return Vector3.zero; }
                  expectForce = expectForce.normalized * speed;
                  return (expectForce - vehicle.currentForce) * weight;
            }
      }
}
