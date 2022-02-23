using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerGroupManager : MonoSingleton<TrigerGroupManager>
{
      public Dictionary<int, List<Transform>> allTriget = new Dictionary<int, List<Transform>>();

      public override void Init()
      {
            TransTrigger[] trans = GameObject.FindObjectsOfType<TransTrigger>();
            foreach(var tran in trans)
            {
                  Add(tran);
            }
      }
      /// <summary>
      /// 当tigger启用时调用
      /// </summary>
      public void Check(TransTrigger trigger)
      {
            //把和tigger所在的其它物体禁用
            foreach (var tgr in allTriget[trigger.group])
            {
                  if(trigger!=tgr&&tgr.gameObject.activeSelf == true)
                  {
                        tgr.gameObject.SetActive(false);
                  }
            }
      }
      public void Add(TransTrigger trigger)
      {
            if (!allTriget.ContainsKey(trigger.group))
            {
                  allTriget.Add(trigger.group, new List<Transform>());
            }
            if (!allTriget[trigger.group].Contains(trigger.transform))
            {
                  allTriget[trigger.group].Add(trigger.transform);
                  //把物体禁用
                  trigger.gameObject.SetActive(false);
            }
      }
}
