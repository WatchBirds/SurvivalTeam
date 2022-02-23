using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI.FSM
{
      abstract public class FSMTrigger
      {
            public FSMTriggerID triggerid;
            public FSMTrigger()
            {
                  Init();
            }
            abstract public void Init();
            abstract public bool HandleTrigger(BaseFSM fSM);    
      }
}
