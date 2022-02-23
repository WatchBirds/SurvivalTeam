
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Perception
{
      /// <summary>
      /// 视觉触发器
      /// </summary>
      public class SightTrigger : AbstractTrigger
      {
            public Transform recievePos;
            public override void Init()
            {
                  if (recievePos == null)
                        recievePos = transform;
                  triggerType = TriggerType.Sight;
            }
      }
}
