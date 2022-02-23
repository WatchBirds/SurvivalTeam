using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Steering
{
      /// <summary>
      /// 徘徊
      /// </summary>
      public class SteeringForWander : Steering
      {
            //1 运动体与徘徊圆圆心的距离
            public float wanderDistance = 10;
            //2徘徊圆半径
            public float wanderRadius = 15;
            //3最大偏移量
            public float maxOffset = 200;
            //4徘徊圆圆周上的目标点
            private Vector3 circleTarget;
            //5改变目标的时间间隔
            public float changeTargetInterval = 3;
            //6目标点
            private Vector3 targetPos;

            private new void Start()
            {
                  base.Start();
                  circleTarget = new Vector3(wanderRadius, 0, 0);
                  InvokeRepeating("ChangeTarget", 0, changeTargetInterval);
            }
            //private void OnDrawGizmos()
            //{
            //      var sphereCenter = transform.position + transform.forward * wanderDistance;//徘徊圆圆心
            //      Gizmos.color = Color.blue;
            //      Gizmos.DrawWireSphere (sphereCenter, wanderRadius);
            //}
            private void ChangeTarget()
            {
                  //根据当前目标点 随机生成偏移位置
                  var offSetPosition = circleTarget+ new Vector3(Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset));
                  //将偏移后的位置 投射到圆周上
                  circleTarget = offSetPosition.normalized * wanderRadius;
                  //位置再折算成相对与运动体位置
                  targetPos = transform.position + transform.forward * wanderDistance + circleTarget;
            }
            public override Vector3 GetForce()
            {
                  expectForce = (targetPos - transform.position).normalized * speed;
                  var realForce = (expectForce - vehicle.currentForce) * weight;
                  return realForce;
            }
      }
}
