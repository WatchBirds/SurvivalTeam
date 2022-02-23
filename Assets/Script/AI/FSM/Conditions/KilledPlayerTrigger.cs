using System;
using System.Collections.Generic;

using FPS.Character;
namespace AI.FSM
{
      /// <summary>
      /// 杀死目标
      /// </summary>
      class KilledPlayerTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  if (fSM.targetObject != null && fSM.targetObject.GetComponent<BasePlayer>().Hp <= 0)
                  {
                        fSM.targetObject = null;
                        return true;
                  }
                  return false;
            }

            public override void Init()
            {
                  triggerid = FSMTriggerID.KilledPlayer;
            }
      }

}