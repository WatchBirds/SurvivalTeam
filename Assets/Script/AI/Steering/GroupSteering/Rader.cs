using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Steering
{
      [Serializable]
      public class Rader
      {
            public string neighborTag = "neighbor";
            public float scanRadius = 10;
            public GameObject[] SanNeighBors(Vector3 selfPos)
            {
                  //获取所有标签为 neighborTag 的物体
                  var arry = GameObject.FindGameObjectsWithTag(neighborTag);
                  //返回以自身为中心在半径内的所有物体
                  return Array.FindAll(arry, go => Vector3.Distance(go.transform.position, selfPos) <= scanRadius);
            }
      }
}
