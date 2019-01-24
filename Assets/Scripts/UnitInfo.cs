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
    public string unitName;
    public Enums.CLASS_TYPE classType;
    public string classTypeName;
    public int level;
    public int hp; // HPの初期値は体力の2倍の数値
    public int exp;
    public int movementRange; // 移動範囲
    public int attackRange; // 攻撃範囲
    public Enums.MOVE_TYPE moveType; // 移動タイプ
    [HideInInspector]
    private bool moving; // 移動したかどうか
    public Enums.ARMY aRMY; // 勢力

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
    public Enums.STATUS status; // 状態

    void Start()
    {
        hp = vitality * 2;
        classTypeName = GetUnitClassData(classType).classTypeName;
    }

    /// <summary>
    /// 移動済みかどうかでマテリアルを切り替える
    /// </summary>
    /// <param name="moving">If set to <c>true</c> moving.</param>
    public void Moving(bool moving)
    {
        this.moving = moving;
    }

    /// <summary>
    /// 外部取得用
    /// </summary>
    /// <returns><c>true</c>, if moving was ised, <c>false</c> otherwise.</returns>
    public bool IsMoving()
    {
        return moving;
    }

    public static Struct.UnitClassData GetUnitClassData(Enums.CLASS_TYPE classType)
    {
        switch (classType)
        {
            case Enums.CLASS_TYPE.SOLDIER:
                return SOLDIER_DATA;
            case Enums.CLASS_TYPE.MAGICIAN:
                return MAGICIAN_DATA;
            case Enums.CLASS_TYPE.ARCHER:
                return ARCHER_DATA;
            default:
                return SOLDIER_DATA;
        }
    }
    // 4 平均　9

        /// <summary>
        /// 兵士
        /// </summary>
    private static readonly Struct.UnitClassData SOLDIER_DATA = new Struct.UnitClassData()
    {
        classTypeName = "兵士",
        attackRange = 1,
        movementRange = 6,
        vitality = 36,
        strength = 28,
        dexterity = 26,
        intelligence = 15,
        speed = 25,
        defense = 26,
        mDefense = 24,
        luck = 22,
        physique = 6
    };

    /// <summary>
    /// 魔法使い
    /// </summary>
    private static readonly Struct.UnitClassData MAGICIAN_DATA = new Struct.UnitClassData()
    {
        classTypeName = "魔法使い",
        attackRange = 2,
        movementRange = 6,
        vitality = 12,
        strength = 27,
        dexterity = 24,
        intelligence = 30,
        speed = 30,
        defense = 23,
        mDefense = 26,
        luck = 18,
        physique = 6
    };

    /// <summary>
    /// 射手
    /// </summary>
    private static readonly Struct.UnitClassData ARCHER_DATA = new Struct.UnitClassData()
    {
        classTypeName = "射手",
        attackRange = 2,
        movementRange = 6,
        vitality = 34,
        strength = 27,
        dexterity = 33,
        intelligence = 12,
        speed = 32,
        defense = 20,
        mDefense = 20,
        luck = 25,
        physique = 6
    };
}
