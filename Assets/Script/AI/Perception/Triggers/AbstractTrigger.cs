using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Perception
{
      /// <summary>
      /// 抽象触发器
      /// </summary>
      public abstract class AbstractTrigger : MonoBehaviour
      {
            /// <summary>
            /// 是否禁用
            /// </summary>
            public bool isRemove;
            /// <summary>
            /// 触发器类型 
            /// </summary>
            public TriggerType triggerType;

            // Start is called before the first frame update
            /// <summary>
            /// 初始化
            /// </summary>
            void Start()
            {
                  Init();
                  //把当前触发器 加入到 感应触发系统中
                  SensorTriggerSystem.instance.AddTrigger(this);
            }

            /// <summary>
            /// 初始化方法
            /// </summary>
            public abstract void Init();
           

            /// <summary>
            /// 销毁方法:把当前触发器从 感应触发系统中移除
            /// </summary>
            public void OnDestroy()
            {
                  isRemove = true;
            }
      }
}
