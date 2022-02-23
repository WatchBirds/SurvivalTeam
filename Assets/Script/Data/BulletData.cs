using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace FPS.item
{
      [Serializable]
      public class BulletData
      {
            [Tooltip("每秒移动距离")]
            public float speed;
            [Tooltip("最大移动距离")]
            public float maxdistance = 200;
      }
}
