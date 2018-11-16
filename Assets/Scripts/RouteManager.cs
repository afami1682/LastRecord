using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RouteManager : MonoBehaviour
{
    // 定数
    public enum AREA { NONE, UNIT, MOVE, ATTACK }
    public enum MOVE { NONE, UP, DOWN, LEFT, RIGHT }

    /// <summary>
    /// 目的地までの最短ルートをチェックする
    /// </summary>
    /// <param name="cursorManager">Cursor manager.</param>
    /// <param name="endPos">End position.</param>
    public void CheckShortestRoute(ref CursorManager cursorManager, Vector3 endPos)
    {
        // 最短の移動ルート情報
        cursorManager.moveRoot = new List<Vector3>();

        // 開始地点から終了地点までたどり着けるか
        bool isEnd = false;
        NodeMove[,] nodeList = new NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];
        nodeList[-(int)cursorManager.focusUnit.moveController.getPos().y, (int)cursorManager.focusUnit.moveController.getPos().x].aREA = AREA.UNIT;

        // スタート地点からエンドまで再帰的に移動コストをチェックする
        CheckRootAreaRecursive(ref nodeList, ref cursorManager.activeAreaList, cursorManager.focusUnit.moveController.getPos(), ref endPos, 0, ref isEnd);

        // ゴールできる場合は最短ルートをチェックする
        if (isEnd) cursorManager.moveRoot = CheckEndRoot(ref cursorManager, ref nodeList, endPos);
    }

    /// <summary>
    /// Checks the root area recursive.
    /// </summary>
    /// <param name="nodeList">Node list.</param>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="endPos">End position.</param>
    /// <param name="previousCost">Previous cost.</param>
    /// <param name="isEnd">If set to <c>true</c> is end.</param>
    private void CheckRootAreaRecursive(ref NodeMove[,] nodeList, ref NodeMove[,] activeAreaList, Vector3 checkPos, ref Vector3 endPos, int previousCost, ref bool isEnd)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return;

        // アクティブエリアでなければ何もしない
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.UNIT)
            return;

        // 省コストで上書きできない場合は終了
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].cost != 0 &&
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost <=
            previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
            return;

        // 移動前のコストと今回のコストを合計して設定する（開始地点を除く）
        if (nodeList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.UNIT)
            nodeList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost;

        // ゴールまで辿り着ける事を確認した
        if (checkPos == endPos)
            isEnd = true;

        // 次に検証する座標を指定（上下左右）
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.up, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.down, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.left, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
        CheckRootAreaRecursive(ref nodeList, ref activeAreaList, checkPos + Vector3.right, ref endPos, nodeList[-(int)checkPos.y, (int)checkPos.x].cost, ref isEnd);
    }

    /// <summary>
    /// 指定された場所の上下左右の内、一番コストが引くいマスをルート情報に追加する
    /// </summary>
    /// <returns>The end root.</returns>
    /// <param name="cursorManager">Cursor manager.</param>
    /// <param name="nodeList">Node list.</param>
    /// <param name="checkPos">Check position.</param>
    private List<Vector3> CheckEndRoot(ref CursorManager cursorManager, ref NodeMove[,] nodeList, Vector3 checkPos)
    {
        if (checkPos == cursorManager.focusUnit.moveController.getPos())
            return cursorManager.moveRoot;

        // 上下左右のコストをチェクし昇順に並び替える
        List<NodeRoot> list = new List<NodeRoot>();
        int valUp = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.up);
        if (-1 < valUp) list.Insert(list.Count, new NodeRoot(MOVE.UP, valUp));
        int valDown = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.down);
        if (-1 < valDown) list.Insert(list.Count, new NodeRoot(MOVE.DOWN, valDown));
        int valLeft = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.left);
        if (-1 < valLeft) list.Insert(list.Count, new NodeRoot(MOVE.LEFT, valLeft));
        int valRight = CheckNodeCost(ref nodeList, ref cursorManager.activeAreaList, checkPos + Vector3.right);
        if (-1 < valRight) list.Insert(list.Count, new NodeRoot(MOVE.RIGHT, valRight));
        list.Sort((a, b) => a.cost.CompareTo(b.cost));

        // もっともコストの低いマスにチェックを移す
        switch (list[0].move)
        {
            case MOVE.UP:
                cursorManager.moveRoot.Insert(0, Vector3.down); // 移動ルートの追加
                CheckEndRoot(ref cursorManager, ref nodeList, checkPos + Vector3.up); // スタート地点まで再帰的にたどる
                break;

            case MOVE.DOWN:
                cursorManager.moveRoot.Insert(0, Vector3.up); // 移動ルートの追加
                CheckEndRoot(ref cursorManager, ref nodeList, checkPos + Vector3.down); // スタート地点まで再帰的にたどる
                break;

            case MOVE.LEFT:
                cursorManager.moveRoot.Insert(0, Vector3.right); // 移動ルートの追加
                CheckEndRoot(ref cursorManager, ref nodeList, checkPos + Vector3.left); // スタート地点まで再帰的にたどる
                break;

            case MOVE.RIGHT:
                cursorManager.moveRoot.Insert(0, Vector3.left); // 移動ルートの追加
                CheckEndRoot(ref cursorManager, ref nodeList, checkPos + Vector3.right); // スタート地点まで再帰的にたどる
                break;
        }
        return cursorManager.moveRoot;
    }

    private int CheckNodeCost(ref NodeMove[,] nodeList, ref NodeMove[,] activeAreaList, Vector3 checkPos)
    {
        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return -1;

        // アクティブエリアでなければ-1を返す
        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.MOVE &&
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.UNIT)
            return -1;

        // 移動コストを返す
        return nodeList[-(int)checkPos.y, (int)checkPos.x].cost; ;
    }

    /// <summary>
    /// Checks the move area.
    /// </summary>
    /// <param name="cursorManager">Cursor manager.</param>
    public void CheckMoveArea(ref CursorManager cursorManager)
    {
        // スタート地点からエンドまで再帰的に移動コストをチェックする
        cursorManager.activeAreaList = new NodeMove[MapManager.GetFieldData().height, MapManager.GetFieldData().width];
        cursorManager.activeAreaList[-(int)cursorManager.focusUnit.moveController.getPos().y, (int)cursorManager.focusUnit.moveController.getPos().x].aREA = AREA.UNIT;
        CheckMoveAreaRecursive(ref cursorManager, cursorManager.focusUnit.moveController.getPos(), 0);
    }

    /// <summary>
    /// Checks the move area recursive.
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

        // セルにユニットがいた場合のすり抜けチェック
        if (GameManager.GetMapUnit(checkPos))
        {
            switch (GameManager.GetMapUnit(checkPos).aRMY)
            {
                case UnitInfo.ARMY.ALLY:
                    if (cursorManager.focusUnit.aRMY == UnitInfo.ARMY.ENEMY)
                        return;
                    break;
                case UnitInfo.ARMY.ENEMY:
                    if (cursorManager.focusUnit.aRMY == UnitInfo.ARMY.ALLY ||
                        cursorManager.focusUnit.aRMY == UnitInfo.ARMY.NEUTRAL)
                        return;
                    break;
                case UnitInfo.ARMY.NEUTRAL:
                    if (cursorManager.focusUnit.aRMY == UnitInfo.ARMY.ENEMY)
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
        if (cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA != AREA.UNIT)
        {
            cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost + MapManager.GetFieldData().cells[-(int)checkPos.y, (int)checkPos.x].moveCost;
            cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = AREA.MOVE;
        }

        // 移動コストを超えた場合は終了
        if (cursorManager.focusUnit.movementRange <= cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost)
            return;

        // 次に検証する座標を指定（上下左右）
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.up, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.down, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.left, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
        CheckMoveAreaRecursive(ref cursorManager, checkPos + Vector3.right, cursorManager.activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost);
    }

    /// <summary>
    /// Checks the attack area.
    /// </summary>
    /// <param name="list">List.</param>
    /// <param name="startPos">Start position.</param>
    /// <param name="focusUnit">Focus unit.</param>
    public void CheckAttackArea(ref NodeMove[,] list, Vector3 startPos, ref UnitInfo focusUnit)
    {
        // 開始地点を基準に上下左右を検証
        CheckAttackAreaRecursive(ref list, startPos, Vector3.up, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.down, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.left, focusUnit.attackRange);
        CheckAttackAreaRecursive(ref list, startPos, Vector3.right, focusUnit.attackRange);
    }

    /// <summary>
    /// 攻撃可能なエリアを再帰的にチェックし、配列に追加する
    /// </summary>
    /// <param name="activeAreaList">Active area list.</param>
    /// <param name="checkPos">Check position.</param>
    /// <param name="nextPos">Next position.</param>
    /// <param name="previousCost">Previous cost.</param>
    public void CheckAttackAreaRecursive(ref NodeMove[,] activeAreaList, Vector3 checkPos, Vector3 nextPos, int previousCost)
    {
        // 移動コストを超えた場合は終了
        if (previousCost <= 0)
            return;

        checkPos += nextPos;

        // 配列の外（マップ外）なら何もしない
        if (-(int)checkPos.y < 0 ||
            MapManager.GetFieldData().height <= -checkPos.y ||
            checkPos.x < 0 ||
            MapManager.GetFieldData().width <= checkPos.x)
            return;

        if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == AREA.NONE)
        {
            // 攻撃範囲の登録
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA = AREA.ATTACK;
        }
        else if (activeAreaList[-(int)checkPos.y, (int)checkPos.x].aREA == AREA.ATTACK &&
                 activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost < previousCost - 1)
        {
            // 攻撃範囲の上書き
            previousCost--;
            activeAreaList[-(int)checkPos.y, (int)checkPos.x].cost = previousCost;
        }
        else
        {
            return;
        }

        // 次に検証する座標を指定（上下左右）
        if (nextPos != Vector3.down)
            CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.up, previousCost);
        if (nextPos != Vector3.up)
            CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.down, previousCost);
        if (nextPos != Vector3.right)
            CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.left, previousCost);
        if (nextPos != Vector3.left)
            CheckAttackAreaRecursive(ref activeAreaList, checkPos, Vector3.right, previousCost);
    }

    /// <summary>
    /// ユニットタイプ毎の移動可能セルのチェック
    /// </summary>
    /// <returns><c>true</c>, if moveing was ised, <c>false</c> otherwise.</returns>
    /// <param name="cellCategory">Cell category.</param>
    /// <param name="moveType">Move type.</param>
    public static bool isMoveing(int cellCategory, UnitInfo.MOVE_TYPE moveType)
    {
        // キャラ毎の移動可能かどうかのチェック
        switch (moveType)
        {
            case UnitInfo.MOVE_TYPE.WALKING:
                if (cellCategory == 1) return false;
                break;
            case UnitInfo.MOVE_TYPE.ATHLETE: break;
            case UnitInfo.MOVE_TYPE.HORSE: break;
            case UnitInfo.MOVE_TYPE.FLYING: break;
        }
        return true;
    }

}

public struct NodeMove
{
    public int cost;
    public RouteManager.AREA aREA;
}

public class NodeRoot
{
    public RouteManager.MOVE move;
    public int cost;

    public NodeRoot(RouteManager.MOVE move, int cost)
    {
        this.move = move;
        this.cost = cost;
    }
}