using UnityEngine;
namespace AI.FSM
{
      /// <summary>
      /// 发现目标
      /// </summary>
      public class SawPlayerTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  bool b = false;
                  if (fSM.targetObject != null) 
                        b = true;
                  return b;
            }

            public override void Init()
            {
                  triggerid = FSMTriggerID.SawPlayer;
            }
      }
}
