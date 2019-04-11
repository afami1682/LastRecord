using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各Unit情報の構造体
/// </summary>
public class UnitInfo : MonoBehaviour
{
    [Header("基本パラメータ")]
    public Enums.AI_TYPE AiType; // AIタイプ
    public Enums.UNIT_KIND UnitKind; // 勢力
    public Enums.CLASS_TYPE ClassType; // クラスタイプ
    public Enums.MOVE_TYPE MoveType; // 移動タイプ
    public Enums.STATUS Status; // 状態異常

    // ユニークID
    [SerializeField]
    int id = 0;
    public int Id { get { return id; } }

    // レベル
    [SerializeField]
    int level;
    public int Level { get { return level; } }

    // 経験値
    [SerializeField]
    int exp;
    public int Exp { get { return exp; } }

    // HPの初期値は体力の2倍の数値
    [SerializeField]
    int hp;
    public int Hp { get { return hp; } }

    // HPの最大値
    [SerializeField]
    int hPMax;
    public int HpMax { get { return hPMax; } }

    // ユニットの名前
    [SerializeField]
    string unitName = string.Empty;
    public string UnitName { get { return unitName; } }

    // クラス名（ジョブ)
    [SerializeField]
    string unitClassName;
    public string UnitClassName { get { return unitClassName; } }

    // 攻撃範囲
    [SerializeField]
    int attackRange;
    public int AttackRange { get { return attackRange; } }

    // 移動範囲
    [SerializeField]
    int movementRange;
    public int MovementRange { get { return movementRange; } }

    public bool Acted; // 行動したかどうか

    [Header("成長パラメター")]

    // 体力
    [SerializeField]
    int vitality;
    [HideInInspector]
    public int Vitality { get { return vitality; } }

    // 筋力
    [SerializeField]
    int strengtht;
    [HideInInspector]
    public int Strengtht { get { return strengtht; } }

    // 技量
    [SerializeField]
    int technical;
    [HideInInspector]
    public int Technical { get { return technical; } }

    // 速さ
    [SerializeField]
    int speed;
    [HideInInspector]
    public int Speed { get { return speed; } }

    // 防御
    [SerializeField]
    int defense;
    [HideInInspector]
    public int Defense { get { return defense; } }

    // 魔防
    [SerializeField]
    int resist;
    public int Resist { get { return resist; } }

    // 幸運
    [SerializeField]
    int luck;
    [HideInInspector]
    public int Luck { get { return luck; } }

    [Header("実ステータス")]

    public int StatusVitality; // 体力
    public int StatusStrengtht; // 筋力
    public int StatusTechnical; // 技量
    public int StatusSpeed; // 速さ
    public int StatusDefense; // 防御
    public int StatusResist; // 魔防
    public int StatusLuck; // 幸運

    void Start()
    {
        // 成長パラメータのロード(現時点ではInspectorから設定)

        // 実ステータスの設定
        StatusVitality = vitality;
        StatusStrengtht = strengtht;
        StatusTechnical = technical;
        StatusSpeed = speed;
        StatusDefense = defense;
        StatusResist = resist;
        StatusLuck = luck;

        // 基本パラメータのロード(現時点ではInspectorから設定)
        hPMax = vitality * 2;
        hp = hPMax;

        Struct.UnitClassData unitClassData = GetUnitClassData(ClassType);
        unitClassName = unitClassData.UnitClassName;
        attackRange = unitClassData.AttackRange;
        movementRange = unitClassData.MovementRange;
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
                return SwordsmanMaxStatus;
            case Enums.CLASS_TYPE.WIZARD:
                return WizardMaxStatus;
            case Enums.CLASS_TYPE.ARCHER:
                return ArcherMaxStatus;
            default:
                return SwordsmanMaxStatus;
        }
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage">Damage.</param>
    public void Damaged(int damage)
    {
        hp = Mathf.Clamp(hp - damage, 0, 999);
    }

    /// <summary>
    /// 回復する
    /// </summary>
    /// <param name="point">Point.</param>
    public void Recover(int point)
    {
        hp = Mathf.Clamp(hp + point, 0, hPMax);
    }

    /// <summary>
    /// 経験値の加算
    /// </summary>
    /// <param name="val">Value.</param>
    public void AddExp(int val)
    {
        exp += val;
    }

    /// <summary>
    /// Expのリセット
    /// </summary>
    public void ResetExp()
    {
        exp = 0;
    }

    /// <summary>
    /// 新しいステータスの設定
    /// </summary>
    /// <param name="level">Level.</param>
    /// <param name="vitality">Vitality.</param>
    /// <param name="strengtht">Strengtht.</param>
    /// <param name="technical">Technical.</param>
    /// <param name="speed">Speed.</param>
    /// <param name="defense">Defense.</param>
    /// <param name="resist">Resist.</param>
    /// <param name="luck">Luck.</param>
    public void NewStatus(int level, int vitality, int strengtht, int technical, int speed, int defense, int resist, int luck)
    {
        this.level = level;
        this.vitality = vitality;
        this.strengtht = strengtht;
        this.technical = technical;
        this.speed = speed;
        this.defense = defense;
        this.resist = resist;
        this.luck = luck;

        hPMax = vitality * 2;
    }

    // 4 平均　9
    private static readonly Struct.UnitClassData SwordsmanMaxStatus = new Struct.UnitClassData()
    {
        UnitClassName = "剣士", // クラス名
        AttackRange = 1, // 攻撃範囲
        MovementRange = 5, // 移動範囲
        Vitality = 36, // 体力
        Attack = 26, // 攻撃
        Technical = 26, // 技
        Speed = 25, // 速さ
        Defense = 26, // 防御
        Resist = 24, // 魔防
        Luck = 22, // 幸運
    };

    private static readonly Struct.UnitClassData WizardMaxStatus = new Struct.UnitClassData()
    {
        UnitClassName = "ウィザード", // クラス名
        AttackRange = 2, // 攻撃範囲
        MovementRange = 5, // 移動範囲
        Vitality = 36, // 体力
        Attack = 26, // 攻撃
        Technical = 26, // 技
        Speed = 25, // 速さ
        Defense = 26, // 防御
        Resist = 24, // 魔防
        Luck = 22, // 幸運
    };

    private static readonly Struct.UnitClassData ArcherMaxStatus = new Struct.UnitClassData()
    {
        UnitClassName = "射手", // クラス名
        AttackRange = 2, // 攻撃範囲
        MovementRange = 5, // 移動範囲
        Vitality = 36, // 体力
        Attack = 26, // 攻撃
        Technical = 26, // 技
        Speed = 25, // 速さ
        Defense = 26, // 防御
        Resist = 24, // 魔防
        Luck = 22, // 幸運
    };

    private static readonly Struct.UnitClassData KegMaxStatus = new Struct.UnitClassData()
    {
        UnitClassName = "樽", // クラス名
        AttackRange = 0, // 攻撃範囲
        MovementRange = 5, // 移動範囲
        Vitality = 1, // 体力
        Attack = 0, // 攻撃
        Technical = 0, // 技
        Speed = 0, // 速さ
        Defense = 0, // 防御
        Resist = 0, // 魔防
        Luck = 0, // 幸運
    };
}
