using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.item;
using FPS.Character;
public class SyncWeapenMana:BaseWeapenMana
{
      private Animator animator;//
      private AudioSource audioSource;
      private void Start()
      {
            animator = GetComponentInChildren<Animator>();
            InitGun(currentWea);
      }
      public override void Init(string gunName)
      {
            animEvent = GetComponentInChildren<CharacterAnimEvent>();
            //绑定开火动画事件
            animEvent.fireHandle += AnimFire;
            animEvent.aimHandle += AnimAim;
            animEvent.cancleAimHandle += AnimOnCancleAim;
            animEvent.reloadHandle += AnimOnReload;
            GameObject go = ResourceManager.Load<GameObject>(gunName);
            currentWea = Instantiate(go).GetComponent<GunItem>();
            currentWea.id = GetComponent<BasePlayer>().id+ "DefauleGun";
      }
      public override void InitGun(GunItem gun)
      {
            //给枪指定人物
            gun.Owner = GetComponent<BasePlayer>();
            currentWea = gun;
            currentWea.GetComponent<Collider>().enabled = false;
            //更改动画类型
            string paraName = currentWea.gunData.paraName;
            float value = currentWea.gunData.animaType;
            animator.SetFloat(paraName, value);
            //设置动画速度
            animator.SetFloat("speed", currentWea.gunData.shotspeed);
            //切换音频源
            audioSource = currentWea.audioSource; 
      }
      public void OnAncPlayerBorn()
      {
            //更改动画类型
            string paraName = currentWea.gunData.paraName;
            float value = currentWea.gunData.animaType;
            animator.SetFloat(paraName, value);
            animator.SetFloat("speed", currentWea.gunData.shotspeed);
      }
      public override void Drop(GunItem gun)
      {
            if (gun == null)
            { return; }
            gun.GetComponent<Collider>().enabled = true;
            //初始化镜头
            if (gun.gunData.gunType != GunData.GunType.Sniper)
            {
                  foreach (var scope in gun.asynScops)
                  {
                        scope.gameObject.SetActive(false);
                  }
                  gun.baseAim.gameObject.SetActive(true);
            }
            gun.Owner = null;
      }
      public override void Fire(bool b)
      {
            animator.SetBool(currentWea.gunData.fireParam, b);
      }
      public void ChangeScope(string scopeName)
      {
            //查找物体
            Transform scope = Array.Find(currentWea.asynScops, a => a.gameObject.name == scopeName);
            if(scope==null)
            { return; }
            //如果是当前镜头
            if(scope == currentWea.currentAsynScope)
            {
                  //隐藏镜头显示默认镜头
                  scope.gameObject.SetActive(false);
                  currentWea.baseAim.gameObject.SetActive(true);
                  return;
            }
            if(currentWea.currentAsynScope!=null)
            { currentWea.currentAsynScope.gameObject.SetActive(false); }//隐藏上一次的镜头
            //隐藏基本镜头
            currentWea.baseAim.gameObject.SetActive(false);
            scope.gameObject.SetActive(true);
            currentWea.currentAsynScope = scope;
      }
      public override void Aim()
      {
            
      }
/// <summary>
/// 换弹
/// </summary>
/// <param name="b"></param>
      public  void Reload(bool b)
      {
            //播放动画
            animator.SetBool("reload", b);
            //播放声音
            if(b == false)
            {
                  audioSource.Stop();
                  return;
            }
            audioSource.clip = currentWea.reloadClip;
            audioSource.Play();
      }
      public override void AnimFire()
      {
            audioSource.clip = currentWea.shotClip;
            audioSource.Play();
            if(currentWea.CurrentModl == GunData.ShotModl.单发)
            {
                  animator.SetBool("fire", false);
            }
      }
      public override void AnimOnReload()
      {
           
      }

      public override void AnimOnCancleAim()
      {
           
      }

      public override void AnimAim()
      {
            
      }
      public void ChangeGun(GunItem gun)
      {
            Drop(currentWea);
            InitGun(gun);
      }
}

