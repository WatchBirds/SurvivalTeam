using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.item;
namespace FPS.Character
{
      /// <summary>
      /// 角色运动控制
      /// </summary>
      public class PlayerMotor : MonoBehaviour
      {
            /// <summary>
            /// 奔跑速度
            /// </summary>
            public float runSpeed = 2.3f;
            private float defalueRunSpeed;//默认奔跑速度
            /// <summary>
            /// 移动速度
            /// </summary>
            public float moveSpeed = 1.5f;
            private float defaluemoveSpeed;//默认移动速度
            /// <summary>
            /// 转向速度
            /// </summary>           
            public float rotationSpeed = 0.5f;
            /// <summary>
            /// 跳跃力
            /// </summary>
            public float jumpForce = 180;
            public float currentSpeed;//当前速度
            private CharacherAnimator handlecs;
            //动画参数值
            private float right;
            private float foward;
            private float hand;
            //角色刚体
            private Rigidbody rig;
            //射线半径
            private float radius;
            [Header("音频")]
            public AudioSource audioSource;
            public AudioClip walkClip;
            public AudioClip runClip;
            //角色空中时间
            private float hightTime;

            public Transform checkStart;
            public Transform checkEnd;
            public void Start()
            {
                  if(audioSource == null)
                  { audioSource = GetComponent<AudioSource>(); }
                  defaluemoveSpeed = moveSpeed;
                  defalueRunSpeed = runSpeed;
                  currentSpeed = moveSpeed;
                  handlecs = GetComponent<CharacherAnimator>();
                  rig = GetComponent<Rigidbody>();
                  WeaPenMana.OnAim += OnPLayerAim;
                  WeaPenMana.OnCancleAim += OnCancleAim;
                  InputControl.OnMove += Move;
                  InputControl.OnJumpBtdown += InputControl_OnJumpBtdown;
                  radius = transform.GetComponent<CapsuleCollider>().radius;
                  InvokeRepeating("SendMsg", 0, 0.1f);//同步位置
            }
            private void OnDestroy()
            {
                  WeaPenMana.OnAim -= OnPLayerAim;
                  WeaPenMana.OnCancleAim -= OnCancleAim;
                  InputControl.OnMove -= Move;
                  InputControl.OnJumpBtdown -= InputControl_OnJumpBtdown;
                  CancelInvoke("SendMsg");
            }
            private void OnEnable()
            {
                  InvokeRepeating("JumpCheckUpdate", 0, 0.25f);
            }
            private void OnDisable()
            {
                  CancelInvoke("JumpCheckUpdate");
            }
            //玩家瞄准事件监听
            private void OnPLayerAim(GunItem gun)
            {
                  runSpeed *= gun.gunData.aimPlayerSpeed;
                  moveSpeed*= gun.gunData.aimPlayerSpeed;
            }
            //取消瞄准时事件监听
            private void OnCancleAim(GunItem gun)
            {
                  runSpeed = defalueRunSpeed;
                  moveSpeed = defaluemoveSpeed;
            }
            //玩家移动
            public void Move(Vector2 vector)
            {
                  right = vector.x;
                  foward = vector.y;
                  if (right == 0 && foward == 0)//角色停止
                  {
                        handlecs.SetFloat("speed", 0);
                        audioSource.Stop();
                        return;
                  }
                  if (foward < 0)//角色后退
                  {
                        audioSource.clip = walkClip;
                        audioSource.pitch = 1f;
                        currentSpeed = moveSpeed * 0.8f;
                  }
                  else if ((right * right + foward * foward) >= 0.81&& foward > 0)//角色处于向前奔跑
                  {
                        audioSource.clip = runClip;
                        currentSpeed = runSpeed;
                        audioSource.pitch = 1.3f;
                  }   
                  else//角色先前走
                  {
                        audioSource.clip = walkClip;
                        currentSpeed = moveSpeed;
                        audioSource.pitch = 1f;
                  }
                  //播放音频
                  if (!audioSource.isPlaying)
                  {
                        audioSource.Play();
                  }
                  //播放动画
                  handlecs.SetFloat("speed", foward);
                  Vector3 tarPos = new Vector3(right, 0, foward);
                  transform.Translate(tarPos.normalized * currentSpeed * Time.deltaTime, Space.Self);
            }
            //角色上下望动画
            public void PlayerWatch(float value)
            {
                  hand = value;
                  handlecs.SetFloat(handlecs.floatParameter.hand, value);
            }
            private float jumptime;
            //跳跃按钮事件监听
            public void InputControl_OnJumpBtdown()
            { 
                  if(handlecs.GetBool("jump")||(Time.time-jumptime)<1f)
                  {     
                        return;
                  }
                  jumptime = Time.time;
                  rig.AddForce(Vector3.up*jumpForce);
                  handlecs.SetBool("jump", true);
            }
            //发送位置协议
            private void SendMsg()
            {
                  //发送MsgSyncPlayer协议
                  Vector3 pos = transform.position;
                  Vector3 eul = transform.forward;
                  MsgSyncPlayer msg = new MsgSyncPlayer();
                  msg.x = pos.x;
                  msg.y = pos.y;
                  msg.z = pos.z;
                  msg.ex = eul.x;
                  msg.ey = eul.y;
                  msg.ez = eul.z;
                  msg.time = Time.time;
                  msg.foward = foward;
                  msg.right = right;
                  msg.hand = hand;
                  NetManager.Send(msg);
            }
            public float offset;
            //玩家空中检测
            private void JumpCheckUpdate()
            {
                  if (Physics.CheckCapsule(checkStart.position, checkEnd.position , radius / 2, LayerMask.GetMask("Build")))
                  {
                        MsgHitPlayer msgHit = new MsgHitPlayer();
                        msgHit.hitId = GameMain.id;
                        msgHit.damage = 5;
                        NetManager.Send(msgHit);
                  }
                  Vector3 pos1 = transform.position+Vector3.down*offset;
                  Vector3 pos2 = transform.position + Vector3.up;
                  Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, radius,LayerMask.GetMask("Build")|LayerMask.GetMask("BulletHit"));    
                  if (colliders != null && colliders.Length > 0)
                  {
                        handlecs.SetBool("jump", false);
                        //发送跳跃协议
                        MsgJump msg = new MsgJump();
                        msg.state = 1;
                        NetManager.Send(msg);
                        if (hightTime >= 0.1)//在空中时间大于0.1秒时
                        {
                              //每0.01秒4点伤害
                              float hittime = Mathf.Clamp(hightTime, 0.07f, 0.3f);
                              //发送角色受击协议
                              MsgHitPlayer msgHit = new MsgHitPlayer();
                              msgHit.hitId = GameMain.id;
                              msgHit.damage = (int)(hittime/0.01 * 4);
                              NetManager.Send(msgHit);
                        }
                        hightTime = 0;
                  }
                  else
                  { 
                        handlecs.SetBool("jump", true);
                        //发送跳跃协议
                        MsgJump msg = new MsgJump();
                        msg.state = 0;
                        NetManager.Send(msg);
                        //MsgMotionBool msg = new MsgMotionBool();
                        //msg.paraName = "jump";
                        //msg.result = 0;
                        //NetManager.Send(msg);
                        handlecs.SetBool("aim", false);
                        hightTime += Time.deltaTime;
                  }
            }
      }
}