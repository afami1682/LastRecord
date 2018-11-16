using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各Unit情報の構造体
/// </summary>
public class UnitInfo : MonoBehaviour
{
    /// <summary>
    /// 基本パラメータ
    /// </summary>
    [Header("基本パラメータ")]
    public int id;
    public string name;
    public CLASS_TYPE classType;
    public int level;
    public int hp;
    public int movementRange; // 移動範囲
    public int attackRange; // 攻撃範囲
    public MOVE_TYPE moveType; // 移動タイプ
    [HideInInspector]
    public bool isMoving; // 移動中かどうか
    public ARMY aRMY; // 勢力

    [Header("成長パラメータ")]
    public int vitality; // 体力
    public int strength; // 筋力
    public int dexterity; // 技量
    public int intelligence; // 魔力
    public int speed; // 速さ
    public int defense; // 防御
    public int mDefense; // 魔防
    public int luck; // 幸運

    [Header("サブパラメータ")]
    public int physique; // 体格（自分の体格未満のUnitを救出できる）
    public int accompanyId; // 同行UnitのId
    public STATUS status; // 状態

    [HideInInspector]
    public MoveController moveController; // 移動管理クラス

    /// <summary>
    /// 移動タイプ
    /// </summary>
    public enum MOVE_TYPE
    {
        WALKING, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        ATHLETE, // 「建物」以外は移動可能
        HORSE, // 「大山」「川」「海」「建物」「深森」以外は移動可能
        FLYING, // 「深森」以外は移動可能
    }

    /// <summary>
    /// 勢力
    /// </summary>
    public enum ARMY
    {
        ALLY, // 味方
        ENEMY, // 敵
        NEUTRAL // 第３勢力
    }

    /// <summary>
    /// Unitの状態
    /// </summary>
    public enum STATUS
    {
        HEALTH, // 健康（通常）
        POISON, // 毒（数ターンの間、小ダメージを受ける）
        SLEEP, // 睡眠（数ターンの間、行動不可）
        CONFUSION, // 混乱（数ターンの間、操作不能かつ無差別攻撃）
        SILENCE, // 沈黙（数ターンの間、魔法使用不可）
        HANDCUFFS, //手枷（数ターンの間、攻撃不可）
        FETTERS // 足枷 （数ターンの間、移動不可）
    }

    /// <summary>
    /// クラスタイプ
    /// </summary>
    public enum CLASS_TYPE
    {
        SWORDSMAN
    }
}
