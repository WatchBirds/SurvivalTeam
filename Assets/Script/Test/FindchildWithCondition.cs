using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindchildWithCondition
{
      public delegate bool Condition(Transform tra);
      private static bool haveFound;
      public static FindchildWithCondition Instance { get
            {
                  if (_instance == null)
                  { _instance = new FindchildWithCondition(); }
                  haveFound = false;
                  return _instance;
            } }
      private static FindchildWithCondition _instance;
      public  Transform FindChild(Transform parent, string childName, Condition condition)
      {
            if(haveFound)
            { return null; }
            for (int i = 0; i < parent.childCount; i++)
            {
                  Transform child = parent.GetChild(i);
                  if (child.name == childName && condition(child))
                  {
                        return child;
                  }
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                  Transform child = parent.GetChild(i);
                  Transform target = FindChild(child, childName, condition);
                  if (target != null)
                  {
                        return target;
                  }
            }
            return null;
      }
}
