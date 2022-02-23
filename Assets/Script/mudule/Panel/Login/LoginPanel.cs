using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoginPanel : BasePanel
{
      //账号输入框
      private InputField idInput;
      //密码输入框
      private InputField pwInput;
      //登陆按钮
      private Button loginBtn;
      //注册按钮
      private Button regBtn;

      private Transform accountRegist;
      //身份证名字
      private InputField nameInput;
      //身份证号码
      private InputField numberInput;
      //登陆按钮
      private Button okBt;
      //注册按钮
      private Button cancleBt;
      public Button otherBt;
      public override void OnInit()
      {
            skinPath = "LoginPanel";
            layer = PanelManager.Layer.Panel;
      }

      /// <summary>
      /// 当面板显示的时候
      /// </summary>
      /// <param name="para"></param>
      public override void OnShow(params object[] para)
      {
            //寻找组件
            idInput = TransformHelper.FindChiled(skin.transform, "IdInput").GetComponent<InputField>();
            pwInput = TransformHelper.FindChiled(skin.transform, "PwInput").GetComponent<InputField>();
            loginBtn = TransformHelper.FindChiled(skin.transform, "LoginBtn").GetComponent<Button>();
            regBtn = TransformHelper.FindChiled(skin.transform, "RigisterBtn").GetComponent<Button>();

            accountRegist = TransformHelper.FindChiled(skin.transform, "AccountRegist");
            //寻找组件
            nameInput = TransformHelper.FindChiled(skin.transform, "NameInput").GetComponent<InputField>();
            numberInput = TransformHelper.FindChiled(skin.transform, "NumberInput").GetComponent<InputField>();
            okBt = TransformHelper.FindChiled(skin.transform, "OkBt").GetComponent<Button>();
            cancleBt = TransformHelper.FindChiled(skin.transform, "CancleBt").GetComponent<Button>();
            otherBt = TransformHelper.FindChiled(skin.transform, "OtherBt").GetComponent<Button>();

            otherBt.onClick.AddListener(() => { OnLoginClick(0); PlayerPrefs.SetFloat("gameTime", 0); });
            cancleBt.onClick.AddListener(() => { accountRegist.gameObject.SetActive(false); });
            okBt.onClick.AddListener(() => {
                  if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(numberInput.text))
                  { PanelManager.Open<TipPanel>("请输入真确的身份证"); return; }
                  if(!IDCardValidation.CheckIDCard(numberInput.text))
                  { PanelManager.Open<TipPanel>("请输入真确的身份证"); return; }
                  
                  string outData;
                   NameCheck.Main(nameInput.text, numberInput.text, out outData);
                  Result result = IDCardValidation.SpriteResult(outData);
                  if(result.res==2)
                  {
                        PanelManager.Open<TipPanel>("验证失败");
                        return;
                  }
                  MsgAccountData msg = new MsgAccountData();
                  msg.id =idInput.text;
                  msg.accountData = outData;
                  Debug.Log(outData);
                  NetManager.Send(msg);
                  accountRegist.gameObject.SetActive(false);
            });
            //网络协议监听
            NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
            //网络事件监听
            NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
            NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFaild);
            NetManager.AddMsgListener("MsgAccountData", OnMsgAccountData);
            //连接服务器192.168.1.3 106.52.208.65
           NetManager.Connect("106.52.208.65", 132);
            //NetManager.Connect("192.168.1.9", 132);
            if(PlayerPrefs.HasKey("Count"))
            {
                  idInput.text = PlayerPrefs.GetString("Count");
            }
            if (PlayerPrefs.HasKey("PassWord"))
            {
                  pwInput.text = PlayerPrefs.GetString("PassWord");
            }
            loginBtn.onClick.AddListener(()=>OnLoginClick(1));
            regBtn.onClick.AddListener(OnRegClick);
      }
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
            NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFaild);
            NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
            NetManager.RemoveMsgListener("MsgAccountData", OnMsgAccountData);
      }
      private void  OnMsgAccountData(MsgBase msgBase)
      {
            MsgAccountData msg = (MsgAccountData)msgBase;
            if(msg.result == 1)
            {
                  PanelManager.Open<TipPanel>("实名认证成功");
            }
            else
            {
                  PanelManager.Open<TipPanel>("实名认证失败");
            }
      }
      //链接成功回调
      private void OnConnectSucc(string msg)
      {
            Debug.Log("ConnecSucc");
      }
      //链接失败回调
      private void OnConnectFaild(string msg)
      {
            Debug.Log(msg);
      }
      //收到登陆协议后
      private void OnMsgLogin(MsgBase msgBase)
      {
            MsgLogin msgLogin = (MsgLogin)msgBase;
            if (msgLogin.result == 0)
            {
                  PlayerPrefs.SetString("Count", msgLogin.id);
                  PlayerPrefs.SetString("PassWord", msgLogin.pw);
                  //打开房间列表界面
                  PanelManager.Open<MainPanel>();
                  //关闭界面
                  Close();
            }
            else if(msgLogin.result == -3)//没有实名认证
            {
                  accountRegist.gameObject.SetActive(true);
            }
            else if (msgLogin.result == -1)
            {
                  PanelManager.Open<NamePanel>(msgLogin.id);
                  Close();
            }
            else if(msgLogin.result == -2)
            {
                  PanelManager.Open<TipPanel>("请下载最新版本");
            }
            else
            {
                  PanelManager.Open<TipPanel>("登陆失败");
            }
      }
      //登陆按钮点击
      public void OnLoginClick(int isOher)
      {
            if (idInput.text == "" || pwInput.text == "")
            {
                  PanelManager.Open<TipPanel>("用户名或密码不能为空");
                  return;
            }
            MsgLogin msgLogin = new MsgLogin();
            msgLogin.isOther = isOher;
            msgLogin.id = idInput.text;
            msgLogin.pw = pwInput.text;
            msgLogin.version = Application.version;
            NetManager.Send(msgLogin);
      }
//注册按钮点击
public void OnRegClick()
      {
            PanelManager.Open<RegisterPanel>();
      }
}
