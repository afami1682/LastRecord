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
    public int level;
    public int hp;
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
}
