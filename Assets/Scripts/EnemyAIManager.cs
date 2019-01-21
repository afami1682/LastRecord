﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 敵UnitのAI
/// </summary>
public class EnemyAIManager
{
    Struct.Field field;
    readonly int fieldWidth;
    readonly int fieldHeight;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public EnemyAIManager(Struct.Field field)
    {
        this.field = field;
        this.fieldWidth = field.width;
        this.fieldHeight = field.height;
    }

    /// <summary>
    /// アクティブエリア内にて攻撃できるプレイヤーUnitを取得
    /// </summary>
    /// <returns>The attack target unit.</returns>
    /// <param name="activeAreaList">Active area list.</param>
    public GameObject GetAttackTargetUnit(Struct.NodeMove[,] activeAreaList)
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

        // TODO とりあえず現時点では一番最初にみつけたUnitを攻撃対象とする
        return targetList[0];
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
