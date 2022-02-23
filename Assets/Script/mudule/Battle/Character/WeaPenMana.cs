using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.item;
using FPS.Character;
using AI.Perception;
using System;

public class WeaPenMana : BaseWeapenMana
{
      #region 版本一
      public delegate void Handle(GunItem gun);
      public static event Handle OnFire;//当开火
      public static event Handle OnCancleFire;//当取消开火
      public static event Handle OnAim;//当瞄准
      public static event Handle OnCancleAim;//当取消瞄准
      public static event Action<GunItem> OnEnter;//当有物体进入
      public static event Action<GunItem> OnExit;//当有物体离开
      public static event Handle OnDrop;//当丢弃
      public static event Handle OnInit;//当初始化枪
      public static event Handle OnReload;//当换弹
      public static event Action<string> OnChangScope;//当换镜片时
      public static event Action<int> OnGetBullet;
      public GunItem myMoldgun;
      private CtrlPlayer ctrlPlayer ;
      private CharacherAnimator animator;
      //角色周围枪的列表
      private Dictionary<string, GunItem> guns = new Dictionary<string, GunItem>();
      public AudioSource audioSource;
      private Transform maincamer;

      private delegate void ReloadHandle();
      private Dictionary<GunData.GunType, ReloadHandle> reloadHandles;
      private float checkRadius;
      private static event Action<GunItem[]> OnRayCheck;
      private  void Start()
      {
            maincamer = Camera.main.transform;
            animator = GetComponent<CharacherAnimator>();
            InitGun(currentWea);
            //输入控制绑定事件
            InputControl.OnFirebtdown += Fire;
            InputControl.OnAimbtdown += Aim;
            InputControl.OnClickWeapenIcon += InputControl_OnClickWeapenIcon;
            InputControl.OnRelaod += Reload;
            BattleManager.instance.OnReciveChangetGun += Instance_OnReciveChangetGun;
            InputControl.OnClickScopeICon += InputControl_OnClickScopeICon;
            BattleManager.instance.OnReciveChangeScope += Instance_OnReciveChangeScope;
            CtrlPlayer.OnPlayerDie += CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn += CtrlPlayer_OnPlayerBorn;

      }


      private void OnDestroy()
      {
            InputControl.OnFirebtdown -= Fire;
            InputControl.OnAimbtdown -= Aim;
            InputControl.OnClickWeapenIcon -= InputControl_OnClickWeapenIcon;
            InputControl.OnRelaod -= Reload;
            BattleManager.instance.OnReciveChangetGun -= Instance_OnReciveChangetGun;
            InputControl.OnClickScopeICon -= InputControl_OnClickScopeICon;
            BattleManager.instance.OnReciveChangeScope -= Instance_OnReciveChangeScope;
            CtrlPlayer.OnPlayerDie -= CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn -= CtrlPlayer_OnPlayerBorn;

      }
      public override void Init(string gunName)
      {
            ctrlPlayer = GetComponent<CtrlPlayer>();
            GameObject go = ResourceManager.Load<GameObject>(gunName);
            currentWea = Instantiate(go).GetComponent<GunItem>();
            currentWea.id = GameMain.id+"DefauleGun";
      }
      public void GetBullet(int getNumber)
      {
            myMoldgun.allBullet += getNumber;
            OnGetBullet?.Invoke(myMoldgun.allBullet);
      }
      public override void Fire(bool b)
      {
            if (myMoldgun != null && myMoldgun.lefeBullet > 0)
            {
                  animator.SetBool(currentWea.gunData.fireParam, b);
                  MsgFire msg = new MsgFire();
                  msg.state = (!b).GetHashCode();
                  NetManager.Send(msg);
            }
            else if (myMoldgun != null && myMoldgun.lefeBullet <= 0)//开枪状态—无子弹
            {

                  animator.SetBool(myMoldgun.gunData.fireParam, false);
                  animator.SetBool(myMoldgun.gunData.aimParam, false);
                  //发送射击协议
                  MsgFire msg = new MsgFire();
                  msg.state = 1;
                  NetManager.Send(msg);

                  audioSource.clip = myMoldgun.cashClip;
                  audioSource.Play();
            }
            if(myMoldgun != null && myMoldgun.lefeBullet <= 0&&myMoldgun.allBullet>0)
            {
                  Reload();
            }
      }
      public override void Aim()
      {
            //瞄准时取消换弹
            animator.SetBool("reload", false);
            //发送换弹协议
            MsgReload msg = new MsgReload();
            msg.state = 1;
            NetManager.Send(msg);
            //MsgMotionBool msg = new MsgMotionBool();
            //msg.paraName = "reload";
            //msg.result = 1;
            //NetManager.Send(msg);
            if (!animator.GetBool(myMoldgun.gunData.aimParam))
            {
                  animator.SetBool(myMoldgun.gunData.aimParam, true);
            }
            else
            {
                  animator.SetBool(myMoldgun.gunData.aimParam, false);
            }
      }
      public void Reload()
      {
            AnimatorStateInfo stateInfo = animator.animator.GetCurrentAnimatorStateInfo(0);
            if (myMoldgun.allBullet > 0 && !stateInfo.IsName("AimIn"))
            {
                  if (myMoldgun.gunData.gunType == GunData.GunType.Sniper)
                  {
                        animator.SetBool("reload", true);
                        audioSource.clip = myMoldgun.reloadClip;
                  }
                  else
                  {

                        float value = Mathf.Clamp(myMoldgun.lefeBullet, 0, 1);
                        animator.SetFloat("reloadType", value);
                        animator.SetBool("reload", true);
                        //播放音频
                        if (value < 0.1)
                        {
                              audioSource.clip = myMoldgun.reloadAllClip;
                        }
                        else
                        {
                              audioSource.clip = myMoldgun.reloadClip;
                        }
                  }
                  audioSource.Play();

                  //发送换弹协议
                  MsgReload msg = new MsgReload();
                  msg.state = 0;
                  NetManager.Send(msg);
            }
      }
      public override void Drop(GunItem gun)
      {
            if (gun == null)
            { return; }
            Camera.main.transform.SetParent(null);
            //枪处理
            myMoldgun.OnFireStop -= CancleFire;
            gun.Owner = null;
            gun.gameObject.SetActive(true);
            gun.GetComponent<Collider>().enabled = true;
            OnDrop?.Invoke(myMoldgun);
            gun.lefeBullet = myMoldgun.lefeBullet;
            gun.allBullet = myMoldgun.allBullet;
            //模型处理
            Destroy(myMoldgun.gameObject);
      }
      
      /// <summary>
      /// 初始化枪 绑定动画。。。。
      /// </summary>
      /// <param name="gun"></param>
      public override void InitGun(GunItem gun)
      {
            gun.Owner = GetComponent<BasePlayer>();
            //隐藏枪
            gun.gameObject.SetActive(false);
            gun.GetComponent<Collider>().enabled = false;
            currentWea = gun;
            //加载对应模型
            ctrlPlayer.Init(gun.gunData.prefabNa, out myMoldgun);
            myMoldgun.Owner = GetComponent<BasePlayer>();
             myMoldgun.lefeBullet =gun.lefeBullet;
            myMoldgun.allBullet =gun.allBullet;
            myMoldgun.useGunCamerView = myMoldgun.gunData.aimGunCamerView;
            myMoldgun.useMainCamerView = myMoldgun.gunData.aimMainCamerView;
            OnInit?.Invoke(myMoldgun);
            myMoldgun.gunCamera.fieldOfView = myMoldgun.gunData.defalueGunCamerView;
            animEvent = myMoldgun.GetComponentInChildren<CharacterAnimEvent>();
            myMoldgun.OnFireStop += CancleFire;
            //绑定动画事件
            animEvent.fireHandle += AnimFire;
            animEvent.aimHandle += AnimAim;
            animEvent.cancleAimHandle += AnimOnCancleAim;
            animEvent.reloadHandle += AnimOnReload;
            //设置动画速度
            animator.SetFloat("shotSpeed", myMoldgun.gunData.shotspeed);
            maincamer.parent = TransformHelper.FindChiled(myMoldgun.transform, "camera");
            maincamer.localPosition = Vector3.zero;
            maincamer.localEulerAngles = new Vector3(90, 0, 0);
            audioSource = myMoldgun.audioSource;
      }
      public override void AnimFire()
      {
            myMoldgun.GunFire();

            //播放音频
            audioSource.clip = myMoldgun.shotClip;
            audioSource.Play();
            OnFire?.Invoke(myMoldgun);
      }
      public override void AnimAim()
      {
            //播放音频
            audioSource.clip = myMoldgun.aiminClip;
            audioSource.Play();
            OnAim?.Invoke(myMoldgun);
      }
      public override void AnimOnCancleAim()
      {
            OnCancleAim?.Invoke(myMoldgun);
      }
      public override void AnimOnReload()
      {
            myMoldgun.Reload();
            OnReload?.Invoke(myMoldgun);
      }
      //当玩家复活事件监听
      private void CtrlPlayer_OnPlayerBorn(CtrlPlayer ctrlPlayer)
      {
            myMoldgun.gunCamera.fieldOfView = myMoldgun.gunData.defalueGunCamerView;
           // StartCoroutine(GunCamerDelayShow(ctrlPlayer));
            OnCancleAim(myMoldgun);
            animator.SetFloat("shotSpeed", myMoldgun.gunData.shotspeed);
            if (myMoldgun.curentScope == null)
            { animator.SetFloat("scopeType", 0); }
            else
            {
                  animator.SetFloat("scopeType", myMoldgun.curentScope.value);
            }
      }
      //IEnumerator GunCamerDelayShow(CtrlPlayer player)
      //{
      //      while (!player.rreBornFin)
      //      {
      //            yield return null;
      //      }
      //      myMoldgun.gunCamera.enabled = true;
      //}
      //当玩家死亡事件监听
      private void CtrlPlayer_OnPlayerDie()
      {
            StopAllCoroutines();
            myMoldgun.gunCamera.enabled = false;
      }
      //点击镜片事件监听
      private void InputControl_OnClickScopeICon(string scopeName)
      { //发送协议
            MsgChangeScope msg = new MsgChangeScope();
            msg.scopeName = scopeName;
            NetManager.Send(msg);
      }
      //收到换镜片事件监听
      private void Instance_OnReciveChangeScope(string scopeName)
      {
            //查找物体
            Transform scopeTrans = TransformHelper.FindChiled(myMoldgun.transform, scopeName);
            //获取组件
            ScopeItem scope = scopeTrans.GetComponent<ScopeItem>();
            if (scope == null)
            {
                  return;
            }
            //取消瞄准
            animator.SetBool(myMoldgun.gunData.aimParam, false);
            //如果和当前镜片相同
            if (myMoldgun.curentScope == scope)
            {
                  //隐藏镜片
                  myMoldgun.curentScope.Hiding();
                  myMoldgun.curentScope = null;
                  //显示基本瞄准器
                  myMoldgun.baseAim.gameObject.SetActive(true);
                  //改变视野
                  myMoldgun.useGunCamerView = myMoldgun.gunData.aimGunCamerView;
                  myMoldgun.useMainCamerView = myMoldgun.gunData.aimMainCamerView;
                  OnChangScope?.Invoke(scopeTrans.name);
                  animator.SetFloat("scopeType", 0);
                  return;
            }
            //显示镜片
            if (myMoldgun.curentScope != null)
            { myMoldgun.curentScope.Hiding(); }
            //隐藏基本瞄准器
            myMoldgun.baseAim.gameObject.SetActive(false);
            scope.Show();
            //改变视野
            myMoldgun.curentScope = scope;
            myMoldgun.useGunCamerView = scope.aimGunCamerView;
            myMoldgun.useMainCamerView = scope.aimMainCamerView;
            OnChangScope?.Invoke(scopeTrans.name);
            animator.SetFloat("scopeType", scope.value);
      }
      //点击武器图片事件监听
      private void InputControl_OnClickWeapenIcon(string gunid)
      {
            if (guns.ContainsKey(gunid))
            {
                  if(guns[gunid].Owner!=null)
                  { return; }
                  //发送换枪协议
                  MsgChangeGun msg = new MsgChangeGun();
                  msg.gunid = gunid;
                  NetManager.Send(msg);
            }
      }
      //收到换枪协议事件监听
      private void Instance_OnReciveChangetGun(string name)
      {
            if (!guns.ContainsKey(name))
            { return; }
                  GunItem gun = guns[name];
            guns.Remove(name);
            Drop(currentWea);
            InitGun(gun);
      }
      private void CancleFire()
      {
            OnCancleFire?.Invoke(myMoldgun);
      }
      #region 物体检测部分
      private void OnTriggerEnter(Collider other)
      {
            GunItem temp = other.GetComponent<GunItem>();
            if (other.tag == "item" && temp != null)
            {
                  guns.Add(temp.id, temp);
                  OnEnter?.Invoke(temp);
            }
      }
      private void OnTriggerExit(Collider other)
      {
            GunItem temp = other.GetComponent<GunItem>();
            if (other.tag == "item" && temp != null)
            {
                  guns.Remove(temp.id);
                  OnExit?.Invoke(temp);
            }
      }
      //private void ContectCheck()
      //{
      //      //发射球形射线
      //      //通过球型射线获取给定半径的所有碰撞器
      //      var colliders = Physics.OverlapSphere(transform.position, checkRadius,LayerMask.GetMask("Gun"));
      //      if (colliders == null || colliders.Length == 0)
      //      {
      //            this.guns.Clear();
      //            OnRayCheck?.Invoke(null);
      //            return;
      //      }
      //      var obj = ArrayHelper.FindAll(colliders, a => a.GetComponent<GunItem>() != null);
      //      if (obj == null)
      //      {
      //            this.guns.Clear();
      //            OnRayCheck?.Invoke(null);
      //            return;
      //      }
      //      GunItem[] guns = ArrayHelper.Select(obj, a => a.GetComponent<GunItem>());
      //      this.guns.Clear();
      //      for(int i =0;i<guns.Length;i++)
      //      {
      //            this.guns.Add(guns[i].id, guns[i]);
      //      }
      //      OnRayCheck?.Invoke(this.guns.Values);
      //}
      #endregion

      #endregion

}
