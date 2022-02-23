using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPS.item;
using UnityEngine;

public class BulletBox: MonoBehaviour,Iinteraction
{
      [Tooltip("盒子内所有字弹")]
      public int boxAllBullet;
      public string boxid;
      private int temp;
      [Tooltip("补充子弹时间")]
      public float reloadTime;
      public TextMesh text;
      [Tooltip("使用间隔时间")]
      public float useDetime;
      private float lastUseTime;
      /// <summary>
      /// 交互显示的文本
      /// </summary>
      public string Str { get => str; }
      [SerializeField]
      private string str = "补充弹药";
      public bool BeCheck { get => beCheck; set => beCheck = value; }
      private bool beCheck = true;
      private void Start()
      {
            temp = boxAllBullet;
            text.text = ("剩余" + boxAllBullet).ToString();
            NetManager.AddMsgListener("MsgGetBullet", OnMsgGetBullet);
      }
      public void Use(GameObject target)
      {
            float curretnTime = Time.time;
            if (curretnTime - lastUseTime <= 1.8f)
            { return; }

            int getBullet = Mathf.Clamp(boxAllBullet, 0, 100);
            if (getBullet == 0)
            { return; }
            //发送协议
            MsgGetBullet msg = new MsgGetBullet();
            msg.bulletNumber = getBullet;
            msg.boxid = boxid;
            NetManager.Send(msg);
      }
      public void OnMsgGetBullet(MsgBase msgBase)
      {
            Debug.Log("resulve get bullet");
            MsgGetBullet msg = (MsgGetBullet)msgBase;
            if ( msg.boxid!=boxid)
            { Debug.Log("bullet msg.id!=:" + msg.boxid); return; }
            boxAllBullet -= msg.bulletNumber;
            if (boxAllBullet<=0)
            { StartCoroutine(Reload()); }
            text.text = ("剩余" + boxAllBullet).ToString();
            if (msg.id == GameMain.id)
            {
                  WeaPenMana gunMana = BattleManager.instance.GetCtrlPlayer().GetComponent<WeaPenMana>();
                  gunMana.GetBullet(msg.bulletNumber);
            }
            lastUseTime = Time.time;
      }
      IEnumerator Reload()
      {
            string textString = "正在补充剩余时间 ";
            float countTime = reloadTime;
            while (countTime>0)
            {
                  text.text = textString + countTime.ToString("0.0");
                  countTime -= Time.deltaTime;
                  yield return null;
            }
            boxAllBullet = temp;
            text.text = ("剩余" + boxAllBullet).ToString();
      }
      private void OnDestroy()
      {
            NetManager.RemoveMsgListener("MsgGetBullet", OnMsgGetBullet);
      }  
}
