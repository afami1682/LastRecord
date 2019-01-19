using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 共通の計算処理
/// </summary>
public class CommonCalc
{
    // 武器レベル
    // S 補正値 + 5 
    // A 補正値 + 4
    // B 補正値 + 3
    // C 補正値 + 2
    // D 補正値 + 1
    // E 補正値 + 0

    /// <summary>
    /// 攻撃する側の攻撃力計算(四捨五入)
    /// </summary>
    /// <returns>攻撃値</returns>
    /// <param name="myUnit">My unit.</param>
    public int GetAttackPoin(UnitInfo myUnit)
    {
        // 攻撃力 = 筋力 + (武器の攻撃力 * 特攻係数) + 武器レベル補正値
        // 武器の攻撃力は6~20を想定 筋力と防御力はMax50
        // カンストだと 50 + (20 * 1) + 5 = 80 が最大攻撃力となる
        return myUnit.strength + (10 * 1) + 1; // 仮で設定
    }

    /// <summary>
    /// ターゲットに与えるダメージの計算
    /// </summary>
    /// <returns>攻撃のダメージ</returns>
    /// <param name="myUnit">My unit.</param>
    /// <param name="targetUnit">Target unit.</param>
    public int GetAttackDamage(UnitInfo myUnit, UnitInfo targetUnit)
    {
        //  攻撃力- 防御力 - 地形ボーナス
        int damage = GetAttackPoin(myUnit)
            - targetUnit.defense
                        - GameManager.GetMap().field.cells[
                            -(int)targetUnit.transform.position.y,
                            (int)targetUnit.transform.position.x
                           ].defenseBonus;

        return (0 < damage) ? damage : 0;
    }

    /// <summary>
    /// 攻撃の命中率を返す
    /// </summary>
    /// <returns>The hit rate.</returns>
    /// <param name="myUnit">My unit.</param>
    /// <param name="targetUnit">Target unit.</param>
    public int GetHitRate(UnitInfo myUnit, UnitInfo targetUnit)
    {
        // 命中 - 回避
        int hitRate = GetHitVal(myUnit) - GetDodgeVal(targetUnit);
        return Mathf.Clamp(hitRate, 0, 100);
    }

    /// <summary>
    /// 攻撃の命中値を返す
    /// </summary>
    /// <returns>The hit value.</returns>
    /// <param name="unitInfo">Unit info.</param>
    public int GetHitVal(UnitInfo unitInfo)
    {
        // 参考　=(技×3+幸運)/2+(装備武器の命中+非特効補正)+装備武器のレベル補正+3すくみ補正+クラス補正
        // =(技×3+幸運)/2+装備武器のレベル補正
        int hitVal = (unitInfo.dexterity * 3 + unitInfo.luck) + 1; // 仮設定

        return (0 < hitVal) ? hitVal : 0;
    }

    /// <summary>
    /// 回避値を返す
    /// </summary>
    /// <returns>The dodge value.</returns>
    /// <param name="unitInfo">Unit info.</param>
    public int GetDodgeVal(UnitInfo unitInfo)
    {
        // 参考 =(速×3+幸運)/2+装備武器の回避+地形効果+クラス補正
        //  =(速×3+幸運)/2 + 地形効果
        int dodgeVal = (unitInfo.speed * 3 + unitInfo.luck) / 2
            + GameManager.GetMap().field.cells[
            -(int)unitInfo.transform.position.y,
            (int)unitInfo.transform.position.x
        ].avoidanceBonus;

        return (0 < dodgeVal) ? dodgeVal : 0;
    }

    /// <summary>
    /// 実効命中率による攻撃命中判定
    /// </summary>
    /// <returns><c>true</c>, if hit decision was gotten, <c>false</c> otherwise.</returns>
    /// <param name="hitRate">Hit rate.</param>
    public bool GetHitDecision(int hitRate)
    {
        // 0 ~ 99の乱数を二つ生成
        int r1 = Random.Range(0, 100);
        int r2 = Random.Range(0, 100);

        // 二つの乱数の平均値を求める(小数点切り捨て)
        int r3 = (int)Mathf.Floor((r1 + r2) / 2);

        // 平均値が命中率以下であれば命中とする
        return (r3 < hitRate) ? true : false;
    }

    /// <summary>
    /// 確率の成功判定
    /// </summary>
    /// <returns><c>true</c>, if check was randomed, <c>false</c> otherwise.</returns>
    /// <param name="probability">Probability.</param>
    public bool ProbabilityDecision(int probability) { return UnityEngine.Random.Range(0, 100) < probability ? true : false; }
}
