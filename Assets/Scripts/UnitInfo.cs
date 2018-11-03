using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public enum MOVE_TYPE
    {
        WALKING, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        ATHLETE, // 「建物」以外は移動可能
        HORSE, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        FLYING, // 「深森」以外は移動可能
    }

    public enum ARMY
    {
        ALLY, // 味方
        ENEMY, // 敵
        NEUTRAL // 第３勢力
    }

    public int ID = 0;
    public int Level = 1;
    public string Name = "No Name";
    public int HP = 20;
    public int maxHP = 20;
    public int moveDistance = 5; // 移動値
    public int attackRange = 1; // 攻撃範囲
    public MOVE_TYPE moveType = 0; // 移動タイプ
    public bool isMoving = false;
    public ARMY aRMY; // 勢力
    [HideInInspector]
    public MoveController moveController;
}
