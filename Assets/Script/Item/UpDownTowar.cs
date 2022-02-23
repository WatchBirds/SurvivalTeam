using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class UpDownTowar : MonoBehaviour,Iinteraction
{
      public Animator animator;
      public string landState = "LandState";
      public string backState = "BackState";
      private string currentState;
      /// <summary>
      /// 交互显示的文本
      /// </summary>
      public string Str { get => str; }
      [SerializeField]
      private string str = "使用电梯";
      public bool BeCheck { get => beCheck; set => beCheck = value; }
      private bool beCheck = true;
      private void Start()
      {
            currentState = landState;
            //添加监听
            NetManager.AddMsgListener("MsgUpDownTower", OnMsgUpDownTower);
      }
      private void OnMsgUpDownTower(MsgBase msgBase)
      {
            MsgUpDownTower msg = (MsgUpDownTower)msgBase;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("DefalueState") || (stateInfo.IsName("LandDefalueState")))
             {
                  animator.Play(msg.state);
                  currentState = msg.state;
            }
            else
            { return; }
            if (currentState == landState)
            {
                  currentState = backState;
            }
            else
            {
                  currentState = landState;
            }
      }
      private void OnDestroy()
      {
            NetManager.RemoveMsgListener("MsgUpDownTower", OnMsgUpDownTower);
      }

      public void Use(GameObject go)
      {
            if (animator == null)
            { return; }
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("DefalueState") || stateInfo.IsName("LandDefalueState"))
            {
                  //发送协议
                  MsgUpDownTower msg = new MsgUpDownTower();
                  msg.state = currentState;
                  NetManager.Send(msg);
                  //if(currentState == landState)
                  //{
                  //      currentState = backState;
                  //}
                  //else
                  //{
                  //      currentState = landState;
                  //}
            }
      }
}

