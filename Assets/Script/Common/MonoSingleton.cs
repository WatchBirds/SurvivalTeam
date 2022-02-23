/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>{
	
	protected static T m_Instance = null;
 
    /// <summary>获取单例对象/// </summary>
	public static T instance{
        get{
                  if (m_Instance == null)
                  {
                        m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                        if (m_Instance == null)
                        {
                              m_Instance = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                              m_Instance.Init();
                        }

                  }
            return m_Instance;
        }
    }

    private void Awake(){
   
        if( m_Instance == null ){
            m_Instance = this as T;
        }
    }
 /// <summary>可初始化 /// </summary>
    public virtual void Init(){}
 
    //程序退出时
    private void OnApplicationQuit(){
      // m_Instance = null;
    }
}