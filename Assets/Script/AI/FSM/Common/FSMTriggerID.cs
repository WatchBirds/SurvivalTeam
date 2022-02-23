using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI.FSM
{
      /// <summary>
      /// 状态转换条件
      /// </summary>
      public enum FSMTriggerID
      {     
            NoHealth,//生命值为0
            SawPlayer,//发现目标
            ReachPlayer,//目标进入攻击范围
            LosePlayer,//丢失目标
            CompletePatrol,//完成巡逻
            KilledPlayer,//打死目标
            WithOutAttackRange,//目标不在攻击范围
      }
}
