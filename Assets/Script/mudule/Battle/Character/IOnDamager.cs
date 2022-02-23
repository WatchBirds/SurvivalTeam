using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS.Character
{
      public delegate void CostHandle(int damage);
      public interface IOnDamager
      {     
            /// <summary>
            /// 受击
            /// </summary>
            /// <param name="damage">伤害值</param>
            void OnDamage(int damage);
            /// <summary>
            /// 发送受击消息
            /// </summary>
            /// <param name="damage"></param>
            void OnMsgDamage(int damage,string id);
            event CostHandle HpCostHandle;
      }
}