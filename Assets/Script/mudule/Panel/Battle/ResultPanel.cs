using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultPanel : BasePanel
{
      private GameObject resultData;
      private Transform poss;
      private Text resultText;
      private Button backToMainBt;
      private Button backToRoomBt;
      private int offset;
      private int otherCoin;
      public override void OnInit()
      {
            skinPath = "ResultPanel";
            layer = PanelManager.Layer.Panel;
      }
      public override void OnClose()
      {
      }
      public override void OnShow(params object[] para)
      {
            InputControl  input = (InputControl)para[1];
            MsgBattleResult msg = (MsgBattleResult)para[0];
            //寻找组件
            resultData = TransformHelper.FindChiled(skin.transform, "ResultData").gameObject;
            poss = TransformHelper.FindChiled(skin.transform, "Poss");
            resultText = TransformHelper.FindChiled(skin.transform, "ResultText").GetComponent<Text>();
            backToMainBt = TransformHelper.FindChiled(skin.transform, "BackToMainBt").GetComponent<Button>();
            backToRoomBt = TransformHelper.FindChiled(skin.transform, "BackToRoomBt").GetComponent<Button>();
            //按钮绑定事件
            backToMainBt.onClick.AddListener(() => { MsgLeaveBattle msgleave = new MsgLeaveBattle();  NetManager.Send(msgleave); });
            backToRoomBt.onClick.AddListener(() => { MsgBackToRoom msgBack = new MsgBackToRoom();  NetManager.Send(msgBack); });
            //赋值
            if (msg.result == -1)
            {
                  resultText.text = "游戏胜利额外获得金币200";
                  offset = 1;
                  otherCoin = 200;
            }
            else
            {
                  resultText.text = "任务失败(获取金币数减半)";
                  offset = 2;
                  otherCoin = 0;
            }
            int idx = 0;
            foreach(string id in input.countDic.Keys )
            {
                  GameObject go = Instantiate(resultData);
                  go.gameObject.SetActive(true);
                  go.transform.SetParent(poss);
                  go.transform.localPosition = Vector3.zero;
                  go.transform.localScale = Vector3.one;
                  GenerateResultData(go, id,input.countDic[id],input.coinDic[id]/offset+ otherCoin);
                  idx++;
            }
      }
      private void GenerateResultData(GameObject go,string id,int number,int coin)
      {
            //获取组件
            Text playeridtext = TransformHelper.FindChiled(go.transform, "PlayerIdText").GetComponent<Text>();
            Text killcounttext = TransformHelper.FindChiled(go.transform, "KillCountText").GetComponent<Text>();
            Text getcoretext = TransformHelper.FindChiled(go.transform, "GetCoreText").GetComponent<Text>();
            //赋值
            playeridtext.text = id;
            killcounttext.text = number.ToString();
            getcoretext.text = coin.ToString();
            if(id !=GameMain.id)
            { return; }
            //发送获取金币协议
            MsgGetCoin getCoin = new MsgGetCoin();
            getCoin.coin = coin;
            NetManager.Send(getCoin);
      }
}
