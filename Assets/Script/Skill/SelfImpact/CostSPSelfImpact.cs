
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
namespace ARPGDemo.Skill
{
    /// <summary>
    /// 自身影响类：消耗SP
    /// </summary>
    public class CostSPSelfImpact:ISelfImpact
    {
        /// 影响自身的方法
        /// <param name="skillDeployer">技能施放器</param>
        /// <param name="skillData">技能数据对象</param>
        /// <param name="gameObject">自身或者队友对象</param>
        public void SelfImpact(SkillDeployer skillDeployer, SkillData skillData, GameObject go)
        {
            if (go == null)
                return;
            var chaStatu = go.GetComponent<BasePlayer>();
                //  chaStatu.OnCostSp(skillData.costSp);
        }
    }
}
