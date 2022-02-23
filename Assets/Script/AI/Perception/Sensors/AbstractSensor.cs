using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI.Perception
{
      public abstract class AbstractSensor : MonoBehaviour
      {
            /// <summary>
            /// 是否移除，是否禁用
            /// </summary>
            public bool isRemove;

            /// <summary>
            /// 没有感知事件
            /// </summary>
            public event Action OnNonPerception;
            /// <summary>
            /// 感知事件
            /// </summary>
            public event Action<List<AbstractTrigger>> OnPerception;

            // Start is called before the first frame update
            void Start()
            {
                  Init();
                  //把当前感应器放到 感应系统中
                  SensorTriggerSystem.instance.AddSensor(this);
            }
            public abstract void Init();

            private void OnDestroy()
            {
                  isRemove = true;//把当前感应器 从感应系统中移除
            }

            /// <summary>
            /// 检测触发器：检查所有的触发器【触发 条件】
            /// </summary>
            public void OnTestTrigger(List<AbstractTrigger> listTriggers)
            {
                  //找到启用的 所有触发器(排除本身)
                  listTriggers = listTriggers.FindAll(t => t.enabled && t.gameObject != gameObject&&TestTrigger(t)&&t.gameObject.activeSelf!=false);
                  //触发感知事件
                  if (listTriggers.Count>0)
                  {
                        OnPerception?.Invoke(listTriggers);
                  }
                  else
                  {
                        OnNonPerception?.Invoke();
                  }
            }
            /// <summary>
            /// 检测触发器 是否被感知
            /// </summary>
            /// <param name="trigger"></param>
            protected abstract bool TestTrigger(AbstractTrigger trigger);

      }
}
