using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FPS.Character;
using AI.Perception;
using System.Collections;

namespace AI.FSM
{
      /// <summary>
      /// 状态机： 有限状态机核心类
      /// </summary>
      public class BaseFSM : MonoBehaviour
      {
            #region 1.0
            //字段
            private MonsterAnimator chAnim;
            private FSMState currentState;
            //当前状态
            public FSMstateID currentStateId;

            private FSMState defaultState;
            [Tooltip("默认状态")]
            public FSMstateID defaultStateId;
            //所有的状态对象
            private List<FSMState> states = new List<FSMState>();
            //方法
            public void ChangActiveState(FSMTriggerID triggerid)
            {
                  //1 根据当前条件 确定下一个状态是谁？待机》条件》死亡
                  var nextStateId = currentState.GetOutputState(triggerid);
                  //如果是None返回
                  if (nextStateId == FSMstateID.None)
                        return;
                  //如果不是None，继续判断是不是默认
                  FSMState nextState = null;//状态对象
                  if (nextStateId == FSMstateID.Default)
                  { nextState = defaultState; }
                  else
                  { nextState = states.Find(s => s.stateid == nextStateId); }
                  //2退出当前状态
                  currentState.ExitState(this);
                  currentState = nextState;//更新当前状态对象和编号
                  currentStateId = currentState.stateid;
                  //3进入下一个状态
                  currentState.EnterState(this);
            }
            #endregion
            #region 2.0
            //字段
            public string aiConfigFile = "AI_01.txt";//AI配置文件
            public AnimationParams animParams;//?
            [HideInInspector]
            public BasePlayer chState;
            private Coroutine coroutine;
            //方法
            private void Awake()
            {
                  ConfigFSM();
            }

            private void ChState_HpCostHandle(int damage)
            {
                  if(coroutine!=null)
                  {
                        StopCoroutine(coroutine);
                  }
                  movespeedOffset =0.5f;
                  coroutine = StartCoroutine(ReBackSpeed());
            }
            IEnumerator ReBackSpeed()
            {
                  float timecount = 1;
                  while(timecount>0)
                  {
                        timecount -= Time.deltaTime;
                        yield return null;
                  }
                  movespeedOffset = 1;
            }
            //调用AI配置文件 确定 条件 状态的映射关系
            private void ConfigFSM()
            {
                  #region 方法1 硬编码 修改量大，代码复用性差
                  /*
                  IdleState idleState = new IdleState();
                  idleState.AddTrigger(FSMTriggerID.NoHealth, FSMstateID.Dead);
                  idleState.AddTrigger(FSMTriggerID.SawPlayer, FSMstateID.Pursuit);
                  states.Add(idleState);

                  DeadState deadState = new DeadState();
                  states.Add(deadState);

                  PursuitState pursuit = new PursuitState();
                  pursuit.AddTrigger(FSMTriggerID.NoHealth, FSMstateID.Dead);
                  pursuit.AddTrigger(FSMTriggerID.ReachPlayer, FSMstateID.Attackting);
                  pursuit.AddTrigger(FSMTriggerID.LosePlayer, FSMstateID.Default);
                  states.Add(pursuit);
                  */
                  #endregion
                  #region 使用AI配置文件
                  var dic = AIConfigurationReader.Load(aiConfigFile);
                  foreach (var stateName in dic.Keys)
                  {

                        //1 创建状态对象
                        var type = Type.GetType("AI.FSM." + stateName + "State");
                        var stateObj = Activator.CreateInstance(type) as FSMState;

                        //2 添加映射条件
                        foreach (var triggerId in dic[stateName].Keys)
                        {
                              var trigger = (FSMTriggerID)(Enum.Parse(typeof(FSMTriggerID), triggerId));
                              var state = (FSMstateID)(Enum.Parse(typeof(FSMstateID), dic[stateName][triggerId]));
                              stateObj.AddTrigger(trigger, state);
                        }
                        states.Add(stateObj);
                  }
                  #endregion
            }
            /// <summary>
            /// 指定默认状态
            /// </summary>
            private void InitDefaultState()
            {
                  //根据属性窗口指定的 默认状态编号 为其它三个字段赋值
                  defaultState = states.Find(s => s.stateid == defaultStateId);
                  currentState = defaultState;
                  currentStateId = currentState.stateid;
            }
            private void OnEnable()
            {
                  InitDefaultState();
                  //临时调用
                  //InvokeRepeating("ResetTarget", 0, 0.2f);//3.0-1
            }
            private void OnDisable()
            {
                  //CancelInvoke("ResetTarget");
                  //状态机禁用时如果不为死亡状态》ildle状态 如果为死亡状态 感应器禁用
                  if (currentStateId != FSMstateID.Dead)
                  {
                        currentState.ExitState(this);
                        currentState = states.Find(s => s.stateid == FSMstateID.Idle);
                        currentStateId = currentState.stateid;
                        PlayAnimation(animParams.Idle);
                  }
                  else
                  {
                        var sensors = GetComponents<AbstractSensor>();
                        foreach (var sensor in sensors)
                        {
                              sensor.enabled = false;
                        }
                  }
            }
            public void PlayAnimation(string animPara)
            {

                  chAnim.SetBool(animPara);
            }
            public void Start()
            {
                  chSkillSys = GetComponent<CharacterSkillSystem>();
                  chState = GetComponent<BasePlayer>();
                  chAnim = GetComponent<MonsterAnimator>();
                  navAgent = GetComponent<NavMeshAgent>();//3.0-2
                  sightSensor = GetComponent<SightSensor>();
                  chState.HpCostHandle += ChState_HpCostHandle;
                  if (sightSensor != null)
                  {
                        sightSensor.sightDistance = sightDistance;
                        sightSensor.OnPerception += SightSensor_OnPerception;
                        sightSensor.OnNonPerception += SightSensor_OnNonPerception;
                  }
            }

            private void Update()
            {
                  currentState.Reason(this);
                  currentState.Action(this);
            }
            #endregion
            #region 3.0 ARPGDemo3.2 添加待机到发现目标》追逐 需要的字段和功能
            [Tooltip("目标标签")]
            public string[] targetTags = { "Player" };
            
            //关注的目标物体
            public Transform targetObject = null;
            [Tooltip("视距-发现目标最远距离")]
            public float sightDistance = 10;
           [Tooltip("在追逐状态下跑的速度")]
            public float moveSpeed = 3;
            //寻路 寻路组件网格寻路
            private NavMeshAgent navAgent;
            public float skillDiatance;
            //方法
            /// <summary>
            /// //1 重置目标 6.0中替换为感知系统
            /// </summary>  
            private void ResetTarget()
            {
                  //获取所有tag为 skillData.tags的对象
                  List<GameObject> listTargets = new List<GameObject>();
                  foreach (var tag in targetTags)
                  {
                        var targets = GameObject.FindGameObjectsWithTag(tag);
                        if (targets != null || targets.Length != 0)
                        { listTargets.AddRange(targets); }
                  }
                  if (listTargets.Count == 0)
                        return;
                  //找出一定距离中的敌人 hp不为0
                  var enemys = listTargets.FindAll(a => (Vector3.Distance(transform.position,
                      a.transform.position) <= sightDistance) && (a.GetComponent<BasePlayer>().Hp > 0));
                  if (enemys == null || enemys.Count == 0)
                  {
                        return;
                  }
                  //取最近的
                  targetObject = ArrayHelper.Min(enemys.ToArray(), a => (Vector3.Distance(transform.position, a.transform.position))).transform;

            }
            public float movespeedOffset = 1;
            //2 向目标跑，移向目标
            /// <summary>

            /// 2 向目标跑，移向目标
            /// </summary>
            /// <param name="pos">目标当前的位置</param>
            /// <param name="speed">跑的速度</param>
            /// <param name="stopDista">停止距离</param>
            public void MoveToTarget(Vector3 pos, float speed, float stopDistance)
            {
                  navAgent.speed = speed* movespeedOffset;
                  navAgent.stoppingDistance = stopDistance;
                  navAgent.SetDestination(pos);
            }
            /// <summary>
            /// //3 停止移动
            /// </summary>
            public void StopMove()
            {
                  navAgent.enabled = false;
                  navAgent.enabled = true;
            }
            #endregion
            #region 4.0
            private CharacterSkillSystem chSkillSys;
            public void AutoUseSkill()
            {
                 
                  if(Vector3.Distance( targetObject.position,transform.position)>chState.attackDistance)
                  {
                        chSkillSys.AttackUseeSkill(10);
                  }
                  else
                  {
                        chSkillSys.AttackUseeSkill(1);
                  }
            }
            #endregion
            #region 5.0 定义寻路需要字段和方法
            [Tooltip("巡逻的路点")]
            public Transform[] wayPoints;
            [Tooltip("在离目标点x米时停下")]
            public float patrolArrivalDistance = 1;
            [Tooltip("移动速度")]
            public float walkSpeed = 1.2f;
            //是否完成巡逻
            [HideInInspector]
            public bool IsPatrolComplete = false;
            [Tooltip("巡逻方式")]
            public PatrolMode patrolMode = PatrolMode.Once;
            #endregion
            #region 6.0 引入智能感应器
            private SightSensor sightSensor;
            private void SightSensor_OnNonPerception()
            {
                  targetObject = null;
            }
            private void SightSensor_OnPerception(List<AbstractTrigger> obj)
            {
                  //1 找出targetTags标签中的目标
                  var tempList = obj.FindAll(o => Array.IndexOf(targetTags, o.tag) >= 0);
                  if (tempList.Count > 0)
                  {
                        //找出生命值大于0的目标
                        tempList = tempList.FindAll(o => o.GetComponent<BasePlayer>().Hp > 0);
                        if (tempList.Count > 0)
                        {
                              //找出距离自身最近的目标
                              targetObject = ArrayHelper.Min(tempList.ToArray(), o => Vector3.Distance(o.transform.position, transform.position)).transform;
                        }
                  }
            }
            #endregion
      }
}
