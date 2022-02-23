using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
/// <summary>
/// 相机脱离玩家后
/// </summary>
public class MainCamerMotion : MonoBehaviour
{
      private Transform dieView;
      private Vector3 temp;
      private void Start()
      {
            dieView = GameObject.Find("DieView").transform;
            CtrlPlayer.OnPlayerDie += CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn += CtrlPlayer_OnPlayerBorn;
      }

      private void CtrlPlayer_OnPlayerBorn(CtrlPlayer ctrlPlayer)
      {
            StartCoroutine(MoveToPalyer(ctrlPlayer));
      }

      IEnumerator MoveToPalyer(CtrlPlayer player)
      {
            while (Vector3.Distance(transform.position,player.transform.position)>0.1f)
            {
                  transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.35f);
                  yield return null;
            }
            if (transform.parent != null)
            {
                  transform.localPosition = Vector3.zero;
            }
      }
      //玩家死亡事件监听
      private void CtrlPlayer_OnPlayerDie()
      {
            //Vector3 diepos = transform.position;
            //transform.position = diepos;
            transform.rotation = dieView.rotation;
            StartCoroutine(DelayMove(dieView.position));
      }
      IEnumerator DelayMove(Vector3 pos)
      {
            while (Vector3.Distance( transform.position,pos)>0.01f )
            {
                  transform.position = Vector3.MoveTowards(transform.position, pos, 0.35f);
                  yield return null;
            }
      }
      private void OnDestroy()
      {
            CtrlPlayer.OnPlayerDie -= CtrlPlayer_OnPlayerDie;
            CtrlPlayer.OnPlayerBorn -= CtrlPlayer_OnPlayerBorn;
      }
}
