using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI.FSM
{
      /// <summary>
      /// 状态编号
      /// </summary>
      public enum FSMstateID
      {     
            None,             //无
            Idle,               //待机
            Dead,             //死亡
            Pursuit,          //追逐
            Attacking,    //攻击
            Default,        //默认
            Patrolling     //巡逻
      }
}
