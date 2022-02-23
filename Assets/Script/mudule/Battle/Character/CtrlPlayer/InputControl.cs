using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FPS.Character;
using FPS.item;
using AI.Perception;
using UnityEngine.UI;
using System.Collections;
using System.IO;
/// <summary>
/// 外部输入控制
/// </summary>
public class InputControl:MonoBehaviour
{
      public delegate void MoveHandle(Vector2 vector);
      public delegate void Bthandle();
      public delegate void IconHandle(string name);
      public delegate void FireBtHandle(bool b);
      /// <summary>
      /// 当按下开火按钮
      /// </summary>
      public static event FireBtHandle OnFirebtdown;
      /// <summary>
      /// 当按下瞄准按钮
      /// </summary>
      public static event Bthandle OnAimbtdown;
      /// <summary>
      /// 当移动的时候
      /// </summary>
      public static event MoveHandle OnMove;
      public static event IconHandle OnClickWeapenIcon;
      public static event IconHandle OnClickScopeICon;
      public static event Bthandle OnRelaod;
      public static event Bthandle OnJumpBtdown;
      public static event Action<bool> OBtDown;
      public static event Action OBtUp;
      //private Dictionary<string, GameObject> Images = new Dictionary<string, GameObject>();
      //图片列表
      private Transform content;
      private GameObject ScrollView;
      private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
      private Dictionary<string, GameObject> weapenIcons = new Dictionary<string, GameObject>();
      private Transform layer2;
      private Transform layer1;
      //枪信息父物体
      private Transform gunIconData;
      //显示子弹剩余
      private Text leftBulletText;
      //显示子弹总剩余
      private Text allBulletText;
      //显示当前枪的图片
      private Image gunIcon;
      //切换枪射击模式的按钮
      private Button moldIcon;
      //玩家信息物体
      private GameObject playerData;
      //控制玩家血条
      private Slider mainHpSlider;
      //玩家信息列表
      private Transform dataList;
      private GameObject headData;
      //头顶图片下标
      private int idx = 0;
      private Text killNumber;
      private Text allEnemyNumber;
      private Text leftEnemyNumber;
      //记录玩家击杀数
      public Dictionary<string, int> countDic = new Dictionary<string, int>();
      //记录玩家获取金币数
      public Dictionary<string, int> coinDic = new Dictionary<string, int>();
      //镜片列表父物体
      private Transform scopeContent;
      //所有镜片图片
      private List<Image> scopeIcons = new List<Image>();

      //交互部分
      private Button interactionBt;
      private Text interactionText;
      private Iinteraction curretnInter;//当前交互物体
      private GameObject useTarget;//发起交互的对象

      //屏幕中心图标
      private Image centerImage;
      //屏幕中心图标动画控制器
      private Animator centerAnimator;
      //敌人信息父物体
      private Transform enemyData;
      //敌人名称
      private Text enemyName;
      //敌人血量hp
      private Slider enemyHp;
      //复活提示文本
      private Text rebornText;
      //广播文本
      private Text redioText;
      private Animator redioAnimator;
      //设置按钮
      private Button setBt;
      //聊天部分
      private Transform TalkPanel;
      private InputField inputFile;
      private Text textFile;
      private Button sendBt;

      public Text tipText;
      private void Start()
      {      
            // 添加监听
            NetManager.AddMsgListener("MsgTalk", OnMsgTalk);
            //寻找组件
            ScrollView = TransformHelper.FindChiled(transform, "Scroll View_Itemlist").gameObject;
            content = TransformHelper.FindChiled(transform, "Scroll View_Itemlist/Viewport/Content");
            var sprites = Resources.LoadAll<Sprite>("Ui/GunSprite/Gun_Icons_Spritesheet");
            gunIconData = TransformHelper.FindChiled(transform, "GunIconData");
            leftBulletText = TransformHelper.FindChiled(gunIconData, "LeftBulletText").GetComponent<Text>();
            allBulletText = TransformHelper.FindChiled(gunIconData, "AllBulletText").GetComponent<Text>();
            moldIcon = TransformHelper.FindChiled(gunIconData, "ShotMold").GetComponent<Button>();
            gunIcon = TransformHelper.FindChiled(gunIconData, "GunIcon").GetComponent<Image>();
            playerData = TransformHelper.FindChiled(transform, "PlayerData").gameObject;
            mainHpSlider = TransformHelper.FindChiled(transform, "MainHpSlider").GetComponent<Slider>();
            dataList = TransformHelper.FindChiled(transform, "DataList").transform;
            headData = TransformHelper.FindChiled(transform, "HeadData").gameObject;
            layer1 = TransformHelper.FindChiled(transform, "Layer1");
            layer2 = TransformHelper.FindChiled(transform, "Layer2");
            killNumber = TransformHelper.FindChiled(transform, "KillNumber").GetComponent<Text>();
            allEnemyNumber = TransformHelper.FindChiled(transform, "AllEnemyNumber").GetComponent<Text>();
            leftEnemyNumber = TransformHelper.FindChiled(transform, "LeftEnemyNumber").GetComponent<Text>();
            scopeContent = TransformHelper.FindChiled(layer1, "ScopeIcons/ScopeContent");
            interactionBt = TransformHelper.FindChiled(layer1, "UseButton").GetComponent<Button>();
            interactionText = interactionBt.GetComponentInChildren<Text>();
            centerImage = TransformHelper.FindChiled(transform, "CenterImage").GetComponent<Image>();
            centerAnimator = centerImage.GetComponent<Animator>();
            enemyData = TransformHelper.FindChiled(transform, "EnemyData");
            enemyName = enemyData.GetComponentInChildren<Text>();
            enemyHp = enemyData.GetComponentInChildren<Slider>();
            rebornText = TransformHelper.FindChiled(transform, "RebornTime").GetComponent<Text>();
            redioText = TransformHelper.FindChiled(transform, "RdioText").GetComponent<Text>();
            redioAnimator = redioText.GetComponent<Animator>();
            setBt = TransformHelper.FindChiled(transform, "SetBt").GetComponent<Button>();
            TalkPanel = TransformHelper.FindChiled(transform, "TalkPanel");
            textFile = TransformHelper.FindChiled(TalkPanel, "TextField").GetComponent<Text>();
            inputFile = TransformHelper.FindChiled(TalkPanel, "InputField").GetComponent<InputField>();
            sendBt = TransformHelper.FindChiled(TalkPanel, "SendBt").GetComponent<Button>();
            //绑定事件
            setBt.onClick.AddListener(() => { PanelManager.Open<SystemSetPanel>(true); });
            sendBt.onClick.AddListener(() => { if (string.IsNullOrEmpty(inputFile.text)) { return; } MsgTalk msg = new MsgTalk();msg.msg = inputFile.text;NetManager.Send(msg); });
            interactionBt.onClick.AddListener(IteractionBtEvent);
            for (int i = 0;i<scopeContent.childCount;i++)
            {
                  Image scopeIcon = scopeContent.GetChild(i).GetComponent<Image>();
                  scopeIcons.Add(scopeIcon);
            }
            foreach(var sprit in sprites)
            {
                  this.sprites.Add(sprit.name, sprit);
            }

           SetIconPostion();

            WeaPenMana.OnEnter += WeaPenMana_OnEnter;
            WeaPenMana.OnExit += WeaPenMana_OnExit;
            WeaPenMana.OnDrop += WeaPenMana_OnDrop;
            WeaPenMana.OnFire += WeaPenMana_OnFire;
            WeaPenMana.OnReload += WeaPenMana_OnReload;
            WeaPenMana.OnInit += WeaPenMana_OnInit;
            WeaPenMana.OnAim += WeaPenMana_OnAim;
            WeaPenMana.OnCancleAim += WeaPenMana_OnCancleAim;
            WeaPenMana.OnChangScope += WeaPenMana_OnChangScope;
            WeaPenMana.OnCancleFire += WeaPenMana_OnCancleFire;
            BattleManager.instance.OnGeneratePlayer += Instance_OnGeneratePlayer;
            BattleManager.instance.OnReciveCreatEnemy += Instance_OnReciveCreatEnemy;
            BattleManager.instance.OnReciveKillEnemy += Instance_OnReciveKillEnemy;
            BattleManager.instance.inputHandle += Instance_inputHandle;
            CtrlPlayer.OnPlayerCheck += CtrlPlayer_OnPlayerCheck;
            SetInputPanel.OnSaveIconConfigHandle += SetIconPostion;
            LookingStuff.OnRaycastHit += LookingStuff_OnRaycastHit;
            LookingStuff.OnRaycastDontHit += LookingStuff_OnRaycastDontHit;
            CtrlPlayer.OnPlayerDie += CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn += CtrlPlayer_OnPlayerBorn;
            WeaPenMana.OnGetBullet += WeaPenMana_OnGetBullet;
            StartCoroutine(OnStart());
      }
      private void SetIconPostion()
      {
            //读取配置文件
            IconValueConfig IValueConfig = (IconValueConfig)JsonUtility.FromJson(File.ReadAllText(GameMain.inputConfigPath), typeof(IconValueConfig));
            if (IValueConfig == null)
            { return; }
            for (int i = 0; i < IValueConfig.iconValues.Length; i++)
            {
                  SetIcon(IValueConfig.iconValues[i]);
            }
      }
      private void SetIcon(IconValue iconValue)
      {
            RectTransform rect = TransformHelper.FindChiled(layer1, iconValue.iconName).rectTransform();
            if (rect == null)
            { return; }
            rect.anchoredPosition = new Vector2(iconValue.x, iconValue.y);
            rect.localScale = new Vector3(iconValue.scale, iconValue.scale, iconValue.scale);
            Image image = rect.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, iconValue.A);
      }

      private void CtrlPlayer_OnPlayerCheck(Iinteraction arg1, GameObject arg2)
      {
            if (arg1 == null || arg2 == null)
            {
                  if (interactionBt.gameObject.activeSelf)
                  {
                        interactionBt.gameObject.SetActive(false);

                  }
                  useTarget = null;
                  curretnInter = null;
                  return;
            }
            if (interactionBt.gameObject.activeSelf)
            { return; }
            interactionBt.gameObject.SetActive(true);
            useTarget = arg2;
            curretnInter = arg1;
            interactionText.text = arg1.Str;

      }
      //交互按钮监听
      private void IteractionBtEvent()
      {
            if (useTarget == null || curretnInter == null)
            { return; }
            curretnInter.Use(useTarget);
            //curretnInter.BeCheck = false;
      }
      //消息信息监听
      private void OnMsgTalk(MsgBase msgBase)
      {
            MsgTalk msg = (MsgTalk)msgBase;
            if (msg.id == GameMain.id)
            {
                  inputFile.text = string.Empty;
            }
            string fullMsg = (msg.id + ": " + msg.msg + "\n").ToString();
            textFile.text += fullMsg;
      }
      IEnumerator OnStart()
      {
            tipText.gameObject.SetActive(true);
            float timeco = 10f;
            while(timeco>=0)
            {
                  timeco -= Time.deltaTime;
                  tipText.text = string.Format("敌人大约<color=red>{0}</color>秒后到达", timeco.ToString("0"));
                  yield return null;
            }
            tipText.gameObject.SetActive(false);
      }

      private void OnDestroy()
      {
            NetManager.RemoveMsgListener("MsgTalk", OnMsgTalk);
            WeaPenMana.OnEnter -= WeaPenMana_OnEnter;
            WeaPenMana.OnExit -= WeaPenMana_OnExit;
            WeaPenMana.OnDrop -= WeaPenMana_OnDrop;
            WeaPenMana.OnFire -= WeaPenMana_OnFire;
            WeaPenMana.OnReload -= WeaPenMana_OnReload;
            WeaPenMana.OnInit -= WeaPenMana_OnInit;
            WeaPenMana.OnAim -= WeaPenMana_OnAim;
            WeaPenMana.OnCancleAim -= WeaPenMana_OnCancleAim;
            WeaPenMana.OnChangScope -= WeaPenMana_OnChangScope;
            WeaPenMana.OnCancleFire -= WeaPenMana_OnCancleFire;
            BattleManager.instance.OnGeneratePlayer -= Instance_OnGeneratePlayer;
            BattleManager.instance.OnReciveCreatEnemy -= Instance_OnReciveCreatEnemy;
            BattleManager.instance.OnReciveKillEnemy -= Instance_OnReciveKillEnemy;
            BattleManager.instance.inputHandle -= Instance_inputHandle;
            LookingStuff.OnRaycastHit -= LookingStuff_OnRaycastHit;
            LookingStuff.OnRaycastDontHit -= LookingStuff_OnRaycastDontHit;
            CtrlPlayer.OnPlayerDie -= CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn -= CtrlPlayer_OnPlayerBorn;
            CtrlPlayer.OnPlayerCheck -= CtrlPlayer_OnPlayerCheck;
            WeaPenMana.OnGetBullet -= WeaPenMana_OnGetBullet;
            SetInputPanel.OnSaveIconConfigHandle -= SetIconPostion;
      }
      //换镜片事件监听
      private void WeaPenMana_OnChangScope(string scopeName)
      {
            //找出给出的名字对应的Image
            Image scopeImage = Array.Find(scopeIcons.ToArray(), a => a.name == scopeName);
            //找出颜色是黄色的Image（上一次点击的图片）
            Image lastClick = Array.Find(scopeIcons.ToArray(), a => a.color == Color.yellow);
            //判断是不是上一次的Image
            if(scopeImage==lastClick)
            { scopeImage.color = Color.black; return; }
            //将上一次的Image设为黑色当前点击Image设为黄色
            if (lastClick != null)
            {
                  lastClick.color = Color.black;
            }
            scopeImage.color = Color.yellow;
      }
      private void WeaPenMana_OnGetBullet(int bulletBumber)
      {
            allBulletText.text = bulletBumber.ToString();
      }
      //玩家死亡事件监听
      private void CtrlPlayer_OnPlayerDie()
      {
            layer1.gameObject.SetActive(false);
            rebornText.gameObject.SetActive(true);
            redioAnimator.Play("Show State"); redioText.text = string.Format("<color=green>{0}</color>被击杀", GameMain.id);//死亡广播消息
            //开始文本倒计时
            StartCoroutine(Reborncont());
      }
      IEnumerator Reborncont()
      {
            float timecount = 18f;
            while (timecount >= 0)
            {
                  timecount -= Time.deltaTime;
                  rebornText.text = timecount.ToString("0.0");
                  yield return null;
            }
      }
      //玩家复活事件监听
      private void CtrlPlayer_OnPlayerBorn(CtrlPlayer ctrlPlayer)
      {
            mainHpSlider.value = mainHpSlider.maxValue;
            layer1.gameObject.SetActive(true);
            rebornText.gameObject.SetActive(false);
      }
      //取消瞄准事件监听
      private void WeaPenMana_OnCancleAim(GunItem gun)
      {
            centerImage.enabled = true;
      }
      //瞄准事件监听
      private void WeaPenMana_OnAim(GunItem gun)
      {
            centerImage.enabled = false;
      }
      //收到战斗结果协议
      private void Instance_inputHandle(out InputControl input)
      {
            input = this;
            gameObject.SetActive(false);
      }
      //收到敌人死亡协议监听
      private void Instance_OnReciveKillEnemy(MsgKillEnemy obj)
      {
            if (countDic.ContainsKey(obj.id))
            { countDic[obj.id] += 1; }//增加击杀数
            if(coinDic.ContainsKey(obj.id))
            { coinDic[obj.id] += int.Parse( obj.coin); }//增加金币
            killNumber.text = countDic[GameMain.id].ToString();
            leftEnemyNumber.text = (int.Parse(leftEnemyNumber.text) - 1).ToString();
            string colorText;
            if(obj.id == GameMain.id)
            {
                  colorText = "green";
            }
            else
            {
                  colorText = "yellow";
            }
            redioAnimator.Play("Show State");
           redioText.text = string.Format("<color={0}>{1}</color>击杀了<color=red>{2}</color>", colorText, obj.id, obj.name);
      }

      //收到生成敌人协议监听
      private void Instance_OnReciveCreatEnemy(MsgCreatEnemy msg)
      {
            allEnemyNumber.text = msg.enemies.Length.ToString();
            leftEnemyNumber.text = msg.enemies.Length.ToString();
            killNumber.text = "0";
      }

      //生成玩家时绑定ui信息
      private void Instance_OnGeneratePlayer(BasePlayer obj)
      {
            if (obj.id == GameMain.id)//控制玩家
            {
                  mainHpSlider.value = obj.Hp;
                  obj.HpCostHandle += (int value) => { mainHpSlider.value = value; };//绑定血量变化事件
                
            }
            countDic[obj.id] = 0;//玩家击杀数
            coinDic[obj.id] = 0;//玩家获得金币数
            //玩家左上信息
            GameObject go = Instantiate(playerData);
            go.SetActive(true);
            go.transform.SetParent(dataList);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            Slider hpslider = TransformHelper.FindChiled(go.transform, "HpSlider").GetComponent<Slider>();
            Text idtext = TransformHelper.FindChiled(go.transform, "IdText").GetComponent<Text>();
            Image number = TransformHelper.FindChiled(go.transform, "NumberImage").GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Ui/NumerUi/number_" + (idx + 1));
            number.sprite = sprite;
            hpslider.value = obj.Hp;
            idtext.text = obj.id;
            obj.HpCostHandle += (int value) => { hpslider.value = value; };//绑定事件生命值变化调用
            if(obj.id!=GameMain.id)//同步玩家
            {
                  SyncPlayer sync = (SyncPlayer)obj;
                  sync.OnSyncLeaveBattle += (string id) => { go.SetActive(false); };//绑定玩家退出游戏事件
                  //生成头顶信息
                  GameObject gos = Instantiate(headData);
                  gos.transform.SetParent(layer2);
                  gos.transform.localPosition = Vector3.zero;
                  gos.SetActive(true);
                  //将3d物体坐标转屏幕坐标
                  Vector3 pos = TransformHelper.FindChiled(obj.transform, "UiPos").position;
                  Vector2 temppos = Camera.main.WorldToScreenPoint(pos);
                  float x = Mathf.Clamp(temppos.x, 1, Screen.width - 1);
                  float y = Mathf.Clamp(temppos.y, 1, Screen.height - 1);
                  Vector2 newpos = new Vector2(x, y);
                  (gos.transform as RectTransform).anchoredPosition = newpos;
                  gos.transform.localScale = Vector3.one;
                  Image image = gos.GetComponentInChildren<Image>();
                  Text text = gos.GetComponentInChildren<Text>();
                  image.sprite = sprite;
                  text.text = obj.id;
                  SyncPlayer player = (SyncPlayer)obj;
                  player.OnSyncMove += gos.GetComponent<UiMapp>().Mapping;//绑定移动事件
                  player.OnSyncDie += delegate () { gos.SetActive(false); //同步玩家死亡事件
                        redioAnimator.Play("Show State"); 
                        redioText.text = string.Format("<color=yellow>{0}</color>被击杀", player.id); };//绑定死亡事件
                  player.OnSyncReborn += delegate () { gos.SetActive(true); };//绑定复活事件
                  player.OnSyncLeaveBattle += (string id) => { gos.SetActive(false); 
                        redioAnimator.Play("Show State");
                        redioText.text = string.Format("<color=yellow>{0}</color>离开了战斗", player.id);
                  };//绑定玩家离开战斗事件
            }
            idx++;     
      }
      //初始化枪事件监听
      private void WeaPenMana_OnInit(GunItem gun)
      {
            scopeIcons.ForEach(a => a.color = Color.black);
            //设置信息
            gunIcon.sprite = sprites[gun.gunData.iconName];
            leftBulletText.text = gun.lefeBullet.ToString();
            allBulletText.text = gun.allBullet.ToString();
            Text moldText = moldIcon.GetComponentInChildren<Text>();
            moldText.text = gun.CurrentModl.ToString();
            //给镜片按钮添加监听
            if (gun.gunData.gunType != GunData.GunType.Sniper)
            {
                  for (int i = 0; i < scopeContent.childCount; i++)
                  {
                        Button bt = scopeContent.GetChild(i).GetComponent<Button>();
                        bt.onClick.AddListener(() => { OnClickScopeICon?.Invoke(bt.gameObject.name); });
                  }
            }
            else
            {
                  for (int i = 0; i < scopeContent.childCount; i++)
                  {
                        Button bt = scopeContent.GetChild(i).GetComponent<Button>();
                        bt.onClick.RemoveAllListeners();
                  }

            }
            //给切换射击模式按钮添加事件
            moldIcon.onClick.AddListener(() => { 
                  GunData.ShotModl modl = ArrayHelper.EnumNext(gun.CurrentModl);
                  if ((gun.gunData.modls & modl) != 0) { gun.CurrentModl = modl; moldText.text = gun.CurrentModl.ToString(); //发送协议
                        MsgChangeMold msg = new MsgChangeMold();msg.mold = gun.CurrentModl.ToString(); NetManager.Send(msg);
                  }
            });
      }
      //换弹事件监听
      private void WeaPenMana_OnReload(GunItem gun)
      {
            //设置信息
            leftBulletText.text = gun.lefeBullet.ToString();
            allBulletText.text = gun.allBullet.ToString();
      }
      //开火事件监听
      private void WeaPenMana_OnFire(GunItem gun)
      {
            //设置信息
            leftBulletText.text = gun.lefeBullet.ToString();
            allBulletText.text = gun.allBullet.ToString();
            //if(centerAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShotState"))
            //{ return; }
            centerAnimator.Play("ShotState");
      }
      //取消开火事件监听
      private void WeaPenMana_OnCancleFire(GunItem gun)
      {
            //centerAnimator.Play("BackState");
      }
      //丢弃枪事件监听
      private void WeaPenMana_OnDrop(GunItem gun)
      {
            //设置信息
            gunIcon.sprite = null;
            leftBulletText.text = "";
            allBulletText.text = "";
            //移除事件
            moldIcon.onClick.RemoveAllListeners();
            //给镜片按钮移除监听
            for (int i = 0; i < scopeContent.childCount; i++)
            {
                  Button bt = scopeContent.GetChild(i).GetComponent<Button>();
                  bt.onClick.RemoveAllListeners();
            }
      }
      //枪离开范围事件监听
      private void WeaPenMana_OnExit(GunItem obj)
      {
            if(weapenIcons.ContainsKey(obj.id))
            {
                  Destroy(weapenIcons[obj.id]);
                  weapenIcons.Remove(obj.id);   
            }
            if(weapenIcons.Count<=0)
            {
                  ScrollView.SetActive(false);
            }
      }
      //枪进入范围事件监听
      private void WeaPenMana_OnEnter(GunItem obj)
      {
            ScrollView.SetActive(true);
            GenerateImage(obj);
      }
      //主相机射线事件监听
      private void LookingStuff_OnRaycastHit(RaycastHit hit)
      {
            if (hit.transform.tag != "Enemy")
            { enemyData.gameObject.SetActive(false); return; }
            if (enemyData.gameObject.activeSelf == false)
            {
                  enemyData.gameObject.SetActive(true);
            }
            MonsterStatu monster = hit.transform.GetComponent<MonsterStatu>();
            enemyName.text = monster.monsterName;
            enemyHp.value = monster.Hp / (float)monster.MaxHp;
      }
      private void LookingStuff_OnRaycastDontHit()
      {
            if (enemyData.gameObject.activeSelf == false)
            { return; }
            enemyData.gameObject.SetActive(false);
      }

      public void OnJoyStickMove(Vector2 vector)
      {
            OnMove?.Invoke(vector);
      }
      public void OnJoyStickStop()
      {
            OnMove(Vector2.zero);
      }
      public  void OnAimBtDown()
      {
            OnAimbtdown();
      }
      public  void OnFireBtDown(bool b)
      {
            OnFirebtdown(b);
      }
      public void OnReloadBtDown()
      {
            OnRelaod();
      }
      public void OnJumpBtDown()
      {
            OnJumpBtdown();
      }
      public void IFTouch(bool b)
      {
            OBtDown(b);
      }
      public void IFTouchUp()
      {
            OBtUp();
      }
      //初始化一个枪图片
      private void GenerateImage(GunItem gun)
      {
            string key = gun.id;
            Sprite sprite = sprites[gun.gunData.iconName];
            //创建物体 添加组件
            GameObject go = new GameObject(gun.id);
            go.transform.SetParent(content);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            Image image = go.AddComponent<Image>();
            image.preserveAspect = true;
            Button button = go.AddComponent<Button>();
            image.sprite = sprite;
            button.onClick.AddListener(()=> 
            { OnClickWeapenIcon?.Invoke(gun.id);
                  Destroy(weapenIcons[button.gameObject.name]);
                  weapenIcons.Remove(button.gameObject.name);
            });
            weapenIcons.Add(key, go);
      }
    
}

