using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FPS.item;
public class StorePanel : BasePanel
{
      //关闭按钮
      private Button closeBt;
      //商品父物体
      private Transform content;
      //模型父物体
      private Transform gunMoldPlace;
      //商品预制体
      private GameObject item;
      //枪的图片
      private Sprite[] gunicons;
      //金币文本
      private Text coinText;
      //玩家拥有的枪名称
      private string[] guns;
      //枪信息父物体
      private Transform gunData;
      //伤害
      private Slider damageSlider;
      //射速
      private Slider shotSpeedSlider;
      //弹容量
      private Slider magBulletSlider;
      //后座力
      private Slider offsetSlider;
      public override void OnInit()
      {
            skinPath = "StorePanel";
            layer = PanelManager.Layer.Panel;
      }
      public override void OnShow(params object[] para)
      {
            //寻找组件
            closeBt = TransformHelper.FindChiled(skin.transform, "CloseBt").GetComponent<Button>();
            content = TransformHelper.FindChiled(skin.transform, "Scroll View/Viewport/Content");
            gunMoldPlace = TransformHelper.FindChiled(skin.transform, "GunMoldPlace");
            item = TransformHelper.FindChiled(skin.transform, "Item").gameObject;
            gunicons = Resources.LoadAll<Sprite>("Ui/GunSprite/Gun_Icons_Spritesheet");
            coinText = TransformHelper.FindChiled(skin.transform, "CoinText").GetComponent<Text>();
            gunData = TransformHelper.FindChiled(skin.transform, "GunData");
            damageSlider = TransformHelper.FindChiled(gunData, "DamageSlider").GetComponent<Slider>();
            shotSpeedSlider = TransformHelper.FindChiled(gunData, "ShotSpeedSlider").GetComponent<Slider>();
            magBulletSlider = TransformHelper.FindChiled(gunData, "MagBulletSlider").GetComponent<Slider>();
            offsetSlider = TransformHelper.FindChiled(gunData, "OffsetSlider").GetComponent<Slider>();
            //赋值
            closeBt.onClick.AddListener(() => { PanelManager.Open<MainPanel>(); Close(); });
            //添加网络协议监听
            NetManager.AddMsgListener("MsgGetStoreData", OnMsgGetStoreData);
            NetManager.AddMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.AddMsgListener("MsgBuyProduct", OnMsgBuyProduct);
            NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.AddMsgListener("MsgInviteFriend", OnMsgInviteFriend);
            //发送获取玩家信息协议
            MsgGetPlayerData msgGet = new MsgGetPlayerData();
            NetManager.Send(msgGet);
      }
      public override void OnClose()
      {
            NetManager.RemoveMsgListener("MsgGetStoreData", OnMsgGetStoreData);
            NetManager.RemoveMsgListener("MsgGetPlayerData", OnMsgGetPlayerData);
            NetManager.RemoveMsgListener("MsgBuyProduct", OnMsgBuyProduct);
            NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
            NetManager.RemoveMsgListener("MsgInviteFriend", OnMsgInviteFriend);
      }
      private void OnMsgInviteFriend(MsgBase msgBase)
      {
            MsgInviteFriend msg = (MsgInviteFriend)msgBase;
            if (msg.friendData == null)
            { Debug.Log("frienddata null GameMain"); return; }
            PanelManager.Open<InvitePanel>(msg);
      }
      //收到进入房间协议
      private void OnMsgEnterRoom(MsgBase msgBase)
      {
            MsgEnterRoom msgEnter = (MsgEnterRoom)msgBase;
            //成功进入房间
            if (msgEnter.result == 0)
            {
                  PanelManager.Open<RoomPanel>();
                  Close();
            }
            else
            {
                  PanelManager.Open<TipPanel>("进入房间失败");
            }
      }
      //收到玩家信息协议
      private void OnMsgGetPlayerData(MsgBase msgBase)
      {
            MsgGetPlayerData msg = (MsgGetPlayerData)msgBase;
            coinText.text = msg.playerData.coin.ToString();
            guns = msg.playerData.guns;
            //发送查询商店数据协议
            MsgGetStoreData msgs = new MsgGetStoreData();
            msgs.productType = ProductType.weapen.ToString();
            NetManager.Send(msgs);
      }
      //收到查询商店数据协议
      private void OnMsgGetStoreData(MsgBase msgBase)
      {
            MsgGetStoreData msg = (MsgGetStoreData)msgBase;
            for (int i = 0; i < content.childCount; i++)
            {
                  Destroy(content.GetChild(i).gameObject);
            }
            //生成商品
            for (int i = 0; i < msg.products.Length; i++)
            {

                  GenerateProduct (msg.products[i]);
            }
      }
      //收到买物品协议
      private void OnMsgBuyProduct(MsgBase msgBase)
      {
            MsgBuyProduct msg = (MsgBuyProduct)msgBase;
            if(msg.result == 0)
            {
                  PanelManager.Open<TipPanel>("购买失败");
                  return;
            }
            else if(msg.result == 1)
            {
                  //发送获取玩家信息协议
                  MsgGetPlayerData msgGet = new MsgGetPlayerData();
                  NetManager.Send(msgGet);
            }
      }
      //生成一个产品信息
      private void GenerateProduct(ProductData productData)
      {
            GameObject product = Instantiate(item);
            //购买按钮
            Transform buyBt = TransformHelper.FindChiled(product.transform, "BuyBt");
            Button buyButton = buyBt.GetComponent<Button>();
            Text buyText = buyBt.GetComponentInChildren<Text>();
            //枪图标
            Transform gunImage = TransformHelper.FindChiled(product.transform, "GunImage");
            Image image = gunImage.GetComponent<Image>();
            Button gunBt = gunImage.GetComponent<Button>();
            Transform have = TransformHelper.FindChiled(product.transform, "Have");
            //背景框
            Transform fram = TransformHelper.FindChiled(product.transform, "Fram");
            //赋值
            product.transform.SetParent(content);
            product.transform.localPosition = Vector3.zero;
            product.transform.localScale = Vector3.one;
            product.SetActive(true);
            Sprite gunIcon = ArrayHelper.Find(gunicons, a => a.name == productData.iconName);
            gunBt.onClick.AddListener(() => { CreatMold(productData.id);  gunData.gameObject.SetActive(true); });//创建模型 显示信息
            if (gunIcon != null)
            {
                  image.sprite = gunIcon;
            }
            //如果玩家已经购买了
            if(ArrayHelper.Find(guns,a=>a == productData.id) !=null)
            {
                  have.gameObject.SetActive(true);
                  buyText.text = "已购买";
            }
            else
            {
                  buyText.text = productData.price.ToString();
                  buyText.color = Color.yellow;
                  buyButton.onClick.AddListener(() => { MsgBuyProduct buyMsg = new MsgBuyProduct(); buyMsg.productName = productData.id;NetManager.Send(buyMsg); });
            }
      }
      //创建模型
      private void CreatMold(string gunname)
      {
            //判断是否有子物体
            if(gunMoldPlace.childCount>0)
            { Destroy(gunMoldPlace.GetChild(0).gameObject); }
            //加载一个模型
            GameObject moldprefab = ResourceManager.Load<GameObject>(gunname);
            GameObject mold = Instantiate(moldprefab);
            mold.transform.SetParent(gunMoldPlace);
            mold.transform.localPosition = Vector3.zero;
            mold.transform.localEulerAngles = Vector3.zero;
            //传递信息
            GunItem gun = mold.GetComponent<GunItem>();
            damageSlider.value = gun.damage;
            shotSpeedSlider.value = gun.gunData.shotspeed;
            magBulletSlider.value = gun.magBullet;
            offsetSlider.value = gun.cameroffset;
      }
}
