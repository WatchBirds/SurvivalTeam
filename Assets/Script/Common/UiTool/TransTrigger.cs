using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransTrigger : MonoBehaviour
{
      [Tooltip("所在的群")]
      public int group;

      public void Show()
      {
             TrigerGroupManager.instance.Check(this);
      }    
}
