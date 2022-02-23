using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]//"玩家bool类型动画参数名称" 
public class BoolParameter
{
      public string idle = "idle";
      public string move = "move";
}
[Serializable]
public class FloatParameter
{
      public string forward = "forward";
      public string right = "right";
      public string hand = "hand";
}

