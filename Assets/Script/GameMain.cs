using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;

public class GameMain : MonoSingleton<GameMain>
{
      public static string id = "";
      public static string sprite = "";
      public static bool done = false;
      public static string gunName;

      /// <summary>
      /// 玩家输入面板物体位置配置文件名称
      /// </summary>
      public static string inputConfigName = "/InputIconPositionConfig.txt";
      public static string inputConfigPath = Application.streamingAssetsPath + inputConfigName;
      private void Awake()
      {
            if (m_Instance != null)
            { Destroy(gameObject); }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
           
            if(!File.Exists(inputConfigPath))
            {
                  File.Create(inputConfigPath);
            }
#elif UNITY_ANDROID || UNITY_IPHONE
            //if(!File.Exists(Application.streamingAssetsPath + inputConfigName))
            //{
            //      File.Create(Application.streamingAssetsPath + inputConfigName);
            //}
            inputConfigPath = Application.persistentDataPath + inputConfigName;
            if (!File.Exists(inputConfigPath))
            {
             File.Create(inputConfigPath);
                 // StartCoroutine(CopyDataBase(Application.streamingAssetsPath + inputConfigName, inputConfigPath));
            }
#endif
      }
      IEnumerator PLayerGameTimeCheck()
      {
            yield return new WaitForSeconds(0.5f);
            if(PlayerPrefs.HasKey("gameTime"))
            {
                  float tm = PlayerPrefs.GetFloat("gameTime");
                  if((Time.time-tm)/360>2f)
                  {
                        PanelManager.Open<QuitPanel>("1");
                  }
            }
      }
      private void Start()
      {
            DontDestroyOnLoad(gameObject);
            //添加网络关闭监听
            NetManager.AddEventListener(NetManager.NetEvent.Close, OnconnectClose);
            //添加踢下线协议监听
            NetManager.AddMsgListener("MsgKick", OnMsgKick);
            NetManager.AddMsgListener("MsgAddFriend", OnMsgAddFriend);
            //初始化
            PanelManager.Init();
            BattleManager.instance.Init();
            //打开登陆面板
            PanelManager.Open<LoginPanel>();
      }
      private IEnumerator CopyDataBase(string from, string to)
      {
            WWW www = new WWW(from);
            while (www.isDone)
            {
                  yield return null;
            }
            //向给出的路径拷贝数据
            File.WriteAllBytes(to, www.bytes);
      }
      private void Update()
      {
            NetManager.Update();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                  PanelManager.Open<QuitPanel>();
            }

#elif UNITY_ANDROID || UNITY_IPHONE
            
            if (Input.GetKeyDown(KeyCode.Escape) )
            {
                  PanelManager.Open<QuitPanel>();
            }
#endif
      }
      //添加好友协议监听
      private void OnMsgAddFriend(MsgBase msgBase)
      {
            MsgAddFriend msg = (MsgAddFriend)msgBase;
            PanelManager.Open<MessageTIpPanel>(msg);
      }
      private void OnconnectClose(string str)
      {
            Debug.Log(str);
      }
      private void OnMsgKick(MsgBase msgBase)
      {
            PanelManager.Open<TipPanel>("被踢下线");
      }
      /// <summary>
      /// 加载场景
      /// </summary>
      /// <param name="scenename"></param>
      public void LoadScene(string scenename)
      {
             StartCoroutine(Load(scenename));
      }
      private IEnumerator Load(string scenename)
      {
            done = false;
            yield return StartCoroutine(LoadLoading());
            //获取组件
            Transform canvas = GameObject.Find("Canva").transform;
            Slider slider = TransformHelper.FindChiled(canvas, "Slider").GetComponent<Slider>();
            Text progressText = TransformHelper.FindChiled(canvas, "ProgressText").GetComponent<Text>();
            AsyncOperation async1 = SceneManager.LoadSceneAsync(scenename);//加载主场景 
            do
            {
                  slider.value = async1.progress;
                  progressText.text = async1.progress * 100 + "%";
                  yield return new WaitForFixedUpdate();
            } while (!async1.isDone);
            done = async1.isDone;
      }
      //private IEnumerator Load(string scenename)
      //{
      //      done = false;
      //      yield return StartCoroutine(LoadLoading());
      //      //获取组件
      //      Transform canvas = GameObject.Find("Canva").transform;
      //      Slider slider = TransformHelper.FindChiled(canvas, "Slider").GetComponent<Slider>();
      //      Text progressText = TransformHelper.FindChiled(canvas, "ProgressText").GetComponent<Text>();
      //      AsyncOperation async1 = SceneManager.LoadSceneAsync(scenename);//加载主场景 
      //            slider.value = async1.progress;
      //            progressText.text = async1.progress * 100 + "%";
      //            yield return new WaitUntil(()=>async1.isDone == true);
      //      done = async1.isDone;
      //}
      private IEnumerator LoadLoading()
      {
            AsyncOperation async = SceneManager.LoadSceneAsync("Loading");
            yield return new WaitUntil(()=>async.isDone == true);
      }
      /// <summary>
      /// 添加EventTrigger的监听事件
      /// </summary>
      /// <param name="obj">添加监听的对象</param>
      /// <param name="eventID">添加的监听类型</param>
      /// <param name="action">触发方法</param>
      public void AddTriggersListener(GameObject obj, EventTriggerType eventID, UnityAction<BaseEventData> action)
      {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                  trigger = obj.AddComponent<EventTrigger>();
            }

            if (trigger.triggers.Count == 0)
            {
                  trigger.triggers = new List<EventTrigger.Entry>();
            }

            UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(action);
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                  eventID = eventID
            };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
      }
}
