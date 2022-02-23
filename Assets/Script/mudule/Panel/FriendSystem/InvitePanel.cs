using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class InvitePanel : BasePanel
{

      private Animator animator;
      private Image hdImage;
      private Button acceptBt;
      private Button refusedBt;
      private Text nameText;
      private Text countText;
      private bool isCloseing =false ;
      public override void OnInit()
      {
            skinPath = "InvitePanel";
            layer = PanelManager.Layer.Tip;
      }
      public override void OnShow(params object[] para)
      {
            MsgInviteFriend msg = (MsgInviteFriend)para[0];
            //查找组件
            animator = skin.GetComponent<Animator>();
            hdImage = TransformHelper.FindChiled(skin.transform, "HdImage").GetComponent<Image>();
            acceptBt = TransformHelper.FindChiled(skin.transform, "AcceptBt").GetComponent<Button>();
            refusedBt = TransformHelper.FindChiled(skin.transform, "RefusedBt").GetComponent<Button>();
            nameText = TransformHelper.FindChiled(skin.transform, "NameText").GetComponent<Text>();
            countText = TransformHelper.FindChiled(skin.transform, "CountText").GetComponent<Text>();
            //赋值
            Debug.Log(msg.friendData.sprite);
            hdImage.sprite = Resources.Load<Sprite>("Ui/HeadSprites/" + msg.friendData.sprite);
            nameText.text = string.Format("<color=yellow>{0}</color>想和你一起玩", msg.name);
            StartCoroutine(DelayFunction(msg));
            StartCoroutine(AutoClose(10));
      }
      IEnumerator AutoClose(float time)
      {
            while(time>=0)
            {
                  time -= Time.deltaTime;
                  countText.text =("倒计时结束自动拒绝：" + time.ToString("0")).ToString();
                  yield return null;
            }
            Func();
      }
      IEnumerator DelayFunction(MsgInviteFriend inviteFriend)
      {
            animator.Play("Show State");
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            while(length>=0)
            {
                  length -= Time.deltaTime;
                  yield return null;
            }
            acceptBt.onClick.AddListener(() =>
            {
                  //发协议
                  MsgEnterRoom msg = new MsgEnterRoom();
                  msg.id = inviteFriend.roomId;
                  NetManager.Send(msg);
                  Func();
            });
            refusedBt.onClick.AddListener(Func);
      }
      private void Func()
      {
            if(isCloseing)
            { return; }
            isCloseing = true;
            animator.Play("Back State");
            StartCoroutine(DelayClose());
      }
      IEnumerator DelayClose()
      {
            float closeTime = animator.GetCurrentAnimatorStateInfo(0).length;
            while(closeTime>=0)
            {
                  closeTime -= Time.deltaTime;
                  yield return null;
            }
            Close();
      }
      public override void OnClose()
      {
            StopAllCoroutines();
      }
}
