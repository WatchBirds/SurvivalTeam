using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
      /// <summary>
      /// 攻击状态
      /// </summary>
      public class AttackingState : FSMState
      {
            private float attackTime;
            public override void Action(BaseFSM fSM)
            {
                  if (fSM.targetObject == null)
                        return;
                  if (attackTime >= fSM.chState.attackSpeed)
                  {
                        //调用攻击的攻击的方法，利用状态机调用
                        fSM.AutoUseSkill();//只攻击一次
                        attackTime = 0;
                  }
                  attackTime += Time.deltaTime;
                  fSM.transform.LookAt(fSM.targetObject);
                  fSM.transform.eulerAngles = new Vector3(0, fSM.transform.eulerAngles.y, 0);
            }
            public override void init()
            {
                  stateid = FSMstateID.Attacking;
            }
            public override void EnterState(BaseFSM fsm)
            {
                  fsm.StopMove();
                  fsm.PlayAnimation(fsm.animParams.Idle);
                  fsm.AutoUseSkill();
            }
      }
}
