using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Perception
{
      public class SoundTrigger : AbstractTrigger
      {
            public float caseDistance = 5;//传播距离
            public float lifeTime = 3;//声音有效时间
            public bool awakePlay = false;//初始是否有效\

            public override void Init()
            {
                  triggerType = TriggerType.Sound;
                  this.enabled = awakePlay;
            }
            public void OnEnable()
            {
                  StartCoroutine(TimeDown());
            }
            private IEnumerator TimeDown()
            {
                  yield return new WaitForSeconds(lifeTime);
                  this.enabled = false;
            }
      }
}