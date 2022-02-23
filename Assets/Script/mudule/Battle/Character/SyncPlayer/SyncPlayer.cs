using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.item;
namespace FPS.Character
{
      /// <summary>
      /// 同步玩家
      /// </summary>
      public class SyncPlayer : BasePlayer
      {
            private Animator animator;
            private Vector3 targetPos;//目标位置
            private Vector3 targetEul;//目标旋转角度
            private SyncWeapenMana mana;
            public event Action<Vector3> OnSyncMove;//同步玩家移动事件
            public event Action OnSyncDie;//同步玩家死亡事件
            public event Action OnSyncReborn;//同步玩家复活事件
            public event Action<string> OnSyncLeaveBattle;//同步玩家离开战场事件
            private Transform headPos;//头顶信息位置
            [Header("音频")]
            public AudioSource audioSource;
            public AudioClip walkClip;
            public AudioClip runClip;
            public override void Init(string skinName)
            {
                  base.Init(skinName);
                  headPos = TransformHelper.FindChiled(transform, "UiPos");
                  skin.transform.SetParent(transform);
                  skin.transform.localPosition = Vector3.zero;
                  skin.transform.localEulerAngles = Vector3.zero;
                  animator = GetComponentInChildren<Animator>();
                  mana = GetComponent<SyncWeapenMana>();

            }
            private void OnDestroy()
            {
                  OnSyncMove = null;
            }
            /// <summary>
            /// 同步位置
            /// </summary>
            /// <param name="msg"></param>
            public void SyncPos(MsgSyncPlayer msg)
            {
                  //更新位置和旋转
                  targetPos = new Vector3(msg.x, msg.y, msg.z);
                  targetEul = new Vector3(msg.ex, msg.ey, msg.ez);
                  //播放动画
                  animator.SetFloat("forward", msg.foward);
                  animator.SetFloat("right", msg.right);
                  animator.SetFloat("hand", msg.hand);
                  if (Mathf.Abs(msg.foward) <= 0.1 || Mathf.Abs(msg.right) <= 0.1)//停止状态
                  {
                        audioSource.Stop();
                  }                                                                 //播放音频
                  else if (msg.foward < 0 || (msg.right * msg.right + msg.foward * msg.foward) < 0.81)//步行状态
                  {
                        audioSource.clip = walkClip;
                        if (!audioSource.isPlaying)
                        {
                              audioSource.Play();
                        }
                  }

                  else//跑状态
                  {
                        audioSource.clip = runClip;
                        if (!audioSource.isPlaying)
                        {
                              audioSource.Play();
                        }
                  }
            }
            private Vector3 current;
            private Vector3 current2;

            public void FixedUpdate()
            {
                  transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref current, 0.1f);
                  transform.forward = Vector3.SmoothDamp(transform.forward, targetEul, ref current2, 0.1f);
                  OnSyncMove?.Invoke(headPos.position);
            }

            public void Jump()
            {

            }
            public void ChangeGunMold(MsgChangeMold msg)
            {
                  Debug.Log(msg.mold);
                  mana.currentWea.CurrentModl = (GunData.ShotModl)Enum.Parse(typeof(GunData.ShotModl), msg.mold);
            }
            /// <summary>
            /// 同步玩家开火
            /// </summary>
            /// <param name="msg"></param>
            public void Fire(MsgFire msg)
            {
                  bool b;
                  if (msg.state == 0)
                  {
                        b = true;
                  }
                  else
                  {
                        b = false;
                  }
                  mana.Fire(b);
            }
            /// <summary>
            /// 同步玩家换弹
            /// </summary>
            /// <param name="msg"></param>
            public void Reload(MsgReload msg)
            {
                  bool b;
                  if (msg.state == 0)
                  {
                        b = true;
                  }
                  else
                  {
                        b = false;
                  }
                  mana.Reload(b);
            }
            /// <summary>
            /// 同步玩家离开
            /// </summary>
            public void LeaveBattle()
            {
                  OnSyncLeaveBattle?.Invoke(id);
            }
            /// <summary>
            /// 同步玩家死亡
            /// </summary>
            public override void Dead()
            {
                  OnSyncDie?.Invoke();
            }
            /// <summary>
            /// 更换枪
            /// </summary>
            /// <param name="gun"></param>
            public void ChangeGun(GunItem gun)
            {
                  mana.ChangeGun(gun);
            }
            /// <summary>
            /// 更换镜片
            /// </summary>
            public void ChangeScope(string scopeName)
            {
                  mana.ChangeScope(scopeName);
            }
            public override void OnReBorn(int value)
            {
                  base.OnReBorn(value);
                  animator.SetFloat("right", 0);
                  animator.SetFloat("forward", 0);
                  OnSyncReborn?.Invoke();
                  mana.OnAncPlayerBorn();
            }
      }
      
}

