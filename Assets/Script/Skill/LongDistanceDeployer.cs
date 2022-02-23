using UnityEngine;
using System.Collections;
namespace ARPGDemo.Skill
{
      public class LongDistanceDeployer : SkillDeployer
      {
            public float radius;
            private void Start()
            {
                  radius =skillData.attackDistance;
            }
            private void Update()
            {
                  TargetCheck();
            }
            public override void DeploySkill(Transform target)
            {
                  if (target == null)
                  { return; }
                  StartCoroutine(Deploy(target.position + Vector3.up * 1.2f));
            }
            IEnumerator Deploy(Vector3 target)
            {
                  transform.rotation = Quaternion.LookRotation(target - transform.position);
                  float lifetime = 10;
                  while (true)
                  {
                        lifetime -= Time.deltaTime;
                        if (lifetime <= 0)
                        { CollectSkill(); break; }
                        transform.position += transform.forward * Time.deltaTime * 10;
                        yield return null;
                  }
            }
            public void TargetCheck()
            {
                  var colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Build") | LayerMask.GetMask("BulletHit")|LayerMask.GetMask("Players"));
                  if (colliders == null || colliders.Length == 0)
                  {
                        return;
                  }
                  else
                  {
                        //创建特效
                        GameObject FXprefab = ResourceManager.Load<GameObject>("Explosion");
                        GameObject Fx = GameObjectPool.instance.CreatObject(FXprefab.name, FXprefab, transform.position, Quaternion.identity);
                        //确定目标
                        skillData.attackTargets = ResetTargets();
                        //执行所有自身影响
                        if (listSelfImpact != null)
                              listSelfImpact.ForEach(a => a.SelfImpact(this, skillData, skillData.Onwer));
                        //执行所有目标影响
                        if (listTargetImpact != null)
                              listTargetImpact.ForEach(a => a.TargetImpact(this, skillData, null));
                        //回收技能
                        CollectSkill();
                  }

            }
      }
}
