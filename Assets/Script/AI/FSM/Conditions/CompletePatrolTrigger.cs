namespace AI.FSM
{
      class CompletePatrolTrigger : FSMTrigger
      {
            public override bool HandleTrigger(BaseFSM fSM)
            {
                  return fSM.IsPatrolComplete;
            }

            public override void Init()
            {
                  triggerid = FSMTriggerID.CompletePatrol;
            }
      }
}
