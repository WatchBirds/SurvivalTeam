using System;
[Serializable]
/// <summary>
/// 需要存储到数据库的玩家信息
/// </summary>
public class PlayerData
{
      public string name;
      //金币
      public int coin = 0;
      //记事本
      public string text = "new text";
      //头像名称
      public string sprite = "defalue";
      //玩家拥有的枪
      public string[] guns;
}
[Serializable]
public class PlayerFriend
{
      public FriendDbData[] friends;
}
/// <summary>
/// 数据库信息
/// </summary>
[Serializable]
public class FriendDbData
{
      public string name;
      public string fullStr;
}
public enum DataType
{
      coin,
      text,
      sprite,
      guns
}