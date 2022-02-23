using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;

namespace ARPGDemo.Skill
{
      /// <summary>
      /// 目标影响类：HP的减少
      /// </summary>
      public class DamageTargetImpact : ITargetImpact
      {
            int baseDamage = 0;
            /// <summary>
            /// 影响目标的方法
            /// </summary>
            /// <param name="skillDeployer">技能施放器</param>
            /// <param name="onWerSkill">技能数据对象</param>
            /// <param name="targetGameObject">目标对象</param>
            public void TargetImpact(SkillDeployer skillDeployer, SkillData onWerSkill, GameObject targetGameObject)
            {
                  if (onWerSkill.Onwer != null)
                  { 
                        //获取施放者的攻击
                        baseDamage = onWerSkill.Onwer.GetComponent<BasePlayer>().Damage;
                  }
                  skillDeployer.StartCoroutine(RepeatDamage(skillDeployer, onWerSkill));
            }
            /// <summary>
            /// 单次伤害
            /// </summary>
            /// <param name="onWerSkill"></param>
            /// <param name="goTarget"></param>
            private void OnceDamage(SkillData onWerSkill, GameObject goTarget)
            {
                  
                  //1 调用目标的OnDamage方法
                  var damageVa = baseDamage * onWerSkill.damage;  
                  var tarStatu = goTarget.GetComponent<BasePlayer>();//攻击目标状态
                  var selfStatu = onWerSkill.Onwer.GetComponent<BasePlayer>();//自身状态
                  //if (tarStatu.id == GameMain.id)
                  //{
                  //      MsgHitPlayer msg = new MsgHitPlayer();
                  //      msg.id = onWerSkill.Onwer.GetComponent<BasePlayer>().id;
                  //      msg.hitId = tarStatu.id;
                  //      msg.damage = (int)damageVa;
                  //      NetManager.Send(msg);
                  //}
                  IOnDamager onDamager = goTarget.GetComponent<IOnDamager>();
                  if(onDamager == null)
                  { return; }
                  onDamager.OnMsgDamage((int)damageVa, selfStatu.id);
                  ///tarStatu.OnDamage((int)damageVa);
                  //2 创建技能受击特效挂在目标身上
                  if (onWerSkill.hitFxPerfab == null || tarStatu.HitFxPos == null)
                  {
                        return;
                  }
                  //创建特效
                  var hitGo = GameObjectPool.instance.CreatObject(onWerSkill.hitFxName, onWerSkill.hitFxPerfab, 
                        tarStatu.HitFxPos.position, tarStatu.transform.rotation);
                  //挂在受击目标上
                  hitGo.transform.SetParent(tarStatu.HitFxPos);
                  //延迟回收特效
                  GameObjectPool.instance.CollectObject(hitGo, 1f);
            }
            private IEnumerator RepeatDamage(SkillDeployer skillDeployer, SkillData onWerSkill)
            {
                  float attackTime = 0;
                  do
                  {
                        //对onWerSkillData中所有目标敌人造成伤害
                        if (onWerSkill.attackTargets != null && onWerSkill.attackTargets.Length != 0)
                        {
                              for (int i = 0; i < onWerSkill.attackTargets.Length; i++)
                              {
                                    OnceDamage(onWerSkill, onWerSkill.attackTargets[i]);
                              }
                              //隔一段时间重新选择目标攻击,调用施放器的目标选择方法
                              yield return new WaitForSeconds(onWerSkill.damageInterval);
                              attackTime += onWerSkill.damageInterval;
                              onWerSkill.attackTargets = skillDeployer.ResetTargets();
                        }
                        else
                              yield return null;
                  }
                  while (attackTime < onWerSkill.durationTime);
            }
      }
}
