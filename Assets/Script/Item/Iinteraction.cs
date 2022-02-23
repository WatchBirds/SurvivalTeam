using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 可交互
/// </summary>
public interface Iinteraction
{
      /// <summary>
      /// 交互文本
      /// </summary>
       string Str { get; }
      /// <summary>
      /// 是否参与交互
      /// </summary>
      bool BeCheck { get; set; }
      /// <summary>
      /// 使用
      /// </summary>
      /// <param name="go">使用的角色</param>
       void Use(GameObject go);
}

