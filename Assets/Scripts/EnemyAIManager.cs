using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 敵UnitのAI
/// </summary>
public class EnemyAIManager
{
    readonly int fieldWidth;
    readonly int fieldHeight;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public EnemyAIManager(Struct.Field field)
    {
        this.fieldWidth = field.width;
        this.fieldHeight = field.height;
    }

    /// <summary>
    /// アクティブエリア内にて攻撃できるプレイヤーのリストを返す
    /// </summary>
    /// <returns>The attack target unit.</returns>
    /// <param name="activeAreaList">Active area list.</param>
    public List<GameObject> GetAttackTargetList(Struct.NodeMove[,] activeAreaList)
    {
        // 攻撃範囲内にいるプレイヤーUnit
        List<GameObject> targetList = new List<GameObject>();

        // アクティブエリア内にて攻撃できるプレイヤーUnitを取得
        for (int y = 0; y < GameManager.GetMap().field.height; y++)
            for (int x = 0; x < GameManager.GetMap().field.width; x++)
                if (activeAreaList[y, x].aREA == Enums.AREA.MOVE ||
                    activeAreaList[y, x].aREA == Enums.AREA.ATTACK)
                    if (GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)) &&
                        GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)).GetComponent<UnitInfo>().aRMY == Enums.ARMY.ALLY)
                        targetList.Add(GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)));

        // 攻撃できる対象がいなければ終了
        if (targetList.Count < 1) return null;

        return targetList;
    }

    /// <summary>
    /// ターゲットリストの中で一番攻撃する条件の良いユニットを返す
    /// </summary>
    /// <returns>The attack target selection.</returns>
    /// <param name="targetList">Target list.</param>
    public GameObject GetAttackTargetSelection(UnitInfo unitInfo, List<GameObject> targetList)
    {
        // リストが空ならnullを返す
        if (targetList == null) return null;

        // 攻撃可能な全ての敵に対して、攻撃シュミレーションを行う
        List<Dictionary<string, int>> simulationEvaluations = new List<Dictionary<string, int>>();
        UnitInfo targetUnitInfo;
        int damage, hitRate, deathBlowRate, attackCount, evaluationPoint;
        for (int i = 0; i < targetList.Count; i++)
        {
            // 戦闘シュミレーション
            targetUnitInfo = targetList[i].GetComponent<UnitInfo>();
            damage = GameManager.GetCommonCalc().GetAttackDamage(unitInfo, targetUnitInfo);
            hitRate = GameManager.GetCommonCalc().GetHitRate(unitInfo, targetUnitInfo);
            deathBlowRate = GameManager.GetCommonCalc().GetDeathBlowRete(unitInfo, targetUnitInfo);
            attackCount = GameManager.GetCommonCalc().GetAttackCount(unitInfo, targetUnitInfo);

            // 評価ポイントの計算( ダメージ * 攻撃回数 + 命中率 / 2 + 必殺率)
            evaluationPoint = damage * attackCount + hitRate / 2 + deathBlowRate;

            simulationEvaluations.Add(new Dictionary<string, int>(){
                {"id",i},
                {"evaluationPoint", evaluationPoint}}
                );
        };

        // 評価点リストを降順で並び替え
        simulationEvaluations.Sort((a, b) => b["evaluationPoint"] - a["evaluationPoint"]);

        return targetList[simulationEvaluations[0]["id"]];
    }

    /// <summary>
    /// 移動できる範囲で、ターゲットに攻撃できる場所のリストを返す
    /// </summary>
    /// <returns>The attack location calculate.</returns>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="targetUnit">Target unit.</param>
    public List<Vector3> GetAttackLocationList(Struct.NodeMove[,] activeAreaList, GameObject myUnit, GameObject targetUnit)
    {
        Vector3 targetPos = targetUnit.transform.position;
        UnitInfo myUnitInfo = myUnit.GetComponent<UnitInfo>();
        UnitInfo targetUnitInfo = targetUnit.GetComponent<UnitInfo>();

        List<Vector3> attackLocationList = new List<Vector3>();

        // 下列の左側からチェックしていく
        for (int y = (int)targetPos.y - myUnitInfo.attackRange; y <= -(int)targetPos.y + myUnitInfo.attackRange; y++)
            for (int x = (int)targetPos.x - myUnitInfo.attackRange; x <= (int)targetPos.x + myUnitInfo.attackRange; x++)
            {
                // 配列の外（マップ外）は飛ばす
                if (-y < 0 ||
                   fieldHeight <= -y ||
                    x < 0 ||
                   fieldWidth <= x) continue;

                // 敵ユニットの位置は飛ばす
                if ((int)targetPos.y == y && (int)targetPos.x == x) continue;

                // 自分でない他ユニットがいるなら飛ばす
                if (activeAreaList[-y, x].aREA != Enums.AREA.UNIT &&
                GameManager.GetUnit().GetMapUnitObj(new Vector3(x, y, 0)) != null) continue;

                // 攻撃が届く範囲の移動場所の追加
                if (GameManager.GetCommonCalc().GetCellDistance(new Vector3(x, y, 0), targetPos) <= myUnitInfo.attackRange)
                {
                    switch (activeAreaList[-y, x].aREA)
                    {
                        case Enums.AREA.MOVE:
                        case Enums.AREA.UNIT:
                            attackLocationList.Add(new Vector3(x, y, 0));
                            break;
                    }
                }
            }
        return attackLocationList;
    }
}
