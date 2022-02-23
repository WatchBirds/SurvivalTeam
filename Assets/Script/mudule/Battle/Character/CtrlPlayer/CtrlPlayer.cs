using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.item;
using System;

namespace FPS.Character
{
      public class CtrlPlayer : BasePlayer
      {
            //public static event Action<GameObject,GameObject> OnCtrlPlayerColliEnter;
            //public static event Action OnCtrlPlayerColliExit;
            public static event Action<Iinteraction, GameObject> OnPlayerCheck;
            public static event Action OnPlayerDie;
            public static event Action <CtrlPlayer>OnPlayerBorn;

            private CharacterController chController;
            [Header("检测系统")]
            [Tooltip("检测半径")]
            public float checkRadius= 1.5f;
            [Tooltip("检测间隔时间")]
            public float checkTiem = 0.2f;
            private void OnEnable()
            {
                  InvokeRepeating("ContectCheck", 0, checkTiem);
            }
            private void OnDisable()
            {
                  CancelInvoke();
            }
            public override void Dead()
            {
                  MsgPlayerReborn reborn = new MsgPlayerReborn();
                  BattleManager.instance.SendMsg(reborn, 18);
                  Camera.main.transform.SetParent(null);
                  Camera.main.fieldOfView = 60;
                  OnPlayerDie?.Invoke();
            }
            //public void Born()
            //{
            //      OnPlayerBorn?.Invoke();
            //}
            public override void Init(string skinName)
            {
                  
                  skinName = ("arms_" + skinName).ToLower();
                  base.Init(skinName);
                  
            }
            public void Init(string skinName,out GunItem gun)
            {
                  skinName = ("arms_" + skinName).ToLower();
                  base.Init(skinName);
                  gun = skin.GetComponent<GunItem>();
            }
            public override void OnMsgDamage(int damage, string id)
            {
                  if (Hp < 0)
                  { return; }
                  Debug.Log("enemy " + damage);
                  damage -= Defence;
                  MsgHitPlayer msg = new MsgHitPlayer();
                  msg.id = id;
                  msg.hitId = this.id;
                  msg.damage = damage;
                  NetManager.Send(msg);
            }
            private void ContectCheck()
            {
                  //发射球形射线
                  //通过球型射线获取给定半径的所有碰撞器
                  var colliders = Physics.OverlapSphere(transform.position, checkRadius);
                  if (colliders == null || colliders.Length == 0)
                  {
                        OnPlayerCheck?.Invoke(null, null);
                        return;
                  }
                  var obj = ArrayHelper.Find(colliders, a => a.GetComponent<Iinteraction>() != null);
                  if (obj == null)
                  {
                        OnPlayerCheck?.Invoke(null, null);
                        return;
                  }
                  if (obj.GetComponent<Iinteraction>().BeCheck == false)
                  {
                        OnPlayerCheck?.Invoke(null, null); return;
                  }
                  OnPlayerCheck?.Invoke(obj.GetComponent<Iinteraction>(), gameObject);
            }
            public override void OnReBorn(int value)
            {
                  base.OnReBorn(value);
                  OnPlayerBorn?.Invoke(this);
            }
      }
}