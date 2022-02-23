using System;
using System.Collections.Generic;
using UnityEngine;


namespace AI.FSM
{
      /// <summary>
      /// 丢失目标
      /// </summary>
      class LosePlayerTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
            //      if(fSM.targetObject!=null && Vector3.Distance(fSM.targetObject.position, fSM.transform.position) > fSM.sightDistance)
            //      {
            //            fSM.targetObject = null;
            //            return true;
            //      }
            if(fSM.targetObject == null)
                  { return true; }
                  return false;
            }

            public override void Init()
            {
                  triggerid = FSMTriggerID.LosePlayer;
            }
      }
}
