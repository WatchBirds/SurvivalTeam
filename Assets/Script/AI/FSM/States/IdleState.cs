using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.FSM
{
      public class IdleState : FSMState
      {
            public override void Action(BaseFSM fSM)
            {
                  //播放待机动画
                  fSM.PlayAnimation(fSM.animParams.Idle);
            }

            public override void init()
            {
                  //设置状态编号为 待机状态的编号
                  stateid = FSMstateID.Idle;
            }
      }
}
