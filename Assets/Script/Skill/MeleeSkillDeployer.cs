using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPGDemo.Skill
{
      public class MeleeSkillDeployer : SkillDeployer
      {
            private GameObject curretntTarget;
            public override void DeploySkill(Transform target)
            {
                  if (skillData == null)
                        return;
                  //确定目标
                  skillData.attackTargets = ResetTargets();
                  //执行所有自身影响
                  if(listSelfImpact!=null)
                  listSelfImpact.ForEach(a => a.SelfImpact(this, skillData, skillData.Onwer));
                  //执行所有目标影响
                  if (listTargetImpact != null)
                        listTargetImpact.ForEach(a => a.TargetImpact(this, skillData, null));
                  //回收技能
                  CollectSkill();
            }
      }
}
