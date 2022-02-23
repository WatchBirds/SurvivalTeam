using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARPGDemo.Skill;

namespace FPS.Character
{
      /// <summary>
      /// 角色系统和技能系统的外观类：角色技能外观类
      /// </summary>
      public class CharacterSkillSystem : MonoBehaviour
      {
            #region deleget
            public delegate void Handle(SkillData skillData);
            public event Handle OnDeploySkill;
            public event Handle OnSkillOver;
            #endregion 

            private MonsterAnimator chAnim;
            private  SkillData currentUseSkill;
            private CharacterSkillManager skillMar;
            private int nextSkillId;
            private int Batterskillld;
            // Start is called before the first frame update
            void Start()
            {
                  
                  chAnim = GetComponent<MonsterAnimator>();
                  skillMar = GetComponent<CharacterSkillManager>();
                  GetComponentInChildren<CharacterAnimEvent>().attackHandle += DeploySkill;
                   
            }
            /// <summary>
            /// 使用指定编号的技能 进行攻击
            /// </summary>
            /// <param name="skillid">技能编号</param>
            public void AttackUseeSkill(int skillid)
            {
                  SkillData tempskill;
                  #region 2.0
                  ////判断nextskillid是否为0并且相同
                  //if (nextSkillId!=0&&skillid==Batterskillld)
                  //{
                  //      skillid = nextSkillId;
                  //}
                  //准备技能
                  tempskill = skillMar.PrePareSkill(skillid);
                  if (tempskill == null)
                        return;
                  ////判断技能是否为连击
                  //if(tempskill.nextBatterld!=0)
                  //{
                  //      nextSkillId = tempskill.nextBatterld;
                  //      Batterskillld = skillid;
                  //}
                  //else
                  //{
                  //      nextSkillId = 0;
                  //}
                  currentUseSkill = tempskill;
                  //播放动画施放技能
                  chAnim.SetBool(currentUseSkill.animationName);
                  currentUseSkill.activated = true;
                  //进行技能冷却
                  StartCoroutine(skillMar.CoolTimeDown(currentUseSkill));
                  //施放技能时禁用输入脚本
                  OnDeploySkill?.Invoke(currentUseSkill);                  
                  //检查技能施放施放完毕 技能施放完启用输入脚本
                  //if (OnSkillOver != null&&currentUseSkill.IfCheck==true)
                  //{
                  //      StartCoroutine(Check());
                  //}
                  #endregion
                  ////如果是连击，获取下一个技能编号
                  //if (isBatter && currentUseSkill != null)
                  //      skillid = currentUseSkill.nextBatterld;
                  ////通过编号准备技能
                  //currentUseSkill = skillMar.PrePareSkill(skillid);
                  //if (currentUseSkill == null)
                  //      return;
                  //chAnim.PlayAnimation(currentUseSkill.animationName);
            }
            //private IEnumerator Check()
            //{
            //      while ( true)
            //      {
            //            yield return new WaitForFixedUpdate();
            //            if (currentUseSkill.activated == false)
            //            {
            //                  OnSkillOver(currentUseSkill);
            //                  break;
            //            }
            //      }   
            //}
            public void DeploySkill ()
            {
                  Debug.Log(currentUseSkill);
                  skillMar.DeploySkill(currentUseSkill);
            }
            /// <summary>
            /// 使用随机技能
            /// </summary>
            public void UseRandowSkill()
            {
                  //从技能列表随机抽取一个可使用的技能 
                  // 找出所有已经冷却，且魔法消耗小于技能拥有者的剩余SP
                 var usableSkill = skillMar.skills.FindAll(skill => skill.coolRemain == 0/* && skill.costSp <= skill.Onwer.GetComponent<CharacterStatus>().SP*/);
                  //随机抽取集合中的一个技能对象
                  if (usableSkill!=null&&usableSkill.Count>0)
                  {
                        int index = Random.Range(0, usableSkill.Count);
                        int skillid = usableSkill[index].skillID; //获取随机技能编号
                        AttackUseeSkill(skillid);
                  }
            }
      }
}
