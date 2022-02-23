using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UnityEditor.Animations;
using UnityEngine;

namespace FPS.Character
{
      public class CharacherAnimator : MonoBehaviour
      {
            public  Animator animator;
            public string defaluePmt = "idle";
            public BoolParameter boolParameter = new BoolParameter();
            public FloatParameter floatParameter = new FloatParameter();
            
            public void Start()
            {
                  WeaPenMana.OnInit += WeaPenMana_OnInit;
                  animator = GetComponentInChildren<Animator>();
            }
            //当主玩家换枪的时候重新获取动画控制器
            private void WeaPenMana_OnInit(item.GunItem gun)
            {
                  animator =gun.GetComponent<Animator>();
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
            public void SetBool(string pmName,bool b)
            {
                  animator.SetBool(pmName, b);
                 
            }
            public void SetFola(string[] pmtNames, float[] values)
            {

                  for (int i = 0; i < pmtNames.Length; i++)
                  {
                        animator.SetFloat(pmtNames[i], values[i]);
                  }
            }
            public void SetFloat(string pmtName, float values)
            {
                  animator.SetFloat(pmtName, values);
                  
            }
            public void SetTrigger(string pmtName)
            {
                  animator.SetTrigger(pmtName);
            }
            public bool GetBool(string pmtName)
            {
                  return animator.GetBool(pmtName);
            }
            public float GetFloat(string pmtName)
            {
                  return animator.GetFloat(pmtName);
            }
      }
}
