using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.item;
using System;
namespace FPS.Character
{
      public abstract class BasePlayer : MonoBehaviour, IOnDamager
      {

            //角色模型
            protected GameObject skin;
            //玩家id
            public string id;
            //攻击对象标签
            public string[] attackTags;
            public int Damage;
            /// <summary>
            /// 初始化模型
            /// </summary>
            /// <param name="skinName">模型名称</param>
            public virtual void Init(string skinName)
            {
                  GameObject skinRes = ResourceManager.Load<GameObject>(skinName);
                  skin = Instantiate(skinRes);
            }
            #region
            public event CostHandle HpCostHandle;
            #endregion

            /// <summary>
            /// 防御
            /// </summary>
            public int Defence;

            /// <summary>
            /// 生命
            /// </summary>
            public int Hp;

            /// <summary>
            /// 最大生命
            /// </summary>
            public int MaxHp;

            public Transform HitFxPos;
            /// <summary>
            /// 攻击距离
            /// </summary>
            public float attackDistance = 1.2f;
            /// <summary>
            /// 攻击速度
            /// </summary>
            public float attackSpeed = 1;
            /// <summary>
            /// 死亡
            /// </summary>
            abstract public void Dead();

            virtual public void OnDamage(int damage)
            {
                  if(Hp<=0)
                  { return; }
                  if(damage<=0)
                  { return; }
                  Hp -= damage;
                  HpCostHandle?.Invoke(Hp);
            }
            virtual public void OnReBorn(int value)
            {
                  Hp = value;
                  HpCostHandle?.Invoke(Hp);
            }

           virtual  public void OnMsgDamage(int damage,string id)
            {
                 
            }
      }
}
