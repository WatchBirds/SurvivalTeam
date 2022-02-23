using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace FPS.item
{
      public class ShotGunItem : GunItem
      {
            public Transform[] bullletVictor;
            protected override void 自动Shoot()
            {
                  for (int i= 0; i < bullletVictor.Length;i++)
                  {
                        Vector3 direction = bullletVictor[i].forward;
                        GameObject go = GameObjectPool.instance.CreatObject(bullet.name, bullet, bullPos.position, Quaternion.identity);
                        go.GetComponent<Bullet>().Init(direction, buData.speed, buData.maxdistance, damage);
                  }
                  lefeBullet--;
                  if (lefeBullet <= 0)
                  {
                        animator.SetBool(gunData.fireParam, false);
                        animator.SetBool(gunData.aimParam, false);
                        //发送射击协议
                        MsgFire msg = new MsgFire();
                        msg.state = 1;
                        NetManager.Send(msg);
                  }
            }
            public override void Reload()
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
      }
}

