using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.Character;
using AI.Perception;
public class TestPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
      {
            GameObject playerobj = ResourceManager.Load<GameObject>("FpsBasePlayer");
            GameObject player = Instantiate(playerobj);
            player.AddComponent<CharacherAnimator>();
            CtrlPlayer ctrlPlayer = player.AddComponent<CtrlPlayer>();
            player.AddComponent<WeaPenMana>().Init("assault_rifle_03");
            //player.AddComponent<PlayerMotor>();
            player.AddComponent<LookingStuff>();
            player.transform.position = transform.position;

            //GameObject playerobj01 = ResourceManager.Load<GameObject>("BasePlayer");
            //GameObject player01 = Instantiate(playerobj01);
            //SyncPlayer syncPlayer = player01.AddComponent<SyncPlayer>();
            //SyncWeapenMana syncWeapen = player01.AddComponent<SyncWeapenMana>();
            //syncPlayer.Init("third_person_character_lpfp@handgun");
            //syncWeapen.Init("Assault_Rifle_03");
      }
}
