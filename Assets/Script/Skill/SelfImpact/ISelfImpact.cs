using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARPGDemo.Skill
{
    /// <summary>
    /// 自身影响算法【接口】
    /// </summary>
    public interface ISelfImpact
    {
        /// 影响自身的方法
        /// <param name="skillDeployer">技能施放器</param>
        /// <param name="skillData">技能数据对象</param>
        /// <param name="gameObject">自身或者队友对象</param>
        void SelfImpact(SkillDeployer skillDeployer, SkillData skillData, GameObject gameObject);
    }
}
