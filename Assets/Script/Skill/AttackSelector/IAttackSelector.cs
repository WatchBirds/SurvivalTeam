using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARPGDemo.Skill 
{
    /// <summary>
    /// 攻击选择接口【算法】：选择某种范围中的敌人作为目标,
    ///                       例如：圆形范围，
    /// </summary>
    public interface IAttackSelector
    {
        /// <summary>
        /// 选择目标方法：选择 那些敌人作为要攻击的目标
        /// </summary>
        /// <param name="skillData">技能对象</param>
        /// <param name="transform">攻击范围的参考点，例如技能拥有着</param>
        GameObject[] SelectTarget(SkillData skillData, Transform transform);
    } }
