using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Perception
{
      public class SoundSensor : AbstractSensor
      {
            public float hearDistance;//听力范围,听觉距离
            public override void Init()
            {
                  
            }
            protected override bool TestTrigger(AbstractTrigger trigger)
            {
                  if (trigger is SoundTrigger)
                  {
                        //类型转化
                        var tempTrigger = trigger as SoundTrigger;
                        //触发器和传感器的距离
                        var distance = Vector3.Distance(transform.position, tempTrigger.transform.position);
                        //听力范围+传播范围>触发器与传感器的距离
                        return tempTrigger.caseDistance + hearDistance > distance;
                  }
                  return false;
            }
      }
}
