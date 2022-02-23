using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//关于战斗相关协议
[Serializable]//玩家信息
public class CharacterInfo
{
      public string id;
      public int hp;

      public float x = 0;
      public float y = 0;
      public float z = 0;
      public float ex = 0;
      public float ey = 0;
      public float ez = 0;
      public string gunName;
}
/// <summary>
/// 进入战斗协议 服务端推送
/// </summary>
public class MsgEnterBattle:MsgBase
{
      public MsgEnterBattle()
      {
            protoName = "MsgEnterBattle";
      }
      //服务端回
      public CharacterInfo[] characters;
      //地图编号
      public int mapId = 1;
}
/// <summary>
/// 游戏结果协议 服务端推送
/// </summary>
public class MsgBattleResult:MsgBase
{
      public MsgBattleResult()
      {
            protoName = "MsgBattleResult";
      }
      //服务端回
      public int result = 0;
}
/// <summary>
/// 离开游戏协议 服务端推送
/// </summary>
public class MsgLeaveBattle:MsgBase
{
      public MsgLeaveBattle()
      {
            protoName = "MsgLeaveBattle";
      }
      public string id = "";//玩家id
}
[Serializable]
public class EnemyInfo
{
      /// <summary>预制体名称</summary>///
      public string prefabName;
      /// <summary>敌人唯一id</summary>///
      public string id;
      /// <summary>生成位置下标</summary>///
      public int birthPos;
      //大体位置下标
      public int pos;
}
/// <summary>
/// 创建敌人协议
/// </summary>
public class MsgCreatEnemy : MsgBase
{
      public MsgCreatEnemy()
      {
            protoName = "MsgCreatEnemy";
      }
      //服务端回
      public EnemyInfo[] enemies;

}
/// <summary>
/// 玩家加载完毕协议
/// </summary>
public class MsgLoadFin : MsgBase
{
      public MsgLoadFin()
      {
            protoName = "MsgLoadFin";
      }
      public int result;
      public string id;
}
/// <summary>
/// 击中协议
/// </summary>
public class MsgHit : MsgBase
{
      public MsgHit()
      {
            protoName = "MsgHit";
      }
      //服务端回
      public string id;
      //击中位置
      public float x;
      public float y;
      public float z;
      //方向
      public float ex;
      public float ey;
      public float ez;
      //特效名称
      public string fxname;
}
/// <summary>
/// 攻击敌人协议
/// </summary>
public class MsgHitCharacter : MsgBase
{
      public MsgHitCharacter()
      {
            protoName = "MsgHitCharacter";
      }
      //服务端回
      public string id;
      //攻击到的角色id
      public string hitId;
      //怪物名称
      public string hitName;
      //造成的伤害
      public int damage;
}
/// <summary>
/// 攻击玩家协议
/// </summary>
public class MsgHitPlayer : MsgBase
{
      public MsgHitPlayer()
      {
            protoName = "MsgHitPlayer";
      }
      public string id;
      public string hitId;
      public int damage;
}
/// <summary>
/// 玩家换枪协议
/// </summary>
public class MsgChangeGun : MsgBase
{
      public MsgChangeGun()
      {
            protoName = "MsgChangeGun";
      }
      //服务端补充
      public string id;
      //客户端发
      public string gunid;
}
[Serializable]
public class GunInfo
{
      public string id;
      public int posindex;
      public string gunname;
}
/// <summary>
/// 生成枪协议
/// </summary>
public class MsgCreatGun : MsgBase
{
      public MsgCreatGun()
      {
            protoName = "MsgCreatGun";
      }
      public GunInfo[] guns;
}
/// <summary>
/// 敌人被击杀协议
/// </summary>
public class MsgKillEnemy : MsgBase
{
      public MsgKillEnemy()
      {
            protoName = "MsgKillEnemy";
      }
      //击杀者id
      public string id;
      //被击杀者id
      public string bekillId;
      //怪物名称
      public string name;
      //击杀者可获取的金币数
      public string coin;
}
/// <summary>
/// 玩家被击杀协议
/// </summary>
public class MsgPlayerDie : MsgBase
{
      public MsgPlayerDie()
      {
            protoName = "MsgPlayerDie";
      }
      //玩家id
      public string id;
      //击杀者id
      public string whokill;
}
//返回房间协议
public class MsgBackToRoom : MsgBase
{
      public MsgBackToRoom()
      {
            protoName = "MsgBackToRoom";
      }
      public int result = 0;//1代表成功0代表失败
      public string id;//离开的玩家id
}
public class MsgChangeScope:MsgBase
{
      public MsgChangeScope()
      {
            protoName = "MsgChangeScope";
      }
      public string id;
      public string scopeName;
      public int result;//0代表失败1代表成功
}
public class MsgGetPlayerNumber:MsgBase
{
      public MsgGetPlayerNumber()
      {
            protoName = "MsgGetPlayerNumber";
      }

      public int playerNumber;
}
public class MsgPlayerReborn:MsgBase
{
      public MsgPlayerReborn()
      {
            protoName = "MsgPlayerReborn";
      }
      //服务端发
      public string id;
      public int result;//0代表失败
}
public class MsgGetCoin:MsgBase
{
      public MsgGetCoin()
      {
            protoName = "MsgGetCoin";
      }
      //客户端发
      public int coin;
}
public class MsgGetBullet:MsgBase
{
      public MsgGetBullet()
      {
            protoName = "MsgGetBullet";
      }
      public string id;
      public string boxid;
      public int bulletNumber;
}
public class MsgGetOwerId:MsgBase
{
      public MsgGetOwerId()
      {
            protoName = "MsgGetOwerId";
      }
      public string ownerid;
}

