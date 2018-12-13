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
    public Enum.CLASS_TYPE classType;
    public int level;
    public int hp;
    public int movementRange; // 移動範囲
    public int attackRange; // 攻撃範囲
    public Enum.MOVE_TYPE moveType; // 移動タイプ
    [HideInInspector]
    public bool isMoving; // 移動中かどうか
    public Enum.ARMY aRMY; // 勢力

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
    public Enum.STATUS status; // 状態
}
