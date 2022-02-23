using UnityEngine;

namespace AI.FSM
{
      /// <summary>
      /// 追逐状态
      /// </summary>
      public class PursuitState : FSMState
      {
            public override void Action(BaseFSM fSM)
            {
                  //1:条件需要有追逐的目标
                  if (fSM.targetObject == null)
                        return;
                  //2：播放跑动画
                  fSM.PlayAnimation(fSM.animParams.Run);
                  //3：追的速度，靠近的距离=攻击距离/2
                  fSM.MoveToTarget(fSM.targetObject.position, fSM.moveSpeed, fSM.chState.attackDistance/2);
            }
            public override void init()
            {
                  stateid = FSMstateID.Pursuit;
            }
            public override void ExitState(BaseFSM fsm)
            {
                  //4:停下—转换状态
                  fsm.StopMove();
            }
    
      }
}
