using System.Collections;
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
    /// 攻撃できる範囲内の中で、一番有効な敵に対して、一番良い場所から攻撃する
    /// </summary>
    /// <returns>The attack location calculate.</returns>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="targetUnit">Target unit.</param>
    public Vector3 GetAttackLocationCalc(Struct.NodeMove[,] activeAreaList, GameObject myUnit, GameObject targetUnit)
    {

        Vector3 targetPos = targetUnit.transform.position;
        UnitInfo targetUnitInfo = targetUnit.GetComponent<UnitInfo>();

        // ターゲト周辺の移動可能マス内で一番有効な攻撃場所を探索する
        Vector3 checkPos;
        List<Vector3> cellPos = new List<Vector3>();
        for (int a = 1; a <= targetUnitInfo.attackRange; a++)
        {
            // 上
            checkPos = targetPos + new Vector3(0, a, 0);
            if (1 > (int)checkPos.y)
            {
                // 離れた位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
              GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);

                // 今いる位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                    checkPos == myUnit.transform.position)
                    cellPos.Add(checkPos);
            }
            // 右上
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(a / 2, a / 2, 0);
                if ((int)checkPos.x < GameManager.GetMap().field.width &&
                    1 > (int)checkPos.y)
                {
                    // 離れた位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
                  GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);

                    // 今いる位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                        checkPos == myUnit.transform.position)
                        cellPos.Add(checkPos);
                }
            }
            // 右
            checkPos = targetPos + new Vector3(a, 0, 0);
            if ((int)checkPos.x < GameManager.GetMap().field.width)
            {
                // 離れた位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
              GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);

                // 今いる位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                    checkPos == myUnit.transform.position)
                    cellPos.Add(checkPos);
            }
            // 右下
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(a / 2, -a / 2, 0);
                if ((int)checkPos.x < GameManager.GetMap().field.width &&
                    (int)checkPos.y > -GameManager.GetMap().field.height)
                {
                    // 離れた位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
                  GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);

                    // 今いる位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                        checkPos == myUnit.transform.position)
                        cellPos.Add(checkPos);
                }
            }
            // 下
            checkPos = targetPos + new Vector3(0, -a, 0);
            if ((int)checkPos.y > -GameManager.GetMap().field.height)
            {
                // 離れた位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
              GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);

                // 今いる位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                    checkPos == myUnit.transform.position)
                    cellPos.Add(checkPos);
            }

            // 左下
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(-a / 2, -a / 2, 0);
                if (-1 < (int)checkPos.x &&
                    (int)checkPos.y > -GameManager.GetMap().field.height)
                {
                    // 離れた位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
                  GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);

                    // 今いる位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                        checkPos == myUnit.transform.position)
                        cellPos.Add(checkPos);
                }
            }
            // 左
            checkPos = targetPos + new Vector3(-a, 0, 0);
            if (-1 < (int)checkPos.x)
            {
                // 離れた位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
              GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);

                // 今いる位置の検証
                if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                    checkPos == myUnit.transform.position)
                    cellPos.Add(checkPos);
            }

            // 左上
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(-a / 2, a / 2, 0);
                if (-1 < (int)checkPos.x &&
                    1 > (int)checkPos.y)
                {
                    // 離れた位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE &&
                  GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);

                    // 今いる位置の検証
                    if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT &&
                        checkPos == myUnit.transform.position)
                        cellPos.Add(checkPos);
                }
            }
        }

        // TODO とりあえず一番最初に見つけた移動エリアを返す
        return (0 < cellPos.Count) ? cellPos[0] : Vector3.zero;
    }
}
