using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FPS.Character;
namespace ARPGDemo.Skill
{
    public class CircleAttackSelector : IAttackSelector
    {
        /// <summary>
        /// 选择目标方法：选择圆形敌人作为要攻击的目标
        /// </summary>
        /// <param name="skillData">技能对象</param>
        /// <param name="transform">攻击范围的参考点，例如技能拥有着</param>
        public GameObject[] SelectTarget(SkillData skillData, Transform skillTransform)
        {
            //通过球型射线获取给定半径的所有碰撞器
            var colliders = Physics.OverlapSphere(skillTransform.position, skillData.attackDistance);           
            if (colliders == null || colliders.Length == 0)
                return null;
            //找出标签为 skilldata.tag中的敌人并且hp不为0
            var enemys = ArrayHelper.FindAll(colliders, a => (Array.IndexOf(skillData.tags, a.tag) >= 0) &&
                 (a.GetComponent<BasePlayer>().Hp > 0));
            if (enemys == null || enemys.Length == 0)
                return null;
                 
            //根据技能攻击类型返回单个或多个敌人
            switch (skillData.attackType)
            {
                case SkillAttacckTape.Single:
                    var collider = ArrayHelper.Min(enemys, a => (Vector3.Distance(skillTransform.position, a.transform.position)));
                    return new GameObject[] { collider.gameObject };
                case SkillAttacckTape.Group:
                    return ArrayHelper.Select(enemys, a => a.gameObject);
            }
            return null;
        }
    }
}
