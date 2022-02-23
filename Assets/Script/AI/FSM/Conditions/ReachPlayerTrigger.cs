using System;
using System.Collections.Generic;
using UnityEngine;
namespace AI.FSM
{
      /// <summary>
      /// 目标在攻击范围
      /// </summary>
      public class ReachPlayerTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  if (fSM.targetObject != null)
                  {
                        //小于攻击距离
                        bool b = Vector3.Distance(fSM.transform.position, fSM.targetObject.position) <= fSM.chState.attackDistance/2||Vector3.Distance(fSM.transform.position, fSM.targetObject.position)<fSM.skillDiatance/2;
                        return b;
                  }
                  return false;
            }
            public override void Init()
            {
                  triggerid = FSMTriggerID.ReachPlayer;
            }
      }
}
