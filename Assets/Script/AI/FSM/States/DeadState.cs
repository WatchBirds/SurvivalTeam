using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.FSM
{
      /// <summary>
      /// 死亡状态
      /// </summary>
      public class DeadState : FSMState
      {
            public override void Action(BaseFSM fSM)
            {
                  
            }
            public override void init()
            {
                  stateid = FSMstateID.Dead; 
            }
            public override void EnterState(BaseFSM fsm)
            {
                  fsm.PlayAnimation(fsm.animParams.Dead);
                  fsm.enabled = false;
            }
      }
}
