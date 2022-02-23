using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.Character;
namespace FPS.item
{
      public class Bullet : MonoBehaviour
      {
            private float speed;//速度
            [Tooltip("对应特效")]
            public GameObject[] FX;
            [Tooltip("攻击目标标签")]
            public string[] tags;
            private int damage;//伤害
            private float maxDistace;//最大距离
            public void Init(Vector3 dirct, float speed,float distance,int damage)
            {
                  this.speed = speed;
                  maxDistace = distance;
                  this.damage = damage;
                  transform.rotation = Quaternion.LookRotation(dirct);
            }
            private void Update()
            {
                  if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistace,LayerMask.GetMask("Build","BulletHit")))
                  {
                        if (Vector3.Distance(transform.position, hit.point) <= Time.deltaTime * speed * 10)//命中
                        {
                              transform.position = hit.point;
                              CreatFX(hit.collider.tag, hit.point, -transform.forward);
                             // MonsterStatu player = hit.transform.GetComponentInParent<MonsterStatu>();
                              IOnDamager ondamager = hit.transform.GetComponentInParent<IOnDamager>();
                              if (ondamager != null&&hit.transform.tag!="Player")
                              {
                                    if (hit.transform.name == "Head")
                                    { damage *= 2; }
                                    ondamager.OnMsgDamage(damage,null);
                              }
                              GameObjectPool.instance.CollectObject(gameObject);
           
                        }
                  }
                  transform.Translate(Vector3.forward * speed * Time.deltaTime);
                  maxDistace -= Time.deltaTime * speed;
                  if (maxDistace <= 0)//达到最大距离没击中
                  {
                        GameObjectPool.instance.CollectObject(gameObject);
                  }
            }
            //击中创建特效
            private void CreatFX(string tag, Vector3 pos, Vector3 direct)
            {                
                  int index = Array.IndexOf<string>(tags, tag);
                  if (index < 0)
                        return;  
                  //发送击中协议
                  MsgHit msgHit = new MsgHit();
                  msgHit.fxname = FX[index].name;
                  msgHit.x = pos.x;
                  msgHit.y = pos.y;
                  msgHit.z = pos.z;
                  msgHit.ex = direct.x;
                  msgHit.ey = direct.y;
                  msgHit.ez = direct.z;
                  NetManager.Send(msgHit);
                  Quaternion quaternion = Quaternion.LookRotation(direct);
                  GameObjectPool.instance.CreatObject(FX[index].name, FX[index], pos, quaternion);
                  GameObjectPool.instance.CollectObject(transform.gameObject);
            }
      }
}
