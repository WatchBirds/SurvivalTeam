using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
      /// <summary>
      /// 玩家在视距内攻击距离外
      /// </summary>
      public class WithOutAttackRangeTrigger : FSMTrigger
      {
            private Animator animator;
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  if(animator == null)
                  { animator = fSM.GetComponentInChildren<Animator>(); }
                  if (fSM.targetObject != null)
                  {
                        bool b = (Vector3.Distance(fSM.targetObject.position, fSM.transform.position) > fSM.chState.attackDistance
                              && Vector3.Distance(fSM.targetObject.position, fSM.transform.position) < fSM.sightDistance
                              &&animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                              &&Vector3.Distance(fSM.targetObject.position, fSM.transform.position)>fSM.skillDiatance;
                       
                        return b;
                  }
                  return false;
            }
            public override void Init()
            {
                  triggerid = FSMTriggerID.WithOutAttackRange;
            }
      }
}
