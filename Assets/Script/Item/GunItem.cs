using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
using System;
using ShotModl = FPS.item.GunData.ShotModl;
using System.Reflection;
using Fps.Item;

namespace FPS.item
{
      public class GunItem : BaseItem
      {
            protected delegate void ShotHandle();

            //是否已加载资源
            private bool IFLoad =false;            
            [Tooltip("弹夹剩余子弹")]
            public int lefeBullet;
            [Tooltip("弹夹最大子弹数")]
            public int magBullet = 30;
            [Tooltip("剩余所有子弹")]
            public int allBullet;

             //当前射击模式
            public ShotModl CurrentModl;
            //枪信息
            public GunData gunData;
            //子弹信息
            public BulletData buData;
            //弹夹预制件
            public GameObject mag; 
           //子弹预制体
            public GameObject bullet;
            //子弹生成位置
            public Transform bullPos;
            public Camera gunCamera;
            [HideInInspector]//当前瞄准时使用的枪相机视野
            public float useGunCamerView;
            [HideInInspector]//当前瞄准时使用的主相机视野
            public float useMainCamerView;
            [HideInInspector]//当前瞄准时枪相机模式
            public bool useorthographic;
            //当前使用的镜片
            [HideInInspector]
            public ScopeItem curentScope;
            [Tooltip("枪的基本瞄准器")]
            public Transform baseAim;
            public bool state = false;
            
            protected Dictionary<ShotModl, MethodInfo> shots = new Dictionary<ShotModl, MethodInfo>();
            protected CharacherAnimator animator;

            public event Action OnFireStop;
            public int damage;
            [Header("同步玩家需要的参数")]
            public Transform[] asynScops;
            public Transform currentAsynScope;
            [Header("音频")]
            public AudioSource audioSource;
            public AudioClip shotClip;
            public AudioClip reloadClip;
            public AudioClip reloadAllClip;
            public AudioClip reloadCloseClip;
            public AudioClip aiminClip;
            public AudioClip cashClip;
            public float cameroffset;

            public float Force = 10;

            private void Start()
            {
                  //添加回调
                  Array array = Enum.GetValues(typeof(ShotModl));
                  Type type = GetType();
                  foreach (var temp in array)
                  {
                        if ((gunData.modls & (ShotModl)temp) != 0)//如果包含
                        {
                              string methodName = (temp.ToString() + "Shoot").ToString();
                              MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                              shots.Add((ShotModl)temp, method);
                        }
                  }
            }
            /// <summary>
            /// 开火
            /// </summary>
            public void GunFire()
            {
                  shots[CurrentModl].Invoke(this,null);
            }
            //自动模式
            protected virtual void 自动Shoot()
            {
                  //创建子弹
                  GameObject go = GameObjectPool.instance.CreatObject(bullet.name, bullet, bullPos.position, Quaternion.identity);
                  Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
                  Vector3 direction;
                  RaycastHit hit;
                  if (Physics.Raycast(ray, out hit, buData.maxdistance, LayerMask.GetMask("Build", "BulletHit")))
                  {
                        direction = hit.point - bullPos.position;
                  }
                  else
                  {
                        direction = bullPos.forward;
                  }
                  go.transform.forward = bullPos.forward;

                  //   初始化子弹
                  go.GetComponent<Bullet>().Init(direction, buData.speed, buData.maxdistance, damage);
                  lefeBullet--;
                  if (lefeBullet <= 0)
                  {
                        animator.SetBool(gunData.fireParam, false);
                        animator.SetBool(gunData.aimParam, false);
                        OnFireStop?.Invoke();
                        //发送射击协议
                        MsgFire msg = new MsgFire();
                        msg.state = 1;
                        NetManager.Send(msg);
                  }
                  //GameObject bullte = GameObjectPool.instance.CreatObject(bullet.name, bullet, bullPos.position, bullPos.rotation);
                  //bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * Force, ForceMode.Impulse);
            }
            //单发模式
            protected void 单发Shoot()
            {
                  自动Shoot();
                  animator.SetBool(gunData.fireParam, false);
                  OnFireStop?.Invoke();
            }
            /// <summary>
            /// 换弹
            /// </summary>
            public virtual void Reload()
            {     if (gunData.gunType == GunData.GunType.Sniper)
                  {
                        if (lefeBullet < magBullet && allBullet > 0)
                        {
                              audioSource.clip = reloadAllClip;
                              lefeBullet += 1;
                              allBullet -= 1;
                              audioSource.Play();
                        }
                        else
                        {
                              animator.SetBool("reload", false);
                              audioSource.clip = reloadCloseClip;
                              audioSource.Play();
                        }
                  }
                  else
                  {
                        int value = Mathf.Clamp(allBullet, 0, magBullet);
                        allBullet -= value;
                        lefeBullet = value;
                  }
            }
            //指定拥有者时初始化枪
            protected override void Init()
            {  
                  animator = Owner.GetComponent<CharacherAnimator>();
                  //将枪绑定角色
                  Transform pos = TransformHelper.FindChiled(Owner.transform, gunData.handPos);
                  if(pos == null)
                  { pos = Owner.transform; }
                  transform.SetParent(pos);
                  transform.localPosition = Vector3.zero;
                  transform.localEulerAngles = Vector3.zero;

                  if (IFLoad == true)
                        return;
                
                  IFLoad = true;
            }
            /// <summary>
            /// 当丢弃的时候
            /// </summary>
            protected override void OnDrop()
            {
                  Ray ray = new Ray(transform.position, Vector3.down);
                  transform.SetParent(null);
                  if( Physics.Raycast(ray, out RaycastHit hit, 10,LayerMask.GetMask("Build")))
                  {
                        transform.localPosition = hit.point;
                        transform.localEulerAngles = Vector3.zero;
                  }
            }
      }
}