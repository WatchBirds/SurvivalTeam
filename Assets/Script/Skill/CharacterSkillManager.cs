using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
using AI.FSM;

namespace ARPGDemo.Skill {
    public class CharacterSkillManager : MonoBehaviour
    {
        //管理多个技能数据对象—技能容器
        public List<SkillData> skills = new List<SkillData>();
            //
            public BaseFSM fSM;
        //初始化
        private void Start()
        {
                  fSM = transform.GetComponent<BaseFSM>();
                  //为SkillData中某些字段初始化
            foreach (var skill in skills)
            {
                if ((!string.IsNullOrEmpty(skill.hitFxName)) && skill.hitFxPerfab == null)
                {
                    skill.hitFxPerfab = LoadPrefab(skill.hitFxName);
                }
                if ((!string.IsNullOrEmpty(skill.prefabName)) && skill.skillPrefab == null)
                {
                    skill.skillPrefab = LoadPrefab(skill.prefabName);
                }
                skill.Onwer = this.gameObject;
            }
        }
            private GameObject LoadPrefab(string resName)
            {
                  var prefabGo = ResourceManager.Load<GameObject>(resName);
                  #region 提前创建放入对象池防止卡帧
                  if (prefabGo == null)
                        return null                                                                                                                                                                                                                                                                                                             ;
                  var tempGo = GameObjectPool.instance.CreatObject(resName, prefabGo, transform.position,
                      transform.rotation);
                  GameObjectPool.instance.CollectObject(tempGo);
                  #endregion
                  return prefabGo;
            }
        public SkillData PrePareSkill(int id)
        {
            //1 根据技能id找 技能容器是否有这个技能
            var skill = skills.Find(a => a.skillID == id);
            //2 如果有，同时技能已经冷却完毕，而且角色SP足够返回
            if (skill != null)
            { 
                if (skill.coolRemain == 0)
                { return skill; }
            }
            return null;
        }
        //释放技能，调用施放器的施放方法
        public void DeploySkill(SkillData skillData)
        {
            var tempGo = GameObjectPool.instance.CreatObject(skillData.prefabName, skillData.skillPrefab, skillData.pos.position,
               skillData.Onwer.transform.rotation);
            var deployer = tempGo.GetComponent<SkillDeployer>();
            deployer.skillData = skillData;
            deployer.DeploySkill(fSM.targetObject);
            }
        //技能冷却处理
        public IEnumerator CoolTimeDown(SkillData skillData)
        {
            skillData.coolRemain = skillData.coolTime;
            while(skillData.coolRemain>0)
            {
                        yield return new WaitForFixedUpdate();
                skillData.coolRemain -= Time.deltaTime;
            }
            skillData.coolRemain = 0;
        }
        public float GetSkillCoolRemain(int id)
        {
            return skills.Find(a => a.skillID == id).coolRemain;
        }
    }   
}
