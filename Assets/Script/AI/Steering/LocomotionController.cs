using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Steering
{
      /// <summary>
      /// 运动控制
      /// </summary>
      class LocomotionController : Vehicle
      {
            /// <summary>
            /// 移动
            /// </summary>
            public void Movement()
            {
                  currentForce += finalForce * Time.deltaTime;
                  currentForce = Vector3.ClampMagnitude(currentForce, maxSpeed);
                  transform.position += currentForce * Time.deltaTime;
            }
            /// <summary>
            /// 播放动画
            /// </summary>
            public void PlayAnimation()
            {
                 
            }
            /// <summary>
            /// 转向
            /// </summary>
            public void Rotation()
            {
                  if (currentForce!=Vector3.zero)
                  {
                        var qt = Quaternion.LookRotation(currentForce);
                        transform.rotation = Quaternion.Lerp(transform.rotation, qt, rotationSpeed * Time.deltaTime);
                  }
            }

            private void Update()
            {
                  Rotation();
                  Movement();
                  PlayAnimation();
            }
      }
}
