using UnityEngine;

namespace AI.FSM
{
      public class NoHealthTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  
                  return fSM.chState.Hp <= 0;
            }

            public override void Init()
            {
                  triggerid = FSMTriggerID.NoHealth;
            }
      }
}
