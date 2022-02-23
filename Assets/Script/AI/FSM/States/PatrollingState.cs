using UnityEngine;
using System;

namespace AI.FSM
{
      /// <summary>
      /// 巡逻状态
      /// </summary>
      class PatrollingState : FSMState
      {
            private int currentWayPoint;
            public override void Action(BaseFSM fSM)
            {
                  //1是否到达当前路点
                  if (Vector3.Distance(fSM.transform.position, fSM.wayPoints[currentWayPoint].position) < fSM.patrolArrivalDistance)
                  {
                        currentWayPoint = (currentWayPoint + 1) % fSM.wayPoints.Length;
                        //2是否是最后一个路点
                        if (currentWayPoint == fSM.wayPoints.Length - 1)
                        {
                              //根据巡逻的方式，决定 结束 ，再次开始
                              switch (fSM.patrolMode)
                              {
                                    case PatrolMode.Once:
                                          fSM.IsPatrolComplete = true;
                                          return;
                                    case PatrolMode.Pingpong:
                                          Array.Reverse(fSM.wayPoints);
                                          currentWayPoint += 1;
                                          break;
                              }
                        }
                  }
                  //移动
                  fSM.MoveToTarget(fSM.wayPoints[currentWayPoint].position, fSM.walkSpeed, fSM.patrolArrivalDistance);
                  //播放动画
                  fSM.PlayAnimation(fSM.animParams.Walk);
            }
            public override void EnterState(BaseFSM fsm)
            {
                  fsm.IsPatrolComplete = false;
            }
            public override void ExitState(BaseFSM fsm)
            {   
                  fsm.StopMove();
            }

            public override void init()
            {
                  stateid = FSMstateID.Patrolling;
            }
      }
}
