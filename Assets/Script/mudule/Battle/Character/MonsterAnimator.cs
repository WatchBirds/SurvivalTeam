using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace FPS.Character
{
     public class MonsterAnimator:MonoBehaviour
      {
            private Animator animator;
            private string defaluePmt = "idle";
            private void Start()
            {
                  animator = GetComponentInChildren<Animator>();
            }
            /// <summary>
            /// 设置上一个动画参数为false当前动画参数为true
            /// </summary>
            /// <param name="pmtName">当前动画参数名称</param>
            public void SetBool(string pmtName)
            {

                  animator.SetBool(defaluePmt, false);
                  animator.SetBool(pmtName, true);
                  defaluePmt = pmtName;
            }
      }
}
