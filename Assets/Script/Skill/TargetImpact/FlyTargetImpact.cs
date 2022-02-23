using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ARPGDemo.Skill
{
      public class FlyTargetImpact : ITargetImpact
      {
            public void TargetImpact(SkillDeployer skillDeployer, SkillData skillData, GameObject gameObject)
            {
                  //遍历所有目标给目标刚体加力
                  if (skillData.attackTargets == null)
                  { return; }
                  foreach (var target in skillData.attackTargets)
                  {
                        target.transform.position += new Vector3(0, 5, 0);
                  }
            }
      }
}
