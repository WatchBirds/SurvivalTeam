using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.item;
using FPS.Character;
using AI.Perception;
public abstract class BaseWeapenMana : MonoBehaviour
{
      public GunItem currentWea;
      public CharacterAnimEvent animEvent;
 
      /// <summary>
      /// 初始化
      /// </summary>
      /// <param name="gunName"></param>
      public abstract void Init(string gunName);
      /// <summary>
      /// 初始化一个枪
      /// </summary>
      /// <param name="gun"></param>
      public abstract void InitGun(GunItem gun);
      /// <summary>
      /// 外部调用丢弃枪
      /// </summary>
      /// <param name="gun"></param>
      public abstract void Drop(GunItem gun);
      /// <summary>
      /// 动画调用开火
      /// </summary>
      public abstract void AnimFire();
      /// <summary>
      /// 动画调用换弹
      /// </summary>
      public abstract void AnimOnReload();
      /// <summary>
      /// 动画调用取消瞄准
      /// </summary>
      public abstract void AnimOnCancleAim();
      /// <summary>
      /// 动画调用瞄准
      /// </summary>
      public abstract void AnimAim();
      /// <summary>
      /// 外部调用开火
      /// </summary>
      /// <param name="b"></param>
      public abstract void Fire(bool b);
      /// <summary>
      /// 外部调用瞄准
      /// </summary>
      public abstract void Aim();
      
}

