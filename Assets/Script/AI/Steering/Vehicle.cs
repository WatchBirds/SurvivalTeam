using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
namespace AI.Steering
{
      /// <summary>
      /// 运动体
      /// </summary>
      public abstract class Vehicle : MonoBehaviour
      {
            /// <summary>
            /// 计算间隔
            /// </summary>
            public float computelnterval = 0.2f;
            /// <summary>
            /// 当前操控力
            /// </summary>
            [HideInInspector]
            public Vector3 currentForce;
            /// <summary>
            /// 合力
            /// </summary>
            [HideInInspector]
            public Vector3 finalForce;
            /// /// <summary>
            /// 操控对象数组
            /// </summary>
            [HideInInspector]
            public Steering[] steerings;
            /// <summary>
            /// 是否为平面
            /// </summary>
            public bool isPlane = false;
            /// <summary>
            /// 质量
            /// </summary>
            public float mass = 1;
            /// <summary>
            /// 合力上限
            /// </summary>
            public float maxForce= 100;
            /// <summary>
            /// 最大速度
            /// </summary>
            public float maxSpeed = 10;
            /// <summary>
            /// 转向速度
            /// </summary>
            public float rotationSpeed = 3;
            public MonsterAnimator animator;
            
            // Start is called before the first frame update
            void Start()
            {
                  animator = GetComponent<MonsterAnimator>();
                  steerings = GetComponents<Steering>();
            }
            /// <summary>
            /// 计算合力
            /// </summary>
            public void ComputeFinalForce()
            {
                  finalForce = Vector3.zero;
                  for (int i = 0; i<steerings.Length;i++)
                  {
                        if (steerings[i].enabled)
                        {
                              finalForce += steerings[i].GetForce();
                        }
                  }
                  if (finalForce == Vector3.zero)
                  { currentForce = Vector3.zero;
                        return;
                  }
                  if (isPlane)
                  { finalForce.y = 0; }
                  //限制向量大小在一个特定范围，不超过合力上限
                  finalForce = Vector3.ClampMagnitude(finalForce, maxForce);
                  finalForce /= mass;
            }

            private void OnDisable()
            {
                  CancelInvoke("ComputeFinalForce");
            }

            private void OnEnable()
            {
                  InvokeRepeating("ComputeFinalForce", 0, computelnterval);
            }
      }
}
