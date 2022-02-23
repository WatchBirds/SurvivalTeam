using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using FPS.item;
using FPS.Character;
public class LookingStuff : MonoBehaviour
{
      public static event Action<RaycastHit> OnRaycastHit;
      public static event Action OnRaycastDontHit;
      [Tooltip("射线发射间隔时间")]
      public float delatTime = 0.2f;
      public float maxDistance = 500f;
      public Transform Maincamera;
      [Tooltip("相机X轴最大旋转角度")]
      public float topAngleView = 60;
      [Tooltip("相机X轴最小旋转角度")]
      public float bottomAngleView = -45;
      //"相机灵敏度"
      private float mouseSensitive;
      private float scopeSensitive;
      private float currentSensitive;
      //期望的物体绕Y轴旋转的值
      private float wantedYRotation;
      //当前物体绕Y旋转的值
      private float currentYRotation;
      //期望的相机绕X轴旋转的值
      private float wantedCameraXRotation;
      private float currentCameraXRotation;
      private Transform camerPos;
      private Transform camerTar;
      private float rotationYVelocity, cameraXVelocity;
      //平滑度
      public float yRotationSpeed, xCameraSpeed;
      public float camerzOffset = -1.34F;
      public float camerxOffset;
      public float cameryOffset= 0.22F;
      public float value = 0;
      private PlayerMotor motor;
      private Touch currentTh;
      private bool ifTouch = true;

      //设置主相机位置
     private void SetCamerPos(CtrlPlayer ctrlPlayer)
      {
            camerPos = TransformHelper.FindChiled(transform, "camera");
            Maincamera.SetParent(camerPos);
            Maincamera.localPosition = Vector3.zero;
            Maincamera.localEulerAngles =new Vector3(90,0,0);
      }
      private void Start()
      { 
            Maincamera = Camera.main.transform;
            camerTar = TransformHelper.FindChiled(transform, "CameraTar");
            camerPos = TransformHelper.FindChiled(transform, "camera");
            Maincamera.SetParent(camerPos);
            Maincamera.localPosition = Vector3.zero;
            Maincamera.localEulerAngles = new Vector3(90, 0, 0);
            WeaPenMana.OnAim += OnPlayerAim;
            WeaPenMana.OnCancleAim += OnCancleAim;
            WeaPenMana.OnInit += WeaPenMana_;
            InputControl.OBtDown += InputControl_OBtDown;
            InputControl.OBtUp += InputControl_OBtUp;
            CtrlPlayer.OnPlayerBorn += SetCamerPos;
            SetPanel.OnSaveSliderData += SetPanel_OnSaveSliderData;
            WeaPenMana.OnFire += WeaPenMana_OnFire;
            WeaPenMana.OnCancleFire += WeaPenMana_OnCancleFire;
            motor = GetComponent<PlayerMotor>();
            if (PlayerPrefs.HasKey("nomale"))
            {
                  mouseSensitive = PlayerPrefs.GetFloat("nomale");
            }
            else
            {
                  mouseSensitive = 1;
                  PlayerPrefs.SetFloat("nomale", 1);
                  PlayerPrefs.Save();
            }
            if (PlayerPrefs.HasKey("sniper"))
            {
                  scopeSensitive = PlayerPrefs.GetFloat("sniper");
            }
            else
            {
                  scopeSensitive = 1;
                  PlayerPrefs.SetFloat("sniper", 1);
                  PlayerPrefs.Save();
            }
            currentSensitive = mouseSensitive;
      }

      private void WeaPenMana_OnCancleFire(GunItem gun)
      {
            //currentRecoilYPos = 0;
      }
      private float dotva = 1;
      //private float currentRecoilYPos;
      // public float recoilAmount_y = 0.5f;
      //开火时改变相机旋转量
      private void WeaPenMana_OnFire(GunItem gun)
      {
            //currentRecoilYPos -= (UnityEngine.Random.value - 0.5f) * recoilAmount_y;
            //wantedCameraXRotation -=/* Mathf.Abs(currentRecoilYPos **/ gun.cameroffset*dotva/*);*/;
           
            wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation- gun.cameroffset * dotva, bottomAngleView, topAngleView);
            camerTar.localRotation = Quaternion.Euler(wantedCameraXRotation, 0, 0);
      }

      //设置面板slider事件监听
      private void SetPanel_OnSaveSliderData()
      {
            mouseSensitive = PlayerPrefs.GetFloat("nomale");
            scopeSensitive = PlayerPrefs.GetFloat("sniper");
            currentSensitive = mouseSensitive;
      }

      private void OnEnable()
      {
            InvokeRepeating("RayUpdate", 0, delatTime);
      }
      private void OnDisable()
      {
            CancelInvoke();
      }
      private void WeaPenMana_(GunItem gun)
      {
            Maincamera.position = gun.gunCamera.transform.position;
      }

      private void Update()
      {
            MouseInputEvent();
            ApplingStuff();
      }
      public void MouseInputEvent()
      {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            wantedYRotation += Input.GetAxis("Mouse X") * currentSensitive;

            wantedCameraXRotation -=Input.GetAxis("Mouse Y") * currentSensitive;


#elif UNITY_ANDROID || UNITY_IPHONE
            if (Input.touchCount > 0 && ifTouch)
            {
                  currentTh = MyTouch.GetLastTouch();
                  wantedYRotation += currentTh.deltaPosition.x * currentSensitive*0.3f;
                  wantedCameraXRotation -= currentTh.deltaPosition.y * currentSensitive*0.3f;
            }
#endif
            wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
      }
     
      public void ApplingStuff()
      {
            //if(wantedYRotation==0&& wantedCameraXRotation==0)
            //{ value = 0; return; }
            //currentYRotation = wantedYRotation;
            //currentCameraXRotation = wantedCameraXRotation;
            currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
            currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);
            transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
            camerTar.localRotation = Quaternion.Euler(currentCameraXRotation, 0, 0);
            motor.PlayerWatch(-currentCameraXRotation / 10);
      }
      
      //瞄准事件监听
      private void OnPlayerAim(GunItem gun)
      {
            StartCoroutine(CamerMove(gun));
      }
      //镜头缓动效果
      IEnumerator CamerMove(GunItem gun)
      {
            dotva = 0.5f;
            if (gun.gunData.gunType == GunData.GunType.Sniper)
            {
                  currentSensitive = mouseSensitive * scopeSensitive;
            }
            float target1 = gun.useMainCamerView;
            float target2 = gun.useGunCamerView; 
           
            Camera main = Maincamera.GetComponent<Camera>();
            Camera guncamera = gun.gunCamera;
            float temp = 0;
            
            guncamera.enabled = true;
            main.cullingMask &= ~(1 << 11);
            guncamera.orthographic = gun.gunData.orthographic;
            guncamera.orthographicSize = 0.04f;
            while(Mathf.Abs( main.fieldOfView-target1)>0.01||Mathf.Abs(guncamera.fieldOfView-target2)>0.01)
            {
                  main.fieldOfView = Mathf.SmoothDamp(main.fieldOfView, target1, ref temp, 0.05f);
                  guncamera.fieldOfView = Mathf.SmoothDamp(guncamera.fieldOfView, target2, ref temp, 0.05f);
                  yield return null;
            } 
      }
      //取消瞄准事件监听
      private void OnCancleAim(GunItem gun)
      {
            dotva = 1;
            currentSensitive = mouseSensitive;
            StopAllCoroutines();
            //设置主相机视野
            Maincamera.GetComponent<Camera>().fieldOfView = gun.gunData.defalueMainCamerView;
            //设置枪相机视野
            gun.gunCamera.fieldOfView = gun.gunData.defalueGunCamerView;
            Maincamera.GetComponent<Camera>().cullingMask |= (1 << 11);
            gun.gunCamera.orthographic = false;
            gun.gunCamera.enabled = false;
            
      }
      //按钮抬起事件监听 (防止按钮触发touch)
      private void InputControl_OBtUp()
      {
            ifTouch = true;
      }
      //按钮按下事件监听
      private void InputControl_OBtDown(bool obj)
      {
            ifTouch = obj;
      }
      //从屏幕中央射出射线
      private void RayUpdate()
      {
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f));
            
            if (Physics.Raycast(ray, out RaycastHit hit,maxDistance, LayerMask.GetMask("RayCheck")|LayerMask.GetMask("Build")))
            {
                  OnRaycastHit?.Invoke(hit);
                  return;
            }
            OnRaycastDontHit?.Invoke();
      }
      private void OnDestroy()
      {
            WeaPenMana.OnAim -= OnPlayerAim;
            WeaPenMana.OnCancleAim -= OnCancleAim;
            WeaPenMana.OnInit -= WeaPenMana_;
            CtrlPlayer.OnPlayerBorn -= SetCamerPos;
            SetPanel.OnSaveSliderData -= SetPanel_OnSaveSliderData;
            WeaPenMana.OnFire -= WeaPenMana_OnFire;
            WeaPenMana.OnCancleFire -= WeaPenMana_OnCancleFire;
      }
}


