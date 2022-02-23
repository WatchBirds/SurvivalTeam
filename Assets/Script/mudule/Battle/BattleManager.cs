using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPS.Character;
using UnityEngine;
using AI.Perception;
using FPS.item;
/// <summary>
/// 管理整个战斗
/// </summary>
public class BattleManager : MonoSingleton<BattleManager>
{
      //玩家列表
      public Dictionary<string, BasePlayer> players = new Dictionary<string, BasePlayer>();
      //敌人列表(id由服务端提供)
      public Dictionary<string, MonsterStatu> monsters = new Dictionary<string, MonsterStatu>();
      //敌人同步队列
      public List<string> asynMonsterlist = new List<string>();
      //枪列表(id由服务端提供)
      public Dictionary<string, GunItem> gunitems = new Dictionary<string, GunItem>();
      public event Action<MsgCreatEnemy> OnReciveCreatEnemy;
      public event Action<BasePlayer> OnGeneratePlayer;
      public event Action<string> OnReciveChangetGun;
      public event Action<MsgKillEnemy> OnReciveKillEnemy;
      public event Action<string> OnReciveChangeScope;
      public delegate void Handles(out InputControl input);
      public event Handles inputHandle;
      public event Action<CtrlPlayer> OnCtrlPlayerBorn;

      public Transform playerPos;
      //房主id
      public string owonerId;
      public new void Init()
      {
            DontDestroyOnLoad(gameObject);
            //添加监听
            NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
            NetManager.AddMsgListener("MsgSyncPlayer", OnMsgSyncPlayer);
            //NetManager.AddMsgListener("MsgMotion", OnMsgMotion);
            //NetManager.AddMsgListener("MsgMotionBool", OnMsgMotionBool);
            NetManager.AddMsgListener("MsgFire", OnMsgFire);
            NetManager.AddMsgListener("MsgReload", OnMsgReload);
            NetManager.AddMsgListener("MsgCreatEnemy", OnMsgCreatEnemy);
            NetManager.AddMsgListener("MsgHit", OnMsgHit);
            NetManager.AddMsgListener("MsgHitCharacter", OnMsgHitCharacter);
            NetManager.AddMsgListener("MsgHitPlayer", OnMsgHitPlayer);
            NetManager.AddMsgListener("MsgChangeGun", OnMsgChangeGun);
            NetManager.AddMsgListener("MsgCreatGun", OnMsgCreatGun);
            NetManager.AddMsgListener("MsgKillEnemy", OnMsgKillEnemy);
            NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
            NetManager.AddMsgListener("MsgChangeScope", OnMsgChangeScope);
            NetManager.AddMsgListener("MsgPlayerDie", OnMsgPlayerDie);
            NetManager.AddMsgListener("MsgPlayerReborn", OnMsgPlayerReborn);
            NetManager.AddMsgListener("MsgGetOwerId", OnMsgGetOwerId);
            NetManager.AddMsgListener("MsgSyncMonster", OnMsgSyncMonster);
            NetManager.AddMsgListener("MsgChangeMold", OnMsgChangeMold);
      }
      //重置战场
      public void ResetBattle()
      {
            players.Clear();
            monsters.Clear();
            asynMonsterlist.Clear();
            gunitems.Clear();
            inputHandle = null;
            OnReciveKillEnemy = null;
            OnReciveChangetGun = null;
            OnReciveCreatEnemy = null;
            OnGeneratePlayer = null;
            OnReciveChangeScope = null;
            owonerId = null;
            NetManager.RemoveMsgListener("MsgBackToRoom", OnMsgBackToRoom);
            NetManager.RemoveMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
      }
      private void OnMsgSyncMonster(MsgBase msgBase)
      {
            MsgSyncMonster msg = (MsgSyncMonster)msgBase;
            if(GameMain.id == owonerId)//如果是房主
            { return; }
            if(!monsters.ContainsKey(msg.id))
            { return; }
            MonsterStatu monster = monsters[msg.id];
            Vector3 targetPos = new Vector3(msg.x, msg.y, msg.z);
            Vector3 targetEul = new Vector3(msg.ex, msg.ey, msg.ez);
            monster.transform.position = targetPos;
            monster.transform.eulerAngles = targetEul;
      }
      //获取房主id协议监听
      private void OnMsgGetOwerId(MsgBase msgBase)
      {
            MsgGetOwerId msg = (MsgGetOwerId)msgBase;
            BasePlayer player = GetPlayer(msg.ownerid);
            if (player == null)
            { return; }
            owonerId = msg.ownerid;
      }
      //玩家重生协议监听
      private void OnMsgPlayerReborn(MsgBase msgBase)
      {
            MsgPlayerReborn msg = (MsgPlayerReborn)msgBase;
            if (msg.result == 0)
            { return; }
            BasePlayer player = GetPlayer(msg.id);
            if (player == null)
            { return; }
            //Transform posPa = GameObject.Find("PlayerPos").transform;
            player.transform.position = playerPos.GetChild(UnityEngine.Random.Range(0, 4)).position + new Vector3(0, 2, 0);
            player.gameObject.SetActive(true);
            player.OnReBorn(100);
      }
      //玩家开火协议监听
      private void OnMsgFire(MsgBase msgBase)
      {
            MsgFire msg = (MsgFire)msgBase;
            if(msg.id == GameMain.id)
            { return; }
            SyncPlayer syncPlayer = (SyncPlayer)GetPlayer(msg.id);
            if(syncPlayer == null)
            { return; }
            syncPlayer.Fire(msg);
      }
      //玩家换弹协议监听
      private void OnMsgReload(MsgBase msgBase)
      {
            MsgReload msg = (MsgReload)msgBase;
            if(msg.id == GameMain.id)
            { return; }
            SyncPlayer syncPlayer = (SyncPlayer)GetPlayer(msg.id);
            if (syncPlayer == null)
            { return; }
            syncPlayer.Reload(msg);
      }
      //移动协议监听
      private void OnMsgSyncPlayer(MsgBase msgBase)
      {
            MsgSyncPlayer msg = (MsgSyncPlayer)msgBase;
            if (msg.id == GameMain.id)
            { return; }
            //查找玩家
            SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
            if (player == null)
            { return; }
            //移动同步
            player.SyncPos(msg);
      }
      //创建敌人协议监听
      private void OnMsgCreatEnemy(MsgBase msgBase)
      {
            GameObject trans = GameObject.Find("BirthPlayce0");
            GameObject trans1 = GameObject.Find("BirthPlayce1");
            MsgCreatEnemy msg = (MsgCreatEnemy)msgBase;
            Transform parent;
            for (int i = 0; i < msg.enemies.Length; i++)
            {
                  if (msg.enemies[i].pos == 0)
                  { parent = trans.transform; }
                  else
                  { parent = trans1.transform; }
                  GenerateEnemy(msg.enemies[i], parent.transform);
            }
            OnReciveCreatEnemy?.Invoke(msg);
            
      }
      //进入战斗协议监听
      private void OnMsgEnterBattle(MsgBase msgBase)
      {
            NetManager.AddMsgListener("MsgBackToRoom", OnMsgBackToRoom);
            NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
            PanelManager.Close("RoomPanel");
            MsgEnterBattle msg = (MsgEnterBattle)msgBase;
            //加载场景
            GameMain.instance.LoadScene("Demo_Scene_2");
            //等待场景加载完成
            StartCoroutine(Battle(msg));    
      }
      //敌人受击协议监听
      private void OnMsgHitCharacter(MsgBase msgBase)
      {
            MsgHitCharacter msg = (MsgHitCharacter)msgBase;
            if(!monsters.ContainsKey(msg.hitId))
            { return; }
            MonsterStatu monster = monsters[msg.hitId];
            monster.OnDamage(msg.damage);
            //受攻击后生命值小于0发送击杀协议
            if(monster.Hp<=0&&msg.id == GameMain.id)
            {
                  MsgKillEnemy msgKill = new MsgKillEnemy();
                  msgKill.id = msg.id;
                  msgKill.bekillId = msg.hitId;
                  msgKill.name = msg.hitName;
                  msgKill.coin = monster.coint.ToString();
                  NetManager.Send(msgKill);
            }
      }
      //玩家受击协议监听
      private void OnMsgHitPlayer(MsgBase msgBase)
      {
            MsgHitPlayer msg = (MsgHitPlayer)msgBase;
            BasePlayer player = GetPlayer(msg.hitId);
            if (player == null)
            { return; }
                  player.OnDamage(msg.damage);
      }
      //敌人死亡协议监听
      private void OnMsgKillEnemy(MsgBase msgBase)
      {
            MsgKillEnemy msg = (MsgKillEnemy)msgBase;
            if(!monsters.ContainsKey(msg.bekillId))
            { return; }
            MonsterStatu monster = monsters[msg.bekillId];
            monster.Dead();
            monsters.Remove(msg.bekillId);
            asynMonsterlist.Remove(msg.bekillId);
            OnReciveKillEnemy?.Invoke(msg); 
      }
      //战斗结果协议监听
      private void OnMsgBattleResult(MsgBase msgBase)
      {
            MsgBattleResult msg = (MsgBattleResult)msgBase;
            inputHandle(out InputControl input);
            PanelManager.Open<ResultPanel>(msg,input);
      }
      //离开战斗协议监听
      private void OnMsgLeaveBattle(MsgBase msgBase)
      {
            MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
            //查找玩家
            BasePlayer player = GetPlayer(msg.id);
            if (player == null)
            { return; }
            //如果是自己
            if (msg.id == GameMain.id)
            {
                 ResetBattle();
                  CancelInvoke();
                  PanelManager.CloseAllPanel();
                  //加载ui场景打开房间列表
                  GameMain.instance.LoadScene("Ui");
                  StartCoroutine(OpenPanel<RoomListPanel>());
                  
            }
            else
            {
                  SyncPlayer syncPlayer = (SyncPlayer)player;
                  syncPlayer.LeaveBattle();
                  SensorTriggerSystem.instance.RemoveTrigger(player.GetComponent<AbstractTrigger>());
                  //删除玩家
                  RemovePlayer(msg.id);
                  Destroy(player.gameObject);
            }                                                                                                                                                         
      }
      private void OnMsgChangeMold(MsgBase msgBase)
      {
            MsgChangeMold msg = (MsgChangeMold)msgBase;
            if(msg.id == GameMain.id)
            { return; }
            if(!players.ContainsKey(msg.id))
            { return; }
            SyncPlayer syncPlayer =(SyncPlayer)players[msg.id];
            syncPlayer.ChangeGunMold(msg);
      }
      //返回房间协议监听
      private void OnMsgBackToRoom(MsgBase msgBase)
      {
            MsgBackToRoom msg = (MsgBackToRoom)msgBase;
            if(msg.result == 0)
            { return; }
            BasePlayer player = GetPlayer(msg.id);
            //如果是自己
            if (msg.id == GameMain.id)
            {
                  ResetBattle();
                  CancelInvoke();
                  //关闭面板
                  PanelManager.CloseAllPanel();
                  //加载场景
                  GameMain.instance.LoadScene("Ui");
                  //打开房间Panel
                  StartCoroutine(OpenPanel<RoomPanel>());
            }
            else
            {
                  SensorTriggerSystem.instance.RemoveTrigger(player.GetComponent<AbstractTrigger>());
                  RemovePlayer(msg.id);
                  Destroy(player.gameObject);
            }
         
      }
      private IEnumerator OpenPanel<T>()where T:BasePanel
      {
            while(!GameMain.done)
            {
                  yield return null;
            }
            PanelManager.Open<T>();
            StopAllCoroutines();
      }
      //子弹击中协议监听
      private void OnMsgHit(MsgBase msgBase)
      {
            MsgHit msg = (MsgHit)msgBase;
            if (msg.id == GameMain.id)
            { return; }
            //生成特效
            GameObject fxpa = ResourceManager.Load<GameObject>(msg.fxname);
            if (fxpa == null)
            { return; }
            Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
            Vector3 fow = new Vector3(msg.ex, msg.ey, msg.ez);
            Quaternion quaternion = Quaternion.LookRotation(fow);
            GameObjectPool.instance.CreatObject(fxpa.name, fxpa, pos, quaternion);
      }
      //换枪协议监听
      private void OnMsgChangeGun(MsgBase msgBase)
      {
            MsgChangeGun msg = (MsgChangeGun)msgBase;
            if (!gunitems.ContainsKey(msg.gunid))
            {
                  return;
            }
            if(gunitems[msg.gunid].Owner!=null)
            { return; }
            if (msg.id == GameMain.id)
            {
                  OnReciveChangetGun?.Invoke(msg.gunid);
                  return;
            }
            //查找玩家
            SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
            if (player == null)
            { return; }
            GunItem gun = gunitems[msg.gunid];
            player.ChangeGun(gun);
      }
      //玩家死亡协议监听
      private void OnMsgPlayerDie(MsgBase msgBase)
      {
            MsgPlayerDie msg = (MsgPlayerDie)msgBase;
            //查找玩家
            BasePlayer player = GetPlayer(msg.id);
            if (player == null)
            { return; }
            player.Dead();
            player.gameObject.SetActive(false);//隐藏模型
            //创建死亡模型
            GameObject prefab = ResourceManager.Load<GameObject>("Third_Person_Character_Ragdoll_Prefab");
            GameObject go = Instantiate(prefab, player.transform.position, Quaternion.identity);
            Destroy(go, 15);
      }
      /// <summary>
      /// 延迟发送消息
      /// </summary>
      /// <param name="msg">消息</param>
      /// <param name="time">时间</param>
      public void SendMsg(MsgBase msg,float time)
      {
            StartCoroutine(SendMsgDelay(msg, time));
      }
      
      IEnumerator SendMsgDelay(MsgBase msg,float delateTime)
      {
            yield return new WaitForSeconds(delateTime);
            NetManager.Send(msg);
      }
      private void OnMsgChangeScope(MsgBase msgBase)
      {
            MsgChangeScope msg = (MsgChangeScope)msgBase;
            if(msg.result == 0)
            { return; }
            if(msg.id == GameMain.id)//如果是自己
            {
                  OnReciveChangeScope?.Invoke(msg.scopeName);
                  return;
            }
            //查找玩家
            SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
            player.ChangeScope(msg.scopeName);

      }
      //当收到生成枪的协议
      private void OnMsgCreatGun(MsgBase msgBase)
      {
            Transform pospa = GameObject.Find("GunPos").transform;
            MsgCreatGun msg = (MsgCreatGun)msgBase;
            for(int i = 0;i<msg.guns.Length;i++)
            {
                  GenerateGun(msg.guns[i], pospa);
            }
      }
      private IEnumerator Battle(MsgEnterBattle msg)
      {
            while(!GameMain.done)
            {
                  yield return null; 
            }
            //查找玩家位置父物体
            playerPos = GameObject.Find("PlayerPos").transform;
            //生成玩家
            for (int i = 0;i<msg.characters.Length;i++)
            {
                  GeneratePlayer(msg.characters[i], playerPos.GetChild(i));
            }
            
            //发送MsgLoadFin协议
            MsgLoadFin msgs = new MsgLoadFin();
            msgs.id = GameMain.id;
            //隔段时间发送协议
            SendMsg(msgs, 10);
            //发送获取房主id协议
            MsgGetOwerId msgGetOwer = new MsgGetOwerId();
            NetManager.Send(msgGetOwer);
            //开始同步敌人( 每0.1秒只同步一个敌人 )
            InvokeRepeating("SyncEnemyPos", 0, 0.1f);
      }
      //生成枪
      private void GenerateGun(GunInfo gunInfo,Transform pare)
      {
            GameObject go = ResourceManager.Load<GameObject>(gunInfo.gunname);
            GameObject gun = Instantiate(go);
            GunItem item = gun.GetComponent<GunItem>();
            item.id = gunInfo.id;
            gun.transform.position = pare.GetChild(gunInfo.posindex).position;
            gunitems.Add(item.id, item);
      }
      //生成玩家
      private void GeneratePlayer(CharacterInfo characterInfo,Transform positon)
      {
            string objName = "Player_" + characterInfo.id;
            BasePlayer basePlayer;
            BaseWeapenMana weapenMana;
            if (characterInfo.id == GameMain.id)
            {
                  GameObject playerobj = ResourceManager.Load<GameObject>("FpsBasePlayer");
                  GameObject player = Instantiate(playerobj);
                  player.AddComponent<CharacherAnimator>();
                  basePlayer = player.AddComponent<CtrlPlayer>();
                  weapenMana = player.AddComponent<WeaPenMana>();
                  player.AddComponent<LookingStuff>();
                //  player.AddComponent<PlayerMotor>();
                  GameMain.gunName = characterInfo.gunName;
            }
            else
            {
                  GameObject playerobj = ResourceManager.Load<GameObject>("BasePlayer");
                  GameObject player = Instantiate(playerobj);
                  basePlayer = player.GetComponent<SyncPlayer>();
                  weapenMana = player.AddComponent<SyncWeapenMana>();
                  basePlayer.Init("third_person_character_lpfp@handgun");
            }
            //属性
            basePlayer.id = characterInfo.id;
            basePlayer.Hp = characterInfo.hp;
            //位置旋转
            Vector3 pos = new Vector3(characterInfo.x, characterInfo.y, characterInfo.z);
            Vector3 rot = new Vector3(characterInfo.ex, characterInfo.ey, characterInfo.ez);
            basePlayer.transform.position = positon.position;
            basePlayer.transform.eulerAngles = rot;
            //init
            weapenMana.Init(characterInfo.gunName);
            //把玩家默认的枪加入列表( id为玩家id+"DefauleGun" )
            gunitems.Add(weapenMana.currentWea.id, weapenMana.currentWea);
            AddPlayer(characterInfo.id, basePlayer);
            //调用事件
            OnGeneratePlayer?.Invoke(basePlayer);
      }
   //生成敌人
      private void  GenerateEnemy(EnemyInfo enemy,Transform parent)
      {
            GameObject prtfab = ResourceManager.Load<GameObject>(enemy.prefabName);
            Vector3 pos = parent.GetChild(enemy.birthPos).position;
            GameObject go = Instantiate(prtfab,pos,Quaternion.identity);
            MonsterStatu player = go.GetComponent<MonsterStatu>();
            player.id = enemy.id;
            monsters.Add(player.id, player);
            asynMonsterlist.Add(player.id);
      }
      /// <summary>
      /// 添加玩家
      /// </summary>
      /// <param name="id">玩家id</param>
      /// <param name="player">玩家类型</param>
      public void AddPlayer(string id,BasePlayer player)
      {
            players[id] = player;
      }
      /// <summary>
      /// 移除玩家
      /// </summary>
      /// <param name="id">玩家id</param>
      public void RemovePlayer(string id)
      {
            players.Remove(id);
      }
      /// <summary>
      /// 获取玩家
      /// </summary>
      /// <param name="id">玩家id</param>
      /// <returns></returns>
      public BasePlayer GetPlayer(string id)
      {
            if(players.ContainsKey(id))
            {
                  return players[id];
            }
            return null;
      }
      /// <summary>
      /// 获取玩家控制的角色
      /// </summary>
      /// <returns></returns>
      public BasePlayer GetCtrlPlayer()
      {
            return GetPlayer(GameMain.id);
      }
      //同步敌人列表下标
      private int asyIndex =0;
      //同步敌人位置(房主发送)创建敌人完成后开始 离开战斗停止
      private void SyncEnemyPos()
      {
            if(GameMain.id!=owonerId)
            { return; }
            if(asynMonsterlist.Count <= 0)
            { return; }
            if(asyIndex > monsters.Count-1)
            {
                  asyIndex = 0;
            }
            if(!monsters.ContainsKey(asynMonsterlist[asyIndex]))
            { return; }
            MonsterStatu monster = monsters[asynMonsterlist[asyIndex]];
            MsgSyncMonster msg = new MsgSyncMonster();
            msg.x = monster.transform.position.x;
            msg.y = monster.transform.position.y;
            msg.z = monster.transform.position.z;
            msg.ex = monster.transform.eulerAngles.x;
            msg.ey = monster.transform.eulerAngles.y;
            msg.ez = monster.transform.eulerAngles.z;
            msg.id = monster.id;
            NetManager.Send(msg);
            asyIndex++;
      }
}

