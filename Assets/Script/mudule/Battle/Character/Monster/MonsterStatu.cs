using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Perception;
using System;
namespace FPS.Character
{
      public class MonsterStatu : BasePlayer
      {
            //被击杀后玩家可得的金币数
            public int coint;
            public string monsterName;
            
            public override void Dead()
            {
                  GetComponent<AbstractSensor>().isRemove = true;
                  Destroy(gameObject, 5f);                
            }

            public override void OnMsgDamage(int damage,string id)
            {
                  if (Hp <= 0)
                  { return; }
                  damage -= Defence;
                  if (damage > 0)
                  {
                        //发送受击协议
                        MsgHitCharacter msg = new MsgHitCharacter();
                        msg.damage = damage;
                        msg.hitId = this.id;
                        msg.hitName = monsterName;
                        NetManager.Send(msg);
                  }
            }
            public override void OnDamage(int damage)
            {
                  base.OnDamage(damage);
            }
      }

}
