using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃イベント
/// </summary>
public class AttackEvent : BattleFunc
{

    UnitInfo playerUnit;
    UnitInfo enemyUnit;
    int damage;
    bool deathBlow;
    int test = 0;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="playerUnit">Player unit.</param>
    /// <param name="enemyUnit">Enemy unit.</param>
    /// <param name="damage">Damage.</param>
    /// <param name="deathBlow">If set to <c>true</c> death blow.</param>
    public AttackEvent(UnitInfo playerUnit, UnitInfo enemyUnit, int damage, bool deathBlow)
    {
        this.playerUnit = playerUnit;
        this.enemyUnit = enemyUnit;
        this.damage = damage;
        this.deathBlow = deathBlow;
    }

    /// <summary>
    /// 毎フレーム実行されるイベント
    /// </summary>
    /// <returns>イベントが実行中かどうか</returns>
    protected override bool Event()
    {

        test++;
        Debug.Log(test);

        return 100 < test ? false : true;
    }
}
