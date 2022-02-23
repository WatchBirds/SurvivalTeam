using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FPS.Character
{
      public class CharacterAnimEvent:MonoBehaviour
      {
            public delegate void Handle();
            public event Handle fireHandle;
            public event Handle attackHandle;
            public event Handle aimHandle;
            public event Handle cancleAimHandle;
            public event Handle reloadHandle;
            private Animator animator;
            private void Start()
            {
                  animator = GetComponent<Animator>();
            }
            //动画开火事件
            public void Fire()
            {
                  fireHandle?.Invoke();
            }
            /// <summary>
            /// 攻击时使用(怪物动画)
            /// </summary>
            public void OnAttack()
            {
                  attackHandle?.Invoke();
            }
            //当瞄准时
            public void OnAim()
            {
                  aimHandle?.Invoke();
            }
            //当取消瞄准时
            public void OnCancleAim()
            {
                  cancleAimHandle?.Invoke();
            }
            //当换弹时
            public void OnReload()
            {
                  reloadHandle?.Invoke();
            }
            public void Cancle(string para)
            {
                  animator.SetBool(para, false);
            }
            public void MonsterCancle(string para)
            {
                  animator.SetBool(para, false);
            }
      }

}
