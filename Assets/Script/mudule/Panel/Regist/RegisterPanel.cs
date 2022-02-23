using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
      //账号输入框
      private InputField idInput;
      //密码输入框
      private InputField pwInput;
      //重复密码输入框
      private InputField repInput;
      //注册按钮
      private Button regBtn;
      //关闭按钮
      private Button closeBtn;
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
      }

      public override void OnInit()
      {
            skinPath = "RegisterPanel";
            layer = PanelManager.Layer.Panel;
      }
      //当显示的时候
      public override void OnShow(params object[] para)
      {
            idInput = TransformHelper.FindChiled(skin.transform, "IdInput").GetComponent<InputField>();
            pwInput = TransformHelper.FindChiled(skin.transform, "PwInput").GetComponent<InputField>();
            repInput = TransformHelper.FindChiled(skin.transform, "RepInput").GetComponent<InputField>();
            closeBtn = TransformHelper.FindChiled(skin.transform, "CloseBtn").GetComponent<Button>();
            regBtn = TransformHelper.FindChiled(skin.transform, "RegisterBtn").GetComponent<Button>();
            NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
            //按钮添加监听
            regBtn.onClick.AddListener(OnRegClick);
            closeBtn.onClick.AddListener(OnCloseClick);
      }
      //收到注册协议
      private void OnMsgRegister(MsgBase msgBase)
      {
            MsgRegister msgRegister = (MsgRegister)msgBase;
            if (msgRegister.result == 0)
            {
                  Debug.Log("注册成功");
                  //提示
                  PanelManager.Open<TipPanel>("注册成功");
                  Close();
            }
            else
            {
                  PanelManager.Open<TipPanel>("注册失败");
            }
      }
      //按下注册按钮
      private void OnRegClick()
      {
            if (idInput.text.Length <= 2||pwInput.text.Length<=2)
            {
                  PanelManager.Open<TipPanel>("用户名或密码太短");
                  return;
            }
            //如果密码或者账号为空
            if(string.IsNullOrEmpty(idInput.text)||string.IsNullOrEmpty(pwInput.text))
            {
                  PanelManager.Open<TipPanel>("用户名和密码不能为空");
                  return;
            }
            //如果两次密码不同
            if(pwInput.text!= repInput.text)
            {
                  PanelManager.Open<TipPanel>("两次输入的密码不同");
            }
            //发送协议
            MsgRegister msgRegister = new MsgRegister();
            msgRegister.id = idInput.text;
            msgRegister.pw = pwInput.text;
            NetManager.Send(msgRegister);
      }
      //按下关闭按钮
      private void OnCloseClick()
      {
            Close();
      }
}
