using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FPS.Character;
namespace ARPGDemo.Skill 
{
    /// <summary>
    /// 攻击选择类：选择扇形范围中的敌人作为目标,
    /// </summary>
    public class SectorAttackSelector : IAttackSelector
    {
        /// <summary>
        /// 选择目标方法：选择扇形区域敌人作为要攻击的目标
        /// </summary>
        /// <param name="skillData">技能对象</param>
        /// <param name="skillTransform"> 攻击范围的参考对象 ，例如技能拥有着</param>
        public GameObject[] SelectTarget(SkillData skillData, Transform skillTransform)
        {
            //获取所有tag为 skillData.tags的对象
            List<GameObject> listTargets = new List<GameObject>();
            foreach (var tag in skillData.tags)
            {
                var targets = GameObject.FindGameObjectsWithTag(tag);
                if (targets != null || targets.Length != 0)
                { listTargets.AddRange(targets); }
            }
            
            if (listTargets.Count == 0)
                return null;
            //找出一定距离中的敌人 hp不为0 扇形区域
            var enemys = listTargets.FindAll(a => (Vector3.Distance(skillTransform.position,
                a.transform.position) <= skillData.attackDistance) && (a.GetComponent<BasePlayer>().Hp > 0)
                && (Vector3.Angle(skillTransform.forward, a.transform.position - skillTransform.position) <= (skillData.attackAngle / 2)));
                  if (enemys == null || enemys.Count == 0)
                  {
                        return null;
                  }
            //根据技能攻击类型返回单个或多个敌人
            switch (skillData.attackType)
            {
                case SkillAttacckTape.Single:
                    var enemy = ArrayHelper.Min(enemys.ToArray(), a => (Vector3.Distance(skillTransform.position, a.transform.position)));
                    return new GameObject[] { enemy };
                case SkillAttacckTape.Group:
                    return enemys.ToArray();
            }
            return null;
        }
    }
}
