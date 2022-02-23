//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine.EventSystems;
//using FPS.Character;
//using UnityEngine.UI;
//public class BattlePanel : BasePanel
//{
//      private ETCJoystick joystick;
//      private EventTrigger firBt;
//      private Button airBt;
//      private BaseControl control;
//      public override void OnClose()
//      {
            
//      }

//      public override void OnInit()
//      {
//            skinPath = "BattlePanel";
//            layer = PanelManager.Layer.Panel;
//      }

//      public override void OnShow(params object[] para)
//      {
//            //寻找组件
//            control = FindObjectOfType<BaseControl>();
//            joystick = TransformHelper.FindChiled(skin.transform, "Joy").GetComponent<ETCJoystick>();
//            firBt = TransformHelper.FindChiled(skin.transform, "FirBt").GetComponent<EventTrigger>();
//            airBt = TransformHelper.FindChiled(skin.transform, "AirBt").GetComponent<Button>();
//            //绑定事件
//            joystick.onMove.AddListener(control.OnJoyStickMove);
//            joystick.onMoveEnd.AddListener(control.OnJoyStickStop);
//            GameMain.instance.AddTriggersListener(firBt.gameObject, EventTriggerType.PointerUp, OnfirBtUp);
//            GameMain.instance.AddTriggersListener(firBt.gameObject, EventTriggerType.PointerDown, OnfirBtDown);
//            airBt.onClick.AddListener(control.OnAimBtDown);
//      }
//      private void OnfirBtUp(BaseEventData eventData)
//      {
//            control.OnFireBtUp();
//      }
//      private void OnfirBtDown(BaseEventData eventData)
//      {
//            control.OnFireBtDown();
//      }
//}

