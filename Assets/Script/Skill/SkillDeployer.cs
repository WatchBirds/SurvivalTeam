using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPGDemo.Skill
{
      public abstract class SkillDeployer : MonoBehaviour
      {
            [Header("音频")]
            public AudioSource audioSource;
            public AudioClip hitClip;
            /// <summary> 要施放的技能<summary> 
            private SkillData m_SkillData;
            public SkillData skillData
            {
                  get { return m_SkillData; }
                  set
                  {
                        if (value == null) return;
                        m_SkillData = value;//初始化代码多，业务逻辑多，可能性多:多变的
                        //调用 施放器的配置工厂类
                        attackSelector = DeployerConfigFactory.CreatAttackSelector(m_SkillData);
                        listSelfImpact = DeployerConfigFactory.CreatselfImpacts(m_SkillData);
                        listTargetImpact = DeployerConfigFactory.CreatTargetImpacts(m_SkillData);
                  }
            }
            protected IAttackSelector attackSelector;
            protected List<ISelfImpact> listSelfImpact = new List<ISelfImpact>();
            protected  List<ITargetImpact> listTargetImpact = new List<ITargetImpact>();
            /// <summary> 重置攻击目标</summary>
            public GameObject[] ResetTargets()
            {
                  var targets = attackSelector.SelectTarget(m_SkillData, transform);
                  if (targets != null && targets.Length != 0)
                        return targets;
                  else
                  {
                        return null;
                  }
            }
            /// <summary>施放技能</summary>
            public abstract void DeploySkill(Transform target);
            /// <summary>技能回收<summary>
            public void CollectSkill()
            {
                  if (m_SkillData.durationTime > 0)
                  {
                        GameObjectPool.instance.CollectObject(gameObject, m_SkillData.durationTime);
                  }
                  else
                        GameObjectPool.instance.CollectObject(gameObject);
            }
      }
}
