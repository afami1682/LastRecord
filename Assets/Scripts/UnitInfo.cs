using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各Unit情報の構造体
/// </summary>
public class UnitInfo : MonoBehaviour
{
    public int id;
    public int level;
    public int hp; // HPの初期値は体力の2倍の数値
    public int hpMax; // HPの最大値
    public int exp;
    public string unitName;
    public string className; // クラス名

    public Enums.AI_TYPE aIType; // AIタイプ
    public Enums.ARMY aRMY; // 勢力
    public Enums.CLASS_TYPE classType; // クラスタイプ
    public Enums.MOVE_TYPE moveType; // 移動タイプ
    public Enums.STATUS status; // 状態異常

    public int movingRange; // 移動範囲
    public int attackRange; // 攻撃範囲

    public bool acted; // 行動したかどうか

    [Header("成長パラメータ")]
    public int vitality; // 体力
    public int strengtht; // 筋力
    public int technical; // 技量
    public int speed; // 速さ
    public int defense; // 防御
    public int resist; // 魔防
    public int luck; // 幸運
    public int move; // 移動

    void Start()
    {
        // 初期HPの設定
        hpMax = vitality * 2;
        hp = hpMax;

        className = GetUnitClassData(classType).className;
        // attackRange = 1;
        movingRange = GetUnitClassData(classType).move;
    }

    /// <summary>
    /// クラス別のステータス値を返す
    /// </summary>
    /// <returns>The unit class data.</returns>
    /// <param name="classType">Class type.</param>
    public static Struct.UnitClassData GetUnitClassData(Enums.CLASS_TYPE classType)
    {
        switch (classType)
        {
            case Enums.CLASS_TYPE.SWORDSMAN:
                return SWORDSMAN_MAX_STATUS;
            case Enums.CLASS_TYPE.WIZARD:
                return WIZARD_MAX_STATUS;
            case Enums.CLASS_TYPE.ARCHER:
                return ARCHER_MAX_STATUS;
            default:
                return SWORDSMAN_MAX_STATUS;
        }
    }

    // 4 平均　9
    private static readonly Struct.UnitClassData SWORDSMAN_MAX_STATUS = new Struct.UnitClassData()
    {
        className = "剣士", // クラス名
        vitality = 36, // 体力
        attack = 26, // 攻撃
        technical = 26, // 技
        speed = 25, // 速さ
        defense = 26, // 防御
        resist = 24, // 魔防
        luck = 22, // 幸運
        move = 5 // 移動
    };

    private static readonly Struct.UnitClassData WIZARD_MAX_STATUS = new Struct.UnitClassData()
    {
        className = "ウィザード", // クラス名
        vitality = 36, // 体力
        attack = 26, // 攻撃
        technical = 26, // 技
        speed = 25, // 速さ
        defense = 26, // 防御
        resist = 24, // 魔防
        luck = 22, // 幸運
        move = 5 // 移動
    };

    private static readonly Struct.UnitClassData ARCHER_MAX_STATUS = new Struct.UnitClassData()
    {
        className = "射手", // クラス名
        vitality = 36, // 体力
        attack = 25, // 攻撃
        technical = 26, // 技
        speed = 25, // 速さ
        defense = 26, // 防御
        resist = 24, // 魔防
        luck = 22, // 幸運
        move = 5 // 移動
    };
}
