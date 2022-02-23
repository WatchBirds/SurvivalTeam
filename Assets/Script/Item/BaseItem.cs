using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.Character;
namespace FPS.item
{
      public enum ItemType
            {
                  Gun,
                  Grenade
            }
      public abstract class BaseItem : MonoBehaviour
      {    
            
            [Tooltip("物体id")]
            public string id;
            [Tooltip("物品类型")]
            public ItemType Type;
            public bool becheck = true;
            //该物品拥有者
            public BasePlayer Owner
            {
                  get { return owner; }
                  set
                  {
                        if (value != null)
                        { owner = value; Init(); return; }
                        owner = value;
                        OnDrop();
                  }
            }
            private BasePlayer owner;
            
            /// <summary>
            /// 指定拥有者时初始化物品
            /// </summary>
            protected abstract void Init();
            /// <summary>
            /// 指定拥有者为空时
            /// </summary>
            protected abstract void OnDrop();
      }
}
