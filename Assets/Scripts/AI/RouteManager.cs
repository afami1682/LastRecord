using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// マップ移動でのルート算出クラス
/// </summary>
public class RouteManager
{
    // ルートの算出に必要なフィールドデータ
    Struct.Field field;
    private readonly int fieldWidth;
    private readonly int fieldHeight;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public RouteManager(Struct.Field field)
    {
        this.field = field;
        this.fieldWidth = field.width;
        this.fieldHeight = field.height;
    }

    /// <summary>
    /// フォーカスユニットから目的地までの最短ルートをチェックし、
    /// ユニットの移動ルートリストに登録する
    /// </summary>
    /// <param name="phaseManager">Cursor manager.</param>
    /// <param name="endPos">End position.</param>
    public List<Vector2> CheckShortestRoute(Struct.NodeMove[,] activeAreaList, Vector2 startPos, Vector2 endPos)
    {
        // 開始地点から終了地点までたどり着けるか
        bool isEnd = false;
        Struct.NodeMove[,] nodeList = new Struct.NodeMove[fieldHeight, fieldWidth];
        nodeList[-(int)startPos.y, (int)startPos.x].aREA = Enums.AREA.UNIT;

        // スタート地点からエンドまで再帰的に移動コストをチェックする
        CheckRootAreaRecursive(ref nodeList, activeAreaList, startPos, endPos, 0, ref isEnd);

        // ゴールできる場合は最短ルートをチェックする
        if (isEnd)
        {
            var moveRoot = new List<Vector2>();
            return CheckShootRootRecursive(activeAreaList, ref moveRoot, ref nodeList, endPos);
        }
        return null;
    }

    /// <summary>
    /// CheckShortestRouteクラスの再帰的呼び出し処理（実際のチェック処理）
    /// </summary>
    /// <param name="nodeList">Node list.</param>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="endPos">End position.</param>
    /// <param name="previousCost">Previous cost.</param>
    /// <param name="isEnd">If set to <c>true</c> is end.</param>
    private void CheckRootAreaRecursive(ref Struct.NodeMove[,] nodeList, Struct.NodeMove[,] activeAreaList, Vector2 checkPos, Vector2 endPos, int previousCost, ref bool isEnd)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
           fieldHeight <= -checkPos.y ||
            checkPos.x < 0 ||
           fieldWidth <= checkPos.x)
            return;

        // アクティブエリアでなければ何もしない
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.UNIT)
            return;

        // 省コストで上書きできない場合は終了
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost <=
            previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
            return;

        // 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.UNIT)
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost;

        // ゴールまで辿り着ける事を確認した
        isEnd |= checkPos == endPos;

        // 次に検証する座標を指定（上下左右）
        CheckRootAreaRecursive(ref nodeList, activeAreaList, checkPos + Vector2.up, endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, activeAreaList, checkPos + Vector2.down, endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, activeAreaList, checkPos + Vector2.left, endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, activeAreaList, checkPos + Vector2.right, endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
    }

    /// <summary>
    /// CheckShortestRouteクラスの再帰的呼び出し処理（最短ルートの算出）
    /// </summary>
    /// <returns>The end root.</returns>
    /// <param name="phaseManager">Cursor manager.</param>
    /// <param name="nodeList">Node list.</param>
    /// <param name="checkPos">Check position.</param>
    private List<Vector2> CheckShootRootRecursive(Struct.NodeMove[,] activeAreaList, ref List<Vector2> moveRoot, ref Struct.NodeMove[,] nodeList, Vector2 checkPos)
    {
        // 目的地からスタート位置（ユニット）まで辿り着いたら移動ルートを返す
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.UNIT) return moveRoot;

        // 上下左右のコストをチェクし昇順に並び替える
        List<Struct.NodeRoot> list = new List<Struct.NodeRoot>();
        int valUp = CheckNodeCost(ref nodeList, activeAreaList, checkPos + Vector2.up);
        if (-1 < valUp) list.Insert(list.Count, new Struct.NodeRoot(Enums.MOVE.UP, valUp));
        int valDown = CheckNodeCost(ref nodeList, activeAreaList, checkPos + Vector2.down);
        if (-1 < valDown) list.Insert(list.Count, new Struct.NodeRoot(Enums.MOVE.DOWN, valDown));
        int valLeft = CheckNodeCost(ref nodeList, activeAreaList, checkPos + Vector2.left);
        if (-1 < valLeft) list.Insert(list.Count, new Struct.NodeRoot(Enums.MOVE.LEFT, valLeft));
        int valRight = CheckNodeCost(ref nodeList, activeAreaList, checkPos + Vector2.right);
        if (-1 < valRight) list.Insert(list.Count, new Struct.NodeRoot(Enums.MOVE.RIGHT, valRight));
        list.Sort((a, b) => a.cost.CompareTo(b.cost));

        // もっともコストの低いマスを次のチェック対象とする
        switch (list[0].move)
        {
            case Enums.MOVE.UP:
                moveRoot.Insert(0, Vector2.down);
                CheckShootRootRecursive(activeAreaList, ref moveRoot, ref nodeList, checkPos + Vector2.up);
                break;

            case Enums.MOVE.DOWN:
                moveRoot.Insert(0, Vector2.up);
                CheckShootRootRecursive(activeAreaList, ref moveRoot, ref nodeList, checkPos + Vector2.down);
                break;

            case Enums.MOVE.LEFT:
                moveRoot.Insert(0, Vector2.right);
                CheckShootRootRecursive(activeAreaList, ref moveRoot, ref nodeList, checkPos + Vector2.left);
                break;

            case Enums.MOVE.RIGHT:
                moveRoot.Insert(0, Vector2.left);
                CheckShootRootRecursive(activeAreaList, ref moveRoot, ref nodeList, checkPos + Vector2.right);
                break;
        }
        return moveRoot;
    }

    /// <summary>
    /// CheckShootRootRecursiveで使用する上下左右のコストチェック関数
    /// </summary>
    /// <returns>The node cost.</returns>
    /// <param name="nodeList">Node list.</param>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    private int CheckNodeCost(ref Struct.NodeMove[,] nodeList, Struct.NodeMove[,] activeAreaList, Vector2 checkPos)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
           fieldHeight <= -checkPos.y ||
            checkPos.x < 0 ||
           fieldWidth <= checkPos.x)
            return -1;

        // アクティブエリアでなければ-1を返す
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.UNIT)
            return -1;

        // 移動コストを返す
        return nodeList[-(int)checkPos.y, (int)checkPos.x].cost;
    }

    /// <summary>
    /// フォーカスユニットの移動エリアの算出
    /// </summary>
    /// <param name="phaseManager">Cursor manager.</param>
    public void CheckMoveArea(ref Struct.NodeMove[,] activeAreaList, GameObject checkUnitObj)
    {
        // スタート地点からエンドまで再帰的に移動コストをチェックする
        Vector2 pos = checkUnitObj.transform.position;
        activeAreaList = new Struct.NodeMove[fieldHeight, fieldWidth];
        activeAreaList[-(int)pos.y, (int)pos.x].aREA = Enums.AREA.UNIT;
        CheckMoveAreaRecursive(ref activeAreaList, checkUnitObj, pos, 0);
    }

    /// <summary>
    /// CheckMoveAreaクラスの再帰的算出クラス
    /// </summary>
    /// <param name="phaseManager">Cursor manager.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="previousCost">Previous cost.</param>
    private void CheckMoveAreaRecursive(ref Struct.NodeMove[,] activeAreaList, GameObject checkUnitObj, Vector2 checkPos, int previousCost)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
           fieldHeight <= -checkPos.y ||
            checkPos.x < 0 ||
           fieldWidth <= checkPos.x)
            return;

        // キャラが移動できないマスなら何もしない
        if (!IsMoveing(field.cells[-(int)checkPos.y, (int)checkPos.x].category, checkUnitObj.GetComponent<UnitInfo>().moveType))
            return;

        // 他Unitとのすれ違い判定
        UnitInfo targetUnit = GameManager.GetUnit().GetMapUnitInfo(checkPos);
        if (GameManager.GetAICommonCalc().IsAttackTarget(checkUnitObj.GetComponent<UnitInfo>(), targetUnit)) return;

        // 省コストで上書きできない場合は終了
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost <=
            previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
            return;

        // 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enums.AREA.UNIT)
        {
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = Enums.AREA.MOVE;
        }

        // 移動コストを超えた場合は終了
        if (checkUnitObj.GetComponent<UnitInfo>().movingRange <= activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost) return;

        // 次に検証する座標を指定（上下左右）
        CheckMoveAreaRecursive(ref activeAreaList, checkUnitObj, checkPos + Vector2.up, activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref activeAreaList, checkUnitObj, checkPos + Vector2.down, activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref activeAreaList, checkUnitObj, checkPos + Vector2.left, activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref activeAreaList, checkUnitObj, checkPos + Vector2.right, activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
    }

    /// <summary>
    /// フォーカスユニットの攻撃エリアの算出
    /// </summary>
    /// <param name="list">List.</param>
    /// <param name="startPos">Start position.</param>
    /// <param name="attackRange">Attack range.</param>
    public void CheckAttackArea(ref Struct.NodeMove[,] list, Vector2 startPos, int attackRange)
    {
        // 開始地点を基準に上下左右を検証
        CheckAttackAreaRecursive(ref list, startPos, Vector2.up, attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector2.down, attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector2.left, attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector2.right, attackRange);
    }

    /// <summary>
    /// CheckAttackAreaクラスの再帰的算出クラス
    /// </summary>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="nextPos">Next position.</param>
    /// <param name="previousCost">Previous cost.</param>
    private void CheckAttackAreaRecursive(ref Struct.NodeMove[,] activeAreaList, Vector2 checkPos, Vector2 nextPos, int previousCost)
    {
        // 攻撃範囲を超えた場合は終了
        if (previousCost <= 0) return;

        // コストの算出
        checkPos += nextPos;

        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
           fieldHeight <= -checkPos.y ||
            checkPos.x < 0 ||
           fieldWidth <= checkPos.x)
            return;

        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.NONE)
        {
            // 移動エリアでなければ攻撃範囲として登録
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = Enums.AREA.ATTACK;
        }
        else if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enums.AREA.ATTACK &&
                 activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost < previousCost - 1)
        {
            // 既に攻撃範囲として登録されていても、少ないコストで上書き出来るならば上書きする
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
        }
        else return; // 条件に合致しない場合は、そこで探索終了する


        // 次に検証する座標を指定（上下左右）
        if (nextPos != Vector2.down) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector2.up, previousCost);
        if (nextPos != Vector2.up) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector2.down, previousCost);
        if (nextPos != Vector2.right) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector2.left, previousCost);
        if (nextPos != Vector2.left) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector2.right, previousCost);
    }

    /// <summary>
    /// ユニットタイプ毎の移動可能セルのチェック
    /// </summary>
    /// <returns><c>true</c>, if moveing was ised, <c>false</c> otherwise.</returns>
    /// <param name="cellCategory">Cell category.</param>
    /// <param name="moveType">Move type.</param>
    public static bool IsMoveing(int cellCategory, Enums.MOVE_TYPE moveType)
    {
        // キャラ毎の移動可能かどうかのチェック
        switch (moveType)
        {
            case Enums.MOVE_TYPE.WALKING:
                if (cellCategory == 1) return false;
                break;
            case Enums.MOVE_TYPE.ATHLETE: break;
            case Enums.MOVE_TYPE.HORSE: break;
            case Enums.MOVE_TYPE.FLYING: break;
        }
        return true;
    }
}