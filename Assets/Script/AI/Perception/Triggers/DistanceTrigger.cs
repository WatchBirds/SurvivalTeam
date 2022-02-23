using UnityEngine;

namespace AI.Perception
{
      public class DistanceTrigger : AbstractTrigger
      {
            public Transform recievePos;
            public override void Init()
            {
                  if (recievePos == null)
                        recievePos = transform;
                  triggerType = TriggerType.Distance;
            }
      }
}
