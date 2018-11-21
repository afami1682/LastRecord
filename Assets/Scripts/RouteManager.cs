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
    /// <summary>
    /// フォーカスユニットから目的地までの最短ルートをチェックし、
    /// ユニットの移動ルートリストに登録する
    /// </summary>
    /// <param name="cursorManager">Cursor manager.</param>
    /// <param name="endPos">End position.</param>
    public void CheckShortestRoute(ref CursorManager cursorManager, Vector3 endPos)
    {
        // ユニットの移動ルートを初期化
        cursorManager.moveRoot = new List<Vector3>();

        // 開始地点から終了地点までたどり着けるか
        bool isEnd = false;
        Struct.NodeMove[,] nodeList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];
        nodeList[-(int)cursorManager.focusUnit.moveController.getPos().y, (int)cursorManager.focusUnit.moveController.getPos().x].aREA = Enum.AREA.UNIT;

        // スタート地点からエンドまで再帰的に移動コストをチェックする
        CheckRootAreaRecursive(ref nodeList, ref cursorManager.activeAreaList, cursorManager.focusUnit.moveController.getPos(), ref endPos, 0, ref isEnd);

        // ゴールできる場合は最短ルートをチェックする
        if (isEnd) cursorManager.moveRoot = CheckShootRootRecursive(ref cursorManager, ref nodeList, endPos);
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
    private void CheckRootAreaRecursive(ref Struct.NodeMove[,] nodeList, ref Struct.NodeMove[,] activeAreaList, Vector3 checkPos, ref Vector3 endPos, int previousCost, ref bool isEnd)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return;

        // アクティブエリアでなければ何もしない
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.UNIT)
            return;

        // 省コストで上書きできない場合は終了
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost <=
            previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
            return;

        // 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.UNIT)
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost;

        // ゴールまで辿り着ける事を確認した
        if (checkPos == endPos) isEnd = true;

        // 次に検証する座標を指定（上下左右）
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.up, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.down, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.left, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.right, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
    }

    /// <summary>
    /// CheckShortestRouteクラスの再帰的呼び出し処理（最短ルートの算出）
    /// </summary>
    /// <returns>The end root.</returns>
    /// <param name="cursorManager">Cursor manager.</param>
    /// <param name="nodeList">Node list.</param>
    /// <param name="checkPos">Check position.</param>
    private List<Vector3> CheckShootRootRecursive(ref CursorManager cursorManager, ref Struct.NodeMove[,] nodeList, Vector3 checkPos)
    {
        // 目的地からスタート位置（ユニット）まで辿り着いたら移動ルートを返す
        if (checkPos == cursorManager.focusUnit.moveController.getPos()) return cursorManager.moveRoot;

        // 上下左右のコストをチェクし昇順に並び替える
        List<Struct.NodeRoot> list = new List<Struct.NodeRoot>();
        int valUp = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.up);
        if (-1 < valUp) list.Insert(list.Count, new Struct.NodeRoot(Enum.MOVE.UP, valUp));
        int valDown = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.down);
        if (-1 < valDown) list.Insert(list.Count, new Struct.NodeRoot(Enum.MOVE.DOWN, valDown));
        int valLeft = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.left);
        if (-1 < valLeft) list.Insert(list.Count, new Struct.NodeRoot(Enum.MOVE.LEFT, valLeft));
        int valRight = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.right);
        if (-1 < valRight) list.Insert(list.Count, new Struct.NodeRoot(Enum.MOVE.RIGHT, valRight));
        list.Sort((a, b) => a.cost.CompareTo(b.cost));

        // もっともコストの低いマスを次のチェック対象とする
        switch (list[0].move)
        {
            case Enum.MOVE.UP:
                cursorManager.moveRoot.Insert(0, Vector3.down);
                CheckShootRootRecursive(ref cursorManager, ref nodeList, checkPos + Vector3.up);
                break;

            case Enum.MOVE.DOWN:
                cursorManager.moveRoot.Insert(0, Vector3.up);
                CheckShootRootRecursive(ref cursorManager, ref nodeList, checkPos + Vector3.down);
                break;

            case Enum.MOVE.LEFT:
                cursorManager.moveRoot.Insert(0, Vector3.right);
                CheckShootRootRecursive(ref cursorManager, ref nodeList, checkPos + Vector3.left);
                break;

            case Enum.MOVE.RIGHT:
                cursorManager.moveRoot.Insert(0, Vector3.left);
                CheckShootRootRecursive(ref cursorManager, ref nodeList, checkPos + Vector3.right);
                break;
        }
        return cursorManager.moveRoot;
    }

    /// <summary>
    /// CheckShootRootRecursiveで使用する上下左右のコストチェック関数
    /// </summary>
    /// <returns>The node cost.</returns>
    /// <param name="nodeList">Node list.</param>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    private int CheckNodeCost(ref Struct.NodeMove[,] nodeList, ref Struct.NodeMove[,] activeAreaList, Vector3 checkPos)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return -1;

        // アクティブエリアでなければ-1を返す
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.UNIT)
            return -1;

        // 移動コストを返す
        return nodeList[-(int)checkPos.y, (int)checkPos.x].cost; ;
    }

    /// <summary>
    /// フォーカスユニットの移動エリアの算出
    /// </summary>
    /// <param name="cursorManager">Cursor manager.</param>
    public void CheckMoveArea(ref CursorManager cursorManager)
    {
        // スタート地点からエンドまで再帰的に移動コストをチェックする
        cursorManager.activeAreaList = new Struct.NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];
        cursorManager.activeAreaList[-(int)cursorManager.focusUnit.moveController.getPos().y, (int)cursorManager.focusUnit.moveController.getPos().x].aREA = Enum.AREA.UNIT;
        CheckMoveAreaRecursive(ref cursorManager, cursorManager.focusUnit.moveController.getPos(), 0);
    }

    /// <summary>
    /// CheckMoveAreaクラスの再帰的算出クラス
    /// </summary>
    /// <param name="cursorManager">Cursor manager.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="previousCost">Previous cost.</param>
    private void CheckMoveAreaRecursive(ref CursorManager cursorManager, Vector3 checkPos, int previousCost)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return;

        // キャラが移動できないマスなら何もしない
        if (!isMoveing(MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].category, cursorManager.focusUnit.moveType))
            return;

        // 移動先にユニットがいた場合のすり抜けチェック
        if (GameManager.GetMapUnit(checkPos))
        {
            switch (GameManager.GetMapUnit(checkPos).aRMY)
            {
                case Enum.ARMY.ALLY:
                    if (cursorManager.focusUnit.aRMY == Enum.ARMY.ENEMY)
                        return;
                    break;
                case Enum.ARMY.ENEMY:
                    if (cursorManager.focusUnit.aRMY == Enum.ARMY.ALLY ||
                        cursorManager.focusUnit.aRMY == Enum.ARMY.NEUTRAL)
                        return;
                    break;
                case Enum.ARMY.NEUTRAL:
                    if (cursorManager.focusUnit.aRMY == Enum.ARMY.ENEMY)
                        return;
                    break;
            }
        }

        // 省コストで上書きできない場合は終了
        if (cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
            cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost <=
            previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
            return;

        // 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        if (cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != Enum.AREA.UNIT)
        {
            cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost;
            cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = Enum.AREA.MOVE;
        }

        // 移動コストを超えた場合は終了
        if (cursorManager.focusUnit.movementRange <= cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost) return;

        // 次に検証する座標を指定（上下左右）
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.up, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.down, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.left, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.right, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
    }

    /// <summary>
    /// フォーカスユニットの攻撃エリアの算出
    /// </summary>
    /// <param name="list">List.</param>
    /// <param name="startPos">Start position.</param>
    /// <param name="focusUnit">Focus unit.</param>
    public void CheckAttackArea(ref Struct.NodeMove[,] list, Vector3 startPos, ref UnitInfo focusUnit)
    {
        // 開始地点を基準に上下左右を検証
        CheckAttackAreaRecursive(ref list, startPos, Vector3.up, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.down, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.left, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.right, focusUnit.attackRange);
    }

    /// <summary>
    /// CheckAttackAreaクラスの再帰的算出クラス
    /// </summary>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="nextPos">Next position.</param>
    /// <param name="previousCost">Previous cost.</param>
    private void CheckAttackAreaRecursive(ref Struct.NodeMove[,] activeAreaList, Vector3 checkPos, Vector3 nextPos, int previousCost)
    {
        // 攻撃範囲を超えた場合は終了
        if (previousCost <= 0) return;

        // コストの算出
        checkPos += nextPos;

        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return;

        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enum.AREA.NONE)
        {
            // 移動エリアでなければ攻撃範囲として登録
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = Enum.AREA.ATTACK;
        }
        else if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == Enum.AREA.ATTACK &&
                 activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost < previousCost - 1)
        {
            // 既に攻撃範囲として登録されていても、少ないコストで上書き出来るならば上書きする
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
        }
        else return; // 条件に合致しない場合は、そこで探索終了する


        // 次に検証する座標を指定（上下左右）
        if (nextPos != Vector3.down) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.up, previousCost);
        if (nextPos != Vector3.up) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.down, previousCost);
        if (nextPos != Vector3.right) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.left, previousCost);
        if (nextPos != Vector3.left) CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.right, previousCost);
    }

    /// <summary>
    /// ユニットタイプ毎の移動可能セルのチェック
    /// </summary>
    /// <returns><c>true</c>, if moveing was ised, <c>false</c> otherwise.</returns>
    /// <param name="cellCategory">Cell category.</param>
    /// <param name="moveType">Move type.</param>
    public static bool isMoveing(int cellCategory, Enum.MOVE_TYPE moveType)
    {
        // キャラ毎の移動可能かどうかのチェック
        switch (moveType)
        {
            case Enum.MOVE_TYPE.WALKING:
                if (cellCategory == 1) return false;
                break;
            case Enum.MOVE_TYPE.ATHLETE: break;
            case Enum.MOVE_TYPE.HORSE: break;
            case Enum.MOVE_TYPE.FLYING: break;
        }
        return true;
    }
}