using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoSingleton<GameObjectPool>
{
      private GameObjectPool() { }

      private Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();
      /// <summary>如果对象池已有当前对象则返回池中对象，否者创建一个对象返回并存入池中 </summary>
      public GameObject CreatObject(string key, GameObject go, Vector3 position, Quaternion quaternion)
      {
            GameObject temoGo = FindUsable(key);
            if (temoGo != null)
            {
                  temoGo.transform.position = position;
                  temoGo.transform.rotation = quaternion;
                  temoGo.SetActive(true);
            }
            else
            {
                  temoGo = Instantiate(go, position, quaternion);
                  Add(key, temoGo);
            }
            temoGo.transform.SetParent(transform);
            return temoGo;
      }
      private GameObject FindUsable(string key)
      {
            if (cache.ContainsKey(key))
            {
                  return cache[key].Find(a => !a.activeSelf);
            }
            return null;
      }
      private void Add(string key, GameObject go)
      {
            if (!cache.ContainsKey(key))
            {
                  cache.Add(key, new List<GameObject>());
            }
            cache[key].Add(go);
      }
      /// <summary>
      /// 清除对应键的所有对象
      /// </summary>
      /// <param name="key"></param>
      public void Clear(string key)
      {
            if (cache.ContainsKey(key))
            {
                  foreach (var it in cache[key])
                  { Destroy(it); }
                  cache.Remove(key);
            }
      }
      /// <summary>
      /// 对象池清空
      /// </summary>
      public void ClearAll()
      {
            List<string> keys = new List<string>(cache.Keys);
            foreach (var it in keys)
            {
                  Clear(it);
            }
      }
      /// <summary>
      /// 回收对象
      /// </summary>
      /// <param name="go"></param>
      public void CollectObject(GameObject go)
      {
            go.gameObject.SetActive(false);
      }
      /// <summary>
      /// 延迟回收
      /// </summary>
      /// <param name="go"></param>
      /// <param name="delay">延迟时间</param>
      public void CollectObject(GameObject go, float delay)
      {
            StartCoroutine(CollectDelay(go,delay));   
      }
      private IEnumerator CollectDelay(GameObject go,float delay)
      {
            yield return new WaitForSeconds(delay);
            go.gameObject.SetActive(false);
      }

}
