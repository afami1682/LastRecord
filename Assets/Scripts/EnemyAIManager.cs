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
    int fieldWidth, fieldHeight;

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
    /// 攻撃できる範囲内の中で、一番有効な敵に対して、一番良い場所から攻撃する
    /// </summary>
    /// <param name="phaseManager">Phase manager.</param>
    public Vector3 AttackLocationCalc(ref PhaseManager phaseManager)
    {
        // 攻撃範囲内にいるプレイヤーUnit
        List<GameObject> targetList = new List<GameObject>();

        // アクティブエリア内の攻撃できるプレイヤーUnitの取得
        for (int y = 0; y < GameManager.GetMap().field.height; y++)
            for (int x = 0; x < GameManager.GetMap().field.width; x++)
                if (phaseManager.activeAreaManager.activeAreaList[y, x].aREA == Enums.AREA.MOVE ||
                    phaseManager.activeAreaManager.activeAreaList[y, x].aREA == Enums.AREA.ATTACK)
                    if (GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)) &&
                        GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)).GetComponent<UnitInfo>().aRMY == Enums.ARMY.ALLY)
                        targetList.Add(GameManager.GetUnit().GetMapUnitObj(new Vector3(x, -y, 0)));

        // 攻撃できる対象がいなければ終了
        if (targetList.Count < 1) return Vector3.zero;

        // TODO とりあえず現時点では一番最初にみつけたUnitを攻撃対象とする
        GameObject targetUnit = targetList[0];
        Vector3 targetPos = targetUnit.transform.position;
        UnitInfo targetUnitInfo = targetUnit.GetComponent<UnitInfo>();

        // ターゲト周辺の移動可能マス内で一番有効な攻撃場所を探索する
        Vector3 checkPos;
        List<Vector3> cellPos = new List<Vector3>();
        for (int a = 1; a <= targetUnitInfo.attackRange; a++)
        {
            // 上
            checkPos = targetPos + new Vector3(0, a, 0);
            if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);
            // 右上
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(a / 2, a / 2, 0);
                if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                    if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);
            }
            // 右
            checkPos = targetPos + new Vector3(a, 0, 0);
            if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);
            // 右下
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(a / 2, -a / 2, 0);
                if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                    if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);
            }
            // 下
            checkPos = targetPos + new Vector3(0, -a, 0);
            if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);
            // 左下
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(-a / 2, -a / 2, 0);
                if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                    if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);
            }
            // 左
            checkPos = targetPos + new Vector3(-a, 0, 0);
            if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                    cellPos.Add(checkPos);
            // 左上
            if (a % 2 == 0)
            {
                checkPos = targetPos + new Vector3(-a / 2, a / 2, 0);
                if (phaseManager.activeAreaManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.MOVE)
                    if (GameManager.GetUnit().GetMapUnitObj(checkPos) == null)
                        cellPos.Add(checkPos);
            }
        }

        // TODO とりあえず一番最初に見つけた移動エリアを返す

        return (0 < cellPos.Count) ? cellPos[0] : Vector3.zero;
    }

    /// <summary>
    /// 攻撃場所の算出
    /// </summary>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="targetUnit">Target unit.</param>
    /// <param name="attackRange">Attack range.</param>
    /// <param name="isAttack">If set to <c>true</c> is attack.</param>
    private void AttackLocationCalcRecursively(ref Struct.NodeMove[,] activeAreaList, GameObject targetUnit, Vector3 checkPos, int attackRange, ref bool isAttack)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
           fieldHeight <= -checkPos.y ||
            checkPos.x < 0 ||
           fieldWidth <= checkPos.x)
            return;

        // アクティブエリアでなければ何もしない
        //if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.NONE) return;

        // ターゲットと同じ場所は何もしない
        //if (targetUnit.transform.position == checkPos) return;

        //// 省コストで上書きできない場合は終了
        //if (nodeList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
        //    nodeList[-(int)checkPos.y, (int)checkPos.x].cost <=
        //    previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
        //    return;

        //// 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        //if (nodeList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.UNIT)
        //nodeList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost;

        // 次に検証する座標を指定（上下左右）
        //AttackLocationCalcRecursively(ref activeAreaList, targetUnit, checkPos + Vector3.up, attackRange, ref isAttack);
        //AttackLocationCalcRecursively(ref activeAreaList, targetUnit, checkPos + Vector3.down, attackRange, ref isAttack);
        //AttackLocationCalcRecursively(ref activeAreaList, targetUnit, checkPos + Vector3.left, attackRange, ref isAttack);
        //AttackLocationCalcRecursively(ref activeAreaList, targetUnit, checkPos + Vector3.right, attackRange, ref isAttack);
    }
}
