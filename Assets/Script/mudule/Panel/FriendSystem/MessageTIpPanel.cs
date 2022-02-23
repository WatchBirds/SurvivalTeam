using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 消息提醒面板 需要MsgAddFriend参数
/// </summary>
public class MessageTIpPanel : BasePanel
{
      private Animator animator;
      private Button acceptBt;
      private Button refuseBt;
      private Text message;
      public override void OnInit()
      {
            skinPath = "MessageTIpPanel";
            layer = PanelManager.Layer.Tip;
      }

      public override void OnShow(params object[] para)
      {
            MsgAddFriend msg = (MsgAddFriend)para[0];
            //获取组件
            animator =skin.GetComponent<Animator>();
            acceptBt = TransformHelper.FindChiled(skin.transform, "AcceptBt").GetComponent<Button>();
            refuseBt = TransformHelper.FindChiled(skin.transform, "RefuseBt").GetComponent<Button>();
            message = TransformHelper.FindChiled(skin.transform, "Message").GetComponent<Text>();
            //赋值
            message.text = string.Format("<color=yellow>{0}</color>请求添加你为好友", msg.name);
            animator.Play("GetMessageState");
            StartCoroutine(DelayAddListener(msg.name));
      }
      IEnumerator DelayAddListener(string targetId)
      {
            float lenth = animator.GetCurrentAnimatorStateInfo(0).length;
            while(lenth>=0)
            {
                  lenth -= Time.deltaTime;
                  yield return null;
            }
            //添加监听
            acceptBt.onClick.AddListener(() => {
                  MsgFriendResult msgResult = new MsgFriendResult();
                  msgResult.result = 1;
                  msgResult.targetName = targetId;
                  NetManager.Send(msgResult);
                  acceptBt.enabled = false; //禁用按钮只允许点击一次
                  animator.Play("BackOfState");
                  StartCoroutine(DeLayClose());
            });
            refuseBt.onClick.AddListener(() => {
                  MsgFriendResult msgResult = new MsgFriendResult();
                  msgResult.result = 0;
                  NetManager.Send(msgResult);
                  acceptBt.enabled = false; //禁用按钮只允许点击一次
                  animator.Play("BackOfState");
                  StartCoroutine(DeLayClose());
            });
      }
      IEnumerator DeLayClose()
      {
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            while(length>=0)
            {
                  yield return null;
                  length -= Time.deltaTime;
            }
            Close();
      }
      public override void OnClose()
      {
            StopAllCoroutines();
      }

}
