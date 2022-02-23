using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPGDemo.Skill
{
    //目标影响算法抽象【接口】
    public interface ITargetImpact
    {/// <summary>
    /// 影响目标的方法
    /// </summary>
    /// <param name="skillDeployer">技能施放器</param>
    /// <param name="skillData">技能数据对象</param>
    /// <param name="gameObject">目标对象</param>
         void TargetImpact(SkillDeployer skillDeployer, SkillData skillData, GameObject gameObject);
    }
}
