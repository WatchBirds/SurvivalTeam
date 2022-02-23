using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ARPGDemo.Skill
{
      public class HidingSelfImpact : ISelfImpact
      {
            public void SelfImpact(SkillDeployer skillDeployer, SkillData skillData, GameObject gameObject)
            {
                  //隐藏子物体
                  int count = gameObject.transform.childCount;
                  if (count <= 0)
                        return;
                  for (int i = 0; i < count; i++)
                  {
                        gameObject.transform.GetChild(i).gameObject.SetActive(false);
                  }
                  //让自身和目标位置相同
                  skillDeployer.StartCoroutine(Delay(gameObject,skillData));
                  skillDeployer.StartCoroutine(Move(gameObject, skillData));
            }
            private IEnumerator Delay( GameObject go,SkillData skillData)
            {
                  float time = skillData.durationTime;
                  while (time>0)
                  {   
                        time -= Time.deltaTime;  
                        if (skillData.attackTargets == null || time <= 0)
                        {
                              break;
                        }
                        yield return new WaitForFixedUpdate();
                  }
                  for (int i = 0; i<go.transform.childCount;i++)
                  {
                        go.transform.GetChild(i).gameObject.SetActive(true);
                  }
                  skillData.activated = false;
            }
            private IEnumerator Move(GameObject go,SkillData skill)
            {
                  while (skill.coolRemain > 0)
                  {  
                        if (skill.attackTargets!=null)
                        {
                              go.transform.position = skill.attackTargets[0].transform.position + new Vector3(Random.Range(-1, 2), 0, 0);
                        }
                        yield return new WaitForSeconds(skill.damageInterval);
                  }
            }
      } 
}
