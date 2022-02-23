
using ARPGDemo.Skill;
using System;
using System.Collections.Generic;
/// <summary>
/// 施放器的初始化工厂类 【配置类】
/// </summary>
public class DeployerConfigFactory
{
     //根据条件的不同返回不同的对象，且对象有共同的父类或接口【工厂方法设计模式】
     public static IAttackSelector CreatAttackSelector(SkillData skill)
      {
            IAttackSelector selector = null;
            //switch (skill.damageMode)
            //{
            //      case DamageMode.Cricle:
            //            selector = new CircleAttackSelector();
            //            break;
            //      case DamageMode.Sector:
            //            selector = new SectorAttackSelector();
            //            break;
            //}
            //return selector;
            string allName = "ARPGDemo.Skill." + skill.damageMode + "AttackSelector";
            Type typeObj = Type.GetType(allName);
            selector = (IAttackSelector)Activator.CreateInstance(typeObj);
            return selector;
      }
      public static List<ISelfImpact> CreatselfImpacts(SkillData skill)
      {
          if(skill.selfImpacts.Length == 0)
            {
                  return null;
            }
            List<ISelfImpact> impacts = new List<ISelfImpact>();
            for (int i = 0; i < skill.targetImpacts.Length; i++)
            {
                  Type type = Type.GetType("ARPGDemo.Skill" + skill.selfImpacts[i]);
                  ISelfImpact impact = (ISelfImpact)Activator.CreateInstance(type);
                  impacts.Add(impact);
            }
            return impacts;
      }
      public static List<ITargetImpact> CreatTargetImpacts(SkillData skill)
      {
            if (skill.targetImpacts.Length == 0)
            {
                  return null;
            }
             List<ITargetImpact> impacts = new List<ITargetImpact>();
            for (int i = 0;i<skill.targetImpacts.Length;i++)
            {
                  Type type = Type.GetType("ARPGDemo.Skill." + skill.targetImpacts[i]);
                  ITargetImpact impact = (ITargetImpact)Activator.CreateInstance(type);
                  impacts.Add(impact);
            }
            return impacts;
      }
}