using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Perception
{
      public class SensorTriggerSystem : MonoSingleton<SensorTriggerSystem>
      {
            public override void Init()
            {
                  base.Init();
            }
            /// <summary>
            /// 检测时间间隔
            /// </summary>
            public float checkInterval = 0.2f;
            /// <summary>
            /// 感应器列表
            /// </summary>
            private List<AbstractSensor> listSensor = new List<AbstractSensor>();
            /// <summary>
            /// 触发器列表
            /// </summary>
            private List<AbstractTrigger> listTrigger = new List<AbstractTrigger>();

            /// <summary>
            /// 添加感应器
            /// </summary>
            public void AddSensor(AbstractSensor sensor)
            {
                  listSensor.Add(sensor);
            }

            /// <summary>
            /// 添加触发器
            /// </summary>
            public void AddTrigger(AbstractTrigger trigger)
            {
                  listTrigger.Add(trigger);
            }
            public void RemoveTrigger(AbstractTrigger trigger)
            {
                  listTrigger.Remove(trigger);
            }
            /// <summary>
            /// 检查触发条件:每个感应器 检查 对应触发器
            /// </summary>
            private void CheckTrigger()
            {
                  for(int i = 0; i<listSensor.Count;i++)
                  {
                        if (listSensor[i].enabled)
                        {
                              listSensor[i].OnTestTrigger(listTrigger);
                        }
                  }
            }
            private void Oncheck()
            {
                  
                  CheckTrigger();
                  UpdateSystem();
            }
            /// <summary>
            /// 更新系统
            /// </summary>
            private void UpdateSystem()
            {
                  listSensor.RemoveAll(s => s.isRemove);
                  listTrigger.RemoveAll(t => t.isRemove);
            }
            private void OnDisable()
            {
                  CancelInvoke("Oncheck");
            }

            private void OnEnable()
            {
                  InvokeRepeating("Oncheck", 0, checkInterval);
            }
            
      }
}