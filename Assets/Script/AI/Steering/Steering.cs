using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      ///  操控类 : 各种自动运动【操控】 的共性
      /// </summary>
      public abstract class Steering:MonoBehaviour
      {
            /// <summary>
            /// 期望操控力
            /// </summary>
            [HideInInspector]
            public Vector3 expectForce;
            /// <summary>
            /// 速度
            /// </summary>
            public float speed = 3;
            /// <summary>
            /// 目标
            /// </summary>
            public Transform target;
            /// <summary>
            /// 运动体
            /// </summary>
            [HideInInspector]
            public Vehicle vehicle;
            /// <summary>
            /// 权重
            /// </summary>
            public int weight = 1;

            /// <summary>
            /// 计算实际操控力
            /// </summary>
            public abstract Vector3 GetForce();
           

            protected void Start()
            {
                  vehicle = GetComponent<Vehicle>();     
            }
      }
}
