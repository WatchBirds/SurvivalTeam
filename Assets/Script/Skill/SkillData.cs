using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace ARPGDemo.Skill
{
      [Serializable]//类成员可以在面板显示
      public class SkillData
      {
            [Tooltip("技能ID编号")]
            public int skillID;
             [Tooltip("技能名称")]
            public string name;
            [Tooltip("技能描述")]
            public string description;
            [Tooltip("冷却时间")]
            public float coolTime;
            [Tooltip("魔法消耗")]
            public int costSp;
            [Tooltip("技能打中给的力大小")]
            public float force;
            [Tooltip("技能攻击距离")]
            public float attackDistance;
            [Tooltip("技能攻击角度")]
            public int attackAngle;
            [Tooltip("攻击目标的tag")]
            public string[] tags;
            [Tooltip("连击的下一个技能编号(0不进行连击)")]
            public int nextBatterld;
            [Tooltip("伤害比率(该值*角色伤害为实际伤害)")]
            public float damage;
            [Tooltip("技能持续时间")]
            public float durationTime;
            [Tooltip("伤害间隔")]
            public float damageInterval;
            [Tooltip("技能预制件名称")]
            public string prefabName;
            [Tooltip("该技能动画参数名称")]
            public string animationName;
            [Tooltip("该技能攻击后创建特效名称")]
            public string hitFxName;
            [Tooltip("技能类型 单攻,群攻")]
            public SkillAttacckTape attackType;
            [Tooltip("伤害模式 圆形,扇形,矩形")]
            public DamageMode damageMode;
            [Tooltip("对自身的影响")]
            public string[] selfImpacts;
            [Tooltip("对目标的影响")]
            public string[] targetImpacts;
            [Tooltip("技能施放位置")]
            public Transform pos;
            public bool IfCheck;
            
            [HideInInspector]
            /// <summary>冷却剩余</summary>
            public float coolRemain; 
            [HideInInspector]
            /// <summary>攻击对象数组</summary>
            public GameObject[] attackTargets;
            [HideInInspector] 
            /// <summary>技能所属</summary>
            public GameObject Onwer;
            [HideInInspector]
            /// <summary>技能预制件对象</summary>
            public GameObject skillPrefab;
            [HideInInspector]
            /// <summary>受击特效</summary>
            public GameObject hitFxPerfab;
            [HideInInspector]
            /// <summary>是否激活</summary>
            public bool activated;
      }
}