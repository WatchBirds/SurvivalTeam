using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fps.Item
{
      public  class BulletV2:MonoBehaviour
      {
            private float speed;//速度
            [Tooltip("对应特效")]
            public GameObject[] FX;
            [Tooltip("攻击目标标签")]
            public string[] tags;
            private int damage;//伤害
            private float maxDistace;//最大距离

            private void OnTriggerEnter(Collider other)
            {
                  Debug.Log(other.gameObject.name);
            }
      }
}
