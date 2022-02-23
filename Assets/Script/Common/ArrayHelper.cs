using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ArrayHelper
{
    #region delegate
    public delegate Tkey SelecHandle<T, Tkey>(T t);
      public delegate Tkey SelecHandle2<T2, Tkey>(T2 t2);
    public delegate bool FindHandled<T>(T t);
    #endregion
    /// <summary>对数组正序排序</summary>
    static public void OrderByo<T,Tkey>(T[] arry,SelecHandle<T,Tkey> handle)where Tkey:IComparable<Tkey>
    {
        var temp = default(T);
        for (int i = 0; i < arry.Length; i++)
        {
            for (int j = i + 1; j < arry.Length; j++)
            {
                if (handle(arry[i]).CompareTo(handle(arry[j]))>0)
                {
                    temp = arry[i];
                    arry[i] = arry[j];
                    arry[j] = temp;
                }
            }
        }
    }
    /// <summary>对数组反序排序</summary>
    static public void OrderByDescending<T,Tkey>(T[] arry,SelecHandle<T,Tkey> handle)where Tkey:IComparable<Tkey>
    {
        var temp = default(T);
        for (int i = 0; i < arry.Length; i++)
        {
            for (int j = i + 1; j < arry.Length; j++)
            {
                if (handle(arry[i]).CompareTo(handle(arry[j])) <0)
                {
                    temp = arry[i];
                    arry[i] = arry[j];
                    arry[j] = temp;
                }
            }
        }
    }
    /// <summary>比较数组中元素或对象的成员并返回最大元素或对象</summary>
    static public T Max<T,Tkey>(T[] arry,  SelecHandle<T, Tkey> handle)where Tkey: IComparable<Tkey>
    {
        var temp = default(T);
        temp = arry[0];
        for (int i = 0; i < arry.Length-1; i++)
        {
            if (handle(arry[i+1]).CompareTo(handle(temp))>0)
            {
                temp = arry[i+1];
            }
        }
        return temp;
    }
    /// <summary>比较数组中元素或对象的成员并返回最小元素或对象</summary>
    static public T Min<T, Tkey>(T[] arry, SelecHandle<T, Tkey> handle) where Tkey : IComparable<Tkey>
      {
        var temp = default(T);
        temp = arry[0];
        for (int i = 0; i < arry.Length - 1; i++)
        {
            if (handle(arry[i + 1]).CompareTo(handle(temp)) < 0)
            {
                temp = arry[i + 1];
            }
        }
        return temp;
    }
    /// <summary>查找并返回数组中满足条件的一个元素或对象</summary>
    static public T Find<T>(T[] arry, FindHandled<T> handle)
    {
        var temp = default(T);
        for (int i = 0;i<arry.Length;i++)
        {
            if (handle(arry[i]))
            {
                return arry[i];
            }      
        }
        return temp;
    }
      /// <summary>查找并返回数组中满足条件的所有元素或对象</summary>
      static public T[] FindAll<T>(T[] arry, FindHandled<T> handle)
    {
        List<T> temp = new List<T>();
        for (int i = 0; i < arry.Length; i++)
        {
            if (handle(arry[i]))
            {
                temp.Add(arry[i]);
            }
        }
        return temp.ToArray();
    }
    /// <summary>选择数组中对象的某些成员形成一个独立的数组</summary>
    static public Tkey[] Select<T,Tkey>(T[] arry,SelecHandle<T,Tkey> handle)
    {
        List<Tkey> tkeys = new List<Tkey>();
        foreach (var it in arry)
        {
            tkeys.Add(handle(it));
        }
        return tkeys.ToArray();
    }
      /// <summary>
      /// 返回满足条件的元素的下标如果没有返回-1
      /// </summary>
      public static int OutIndex<T>(T[]arry,FindHandled<T> handle)
      {
            for (int i = 0; i<arry.Length;i++)
            {
                  if (handle(arry[i]))
                  {
                        return i;
                  }
            }
            return -1;
      }
      public static T2[]ArrayCompare<T,Tkey,T2>(T[]arry1,T2[]arry2,SelecHandle<T,Tkey> handle,SelecHandle2<T2,Tkey> handle2) where Tkey : IComparable<Tkey>
      {
            List<T2> tempArry = new List<T2>();
            for(int i =0;i<arry1.Length;i++)
            {
                  for(int j = 0;j<arry2.Length;j++)
                  {
                        if(handle(arry1[i]).CompareTo(handle2(arry2[j]))!=0)
                        {
                              tempArry.Add(arry2[j]);
                        }
                  }
            }
            return tempArry.ToArray();
      }
      public static T EnumNext<T>( T t)where T:IComparable
      {
          Array valuses = Enum.GetValues(typeof(T));
            int idx = 0;
            int legth = valuses.Length;
            int nedidx = 0;
            foreach(var va in valuses)
            {
                  if(va.ToString() == t.ToString())
                  { 
                        idx = (idx + 1) % legth;
                        break;
                  }
                  else
                  { idx = (idx + 1) % legth; }
            }
            foreach (var va in valuses)
            {
                  if(idx == nedidx)
                  {
                        t = (T)Enum.Parse(typeof(T),va.ToString());
                        break;
                  }
                  else
                  { nedidx++; }
            }
            return t;
      }
}
