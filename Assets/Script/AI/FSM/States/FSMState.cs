
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.FSM {
      /// <summary>
      /// 抽象状态类
      /// </summary>
      public abstract class FSMState
      {
            //字段*******
            //状态编号
            public FSMstateID stateid;
            //条件列表
            private List<FSMTrigger> triggers = new List<FSMTrigger>();
            //转换映射表
            private Dictionary<FSMTriggerID, FSMstateID> map = new Dictionary<FSMTriggerID, FSMstateID>();
            //方法*******
            public FSMState()
            {
                  init();
            }
            //初始化
            abstract public void init();
            
            //添加条件:条件和状态的 映射=对应关系
            public void AddTrigger(FSMTriggerID triggerid,FSMstateID stateid)
            {
                  if (map.ContainsKey(triggerid))
                  {
                        map[triggerid] = stateid;
                  }
                  else
                  {
                        map.Add(triggerid, stateid);
                        AddTriggerObject(triggerid);
                  }
            }
            //添加条件对象
            private void AddTriggerObject(FSMTriggerID triggerid)
            {
                  Type type = Type.GetType("AI.FSM." + triggerid + "Trigger");
                  if (type != null)
                  {
                        var triggerObj = (FSMTrigger)Activator.CreateInstance(type);
                        triggers.Add(triggerObj); 
                  }
            }
            //删除条件
            public void RemoveTrigger(FSMTriggerID triggerid)
            {
                  if (map.ContainsKey(triggerid))
                  {
                        map.Remove(triggerid);
                        RemoveTriggerObject(triggerid);
                  }
            }
            //删除条件对象
            private void RemoveTriggerObject(FSMTriggerID triggerid)
            {
                  triggers.RemoveAll(t => t.triggerid == triggerid);
            }
            //查找映射=查找条件映射
            public FSMstateID GetOutputState(FSMTriggerID triggerid)
            {
                  if (map.ContainsKey(triggerid))
                  {
                        return map[triggerid];
                  }
                  else
                        return FSMstateID.None;
            }
            /// <summary>
            /// 状态行为
            /// </summary>
            /// <param name="fSM"></param>
            abstract public void Action(BaseFSM fSM);
            /// <summary>
            /// 条件检查-大部分状态类似
            /// </summary>
            /// <param name="fsm"></param>
            public virtual void Reason(BaseFSM fsm)
            {
                  
                  for(int i = 0; i<triggers.Count;i++)
                  {
                        if (triggers[i].HandleTrigger(fsm))
                        {
                              fsm.ChangActiveState(triggers[i].triggerid);
                              return;
                        }
                  }
            }
            //离开状态
            public virtual void ExitState(BaseFSM fsm)
            { }
            //进入状态
            public virtual void EnterState(BaseFSM fsm)
            { }
      }
}