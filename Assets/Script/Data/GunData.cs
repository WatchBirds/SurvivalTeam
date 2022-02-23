using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace FPS.item
{
      [Serializable]
      public class GunData
      {
            public enum GunType
            {
                  BigGun,
                  HandGun,
                  Sniper
            }
            [Flags]
            public enum ShotModl
            {
                  自动 = 1,
                  单发 = 2
            }
            [Tooltip("枪类型")]
            public GunType gunType;
            [Tooltip("枪的预制体名称")]
            public string prefabNa;
            [Range(0, 1)]
            [Tooltip("动画类型的值")]
            public float animaType;
            [Tooltip("动画类型参数名称")]
            public string paraName = "animaType";
            [Tooltip("开火动画参数")]
            public string fireParam;
            [Tooltip("瞄准动画参数")]
            public string aimParam;
            [Tooltip("换弹动画参数")]
            public string reloadParam;
            [Tooltip("该物体在角色的挂载点")]
            public string handPos;
            [Tooltip("射击速度")]
            public float shotspeed = 5;
            [Tooltip("枪支持的射击模式")]
            public ShotModl modls;
            [Tooltip("瞄准时人物移动速度衰减百分比")]
            public float aimPlayerSpeed = 0.5f;
            [Tooltip("对应图片名称")]
            public string iconName;
            [Tooltip("瞄准时枪相机视野")]
            public float aimGunCamerView;
            [Tooltip("瞄准时主相机视野")]
            public float aimMainCamerView ;
            [Tooltip("不瞄准枪相机的视野")]
            public float defalueGunCamerView;
            [Tooltip("不瞄准下主相机的视野")]
            public float defalueMainCamerView;
            [Tooltip("枪相机的投射模式（默认为正交）")]
            public bool orthographic = true;
      }
}
