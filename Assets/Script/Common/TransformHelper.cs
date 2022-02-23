using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper : MonoBehaviour
{
      public static Transform FindChiled(Transform chiledparent, string childname)
      {
            Transform chiledTF = chiledparent.Find(childname);
            if (chiledTF != null)
                  return chiledTF;
            for (int i = 0; i < chiledparent.childCount; i++)
            {
                  chiledTF = FindChiled(chiledparent.GetChild(i), childname);
                  if (chiledTF != null)
                        return chiledTF;
            }
            return null;
}
    /// <summary>
    /// 转向
    /// </summary>
    /// <param name="target">转向的向量</param>
    /// <param name="transform">自身transform组件</param>
    /// <param name="rotationSpeed">转向速度</param>
    public static void LookAtTarget(Vector3 target,Transform transform,float rotationSpeed)
    {
        if (target != Vector3.zero)
        {
            Quaternion dir = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.Lerp(transform.rotation, dir, rotationSpeed);
        }
    }
}
