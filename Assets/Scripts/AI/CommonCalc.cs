using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace AI
{
    public class CommonCalc
    {
        private Struct.Field field;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="field">Field.</param>
        public CommonCalc(Struct.Field field)
        {
            this.field = field;
        }

        /// <summary>
        /// 特定のユニットから目的地までの移動ルートを返す
        /// </summary>
        /// <returns>The root.</returns>
        /// <param name="targetUnit">Target unit.</param>
        /// <param name="endPos">End position.</param>
        public List<Vector2> GetMoveRoot(GameObject targetUnit, Vector2 endPos)
        {
            var costList = GetMoveAreaCosts(targetUnit);
            List<Vector2> moveRoot = new List<Vector2>();
            CheckShootRootRecursive(costList, ref moveRoot, targetUnit.transform.position, endPos);
            return moveRoot;
        }

        /// <summary>
        /// 特定の軍のUnitに対して、特定のUnitから移動開始した場合のコストリストを返す
        /// </summary>
        /// <returns>The uni move costs.</returns>
        /// <param name="targetUnitObj">Target unit object.</param>
        /// <param name="army">Army.</param>
        public SortedDictionary<int, GameObject> NearUniMoveCosts(GameObject targetUnitObj, Enums.ARMY army)
        {
            var unitList = GameManager.GetUnit().GetUnitList(army);
            var costList = GetMoveAreaCosts(targetUnitObj);

            Vector2 pos;
            var searchUnitList = new SortedDictionary<int, GameObject>();
            foreach (var unit in unitList)
            {
                pos = unit.transform.position;
                searchUnitList.Add(costList[-(int)pos.y, (int)pos.x], unit);
            }
            return searchUnitList;
        }

        /// <summary>
        /// 対象のUnitから移動開始した場合のエリア全体の移動コストリストを返す
        /// </summary>
        /// <returns>The move area costs.</returns>
        /// <param name="targetUnitObj">Target unit object.</param>
        public int[,] GetMoveAreaCosts(GameObject targetUnitObj)
        {
            var costList = new int[field.height, field.width];
            CheckMoveAreaCostsRecursive(
                        ref costList,
                targetUnitObj.transform.position,
                                 targetUnitObj.transform.position,
                                     targetUnitObj.GetComponent<UnitInfo>().moveType,
                                 0);
            return costList;
        }

        /// <summary>
        /// Checks the move area costs recursive.
        /// </summary>
        /// <param name="costList">Cost list.</param>
        /// <param name="checkPos">Check position.</param>
        /// <param name="previousCost">Previous cost.</param>
        private void CheckMoveAreaCostsRecursive(ref int[,] costList, Vector2 startPos, Vector2 checkPos, Enums.MOVE_TYPE moveType, int previousCost)
        {
            // 配列の外（マップ外）なら何もしない
            if (!IsInField((int)checkPos.x, -(int)checkPos.y)) return;

            // キャラが移動できないマスなら何もしない
            if (!IsMoveing(field.cells[-(int)checkPos.y, (int)checkPos.x].category, moveType)) return;

            // 省コストで上書きできない場合は終了
            if (costList[-(int)checkPos.y, (int)checkPos.x] != 0 &&
                costList[-(int)checkPos.y, (int)checkPos.x] <=
                previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost)
                return;

            // 開始地点を除き、移動コストを記録する
            if (startPos != checkPos)
                costList[-(int)checkPos.y, (int)checkPos.x] =
                previousCost + field.cells[-(int)checkPos.y, (int)checkPos.x].moveCost;

            // 次に検証する座標を指定（上下左右）
            CheckMoveAreaCostsRecursive(ref costList, startPos, checkPos + Vector2.down, moveType, costList[-(int)checkPos.y, (int)checkPos.x]);
            CheckMoveAreaCostsRecursive(ref costList, startPos, checkPos + Vector2.left, moveType, costList[-(int)checkPos.y, (int)checkPos.x]);
            CheckMoveAreaCostsRecursive(ref costList, startPos, checkPos + Vector2.right, moveType, costList[-(int)checkPos.y, (int)checkPos.x]);
            CheckMoveAreaCostsRecursive(ref costList, startPos, checkPos + Vector2.up, moveType, costList[-(int)checkPos.y, (int)checkPos.x]);
        }

        /// <summary>
        /// Checks the shoot root recursive.
        /// </summary>
        /// <returns>The shoot root recursive.</returns>
        /// <param name="costList">Cost list.</param>
        /// <param name="moveRoot">Move root.</param>
        /// <param name="startPos">Start position.</param>
        /// <param name="checkPos">Check position.</param>
        private List<Vector2> CheckShootRootRecursive(int[,] costList, ref List<Vector2> moveRoot, Vector2 startPos, Vector2 checkPos)
        {
            // 目的地についたら終了
            if (startPos == checkPos) return moveRoot;

            // 移動方向とコストのリスト
            var list = new List<Struct.NodeRoot>();

            // 上下左右の移動コストのチェック
            var checkUpPos = checkPos + Vector2.up;
            if (IsInField((int)checkUpPos.x, -(int)checkUpPos.y))
                if (costList[-(int)checkUpPos.y, (int)checkUpPos.x] != 0 || checkUpPos == startPos)
                    list.Add(new Struct.NodeRoot(Enums.MOVE.UP, costList[-(int)checkUpPos.y, (int)checkUpPos.x]));

            var checkDownPos = checkPos + Vector2.down;
            if (IsInField((int)checkDownPos.x, -(int)checkDownPos.y))
                if (costList[-(int)checkDownPos.y, (int)checkDownPos.x] != 0 || checkDownPos == startPos)
                    list.Add(new Struct.NodeRoot(Enums.MOVE.DOWN, costList[-(int)checkDownPos.y, (int)checkDownPos.x]));

            var checkLeftPos = checkPos + Vector2.left;
            if (IsInField((int)checkLeftPos.x, -(int)checkLeftPos.y))
                if (costList[-(int)checkLeftPos.y, (int)checkLeftPos.x] != 0 || checkLeftPos == startPos)
                    list.Add(new Struct.NodeRoot(Enums.MOVE.LEFT, costList[-(int)checkLeftPos.y, (int)checkLeftPos.x]));

            var checkRightPos = checkPos + Vector2.right;
            if (IsInField((int)checkRightPos.x, -(int)checkRightPos.y))
                if (costList[-(int)checkRightPos.y, (int)checkRightPos.x] != 0 || checkRightPos == startPos)
                    list.Add(new Struct.NodeRoot(Enums.MOVE.RIGHT, costList[-(int)checkRightPos.y, (int)checkRightPos.x]));

            // リストをコストの昇順で並び替えする
            list.Sort((a, b) => a.cost.CompareTo(b.cost));

            // もっともコストの低いマスを次のチェック対象とする
            switch (list[0].move)
            {
                case Enums.MOVE.UP:
                    moveRoot.Insert(0, Vector2.down);
                    CheckShootRootRecursive(costList, ref moveRoot, startPos, checkPos + Vector2.up);
                    break;

                case Enums.MOVE.DOWN:
                    moveRoot.Insert(0, Vector2.up);
                    CheckShootRootRecursive(costList, ref moveRoot, startPos, checkPos + Vector2.down);
                    break;

                case Enums.MOVE.LEFT:
                    moveRoot.Insert(0, Vector2.right);
                    CheckShootRootRecursive(costList, ref moveRoot, startPos, checkPos + Vector2.left);
                    break;

                case Enums.MOVE.RIGHT:
                    moveRoot.Insert(0, Vector2.left);
                    CheckShootRootRecursive(costList, ref moveRoot, startPos, checkPos + Vector2.right);
                    break;
            }
            return moveRoot;
        }

        /// <summary>
        /// フィールドの中ならTrueを返す
        /// </summary>
        /// <returns><c>true</c>, if outside field was ised, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public bool IsInField(int x, int y)
        {
            return 0 <= y && y < field.height &&
                 0 <= x && x < field.width
                ? true
                : false;
        }

        /// <summary>
        /// 移動タイプ毎の移動可能かどうかのチェック
        /// </summary>
        /// <returns><c>true</c>, if moveing was ised, <c>false</c> otherwise.</returns>
        /// <param name="cellCategory">Cell category.</param>
        /// <param name="moveType">Move type.</param>
        public bool IsMoveing(int cellCategory, Enums.MOVE_TYPE moveType)
        {
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

        /// <summary>
        /// アクティブエリア内にて攻撃できる特定の軍のユニットリストを返す
        /// </summary>
        /// <returns>The attack target list.</returns>
        /// <param name="activeAreaList">Active area list.</param>
        /// <param name="aRMY">A rmy.</param>
        public List<GameObject> GetAttackTargetList(Struct.NodeMove[,] activeAreaList, Enums.ARMY aRMY)
        {
            // 攻撃範囲内にいるプレイヤーUnit
            var targetList = new List<GameObject>();

            // アクティブエリア内にて攻撃できるプレイヤーUnitを取得
            for (int y = 0; y < field.height; y++)

                for (int x = 0; x < field.width; x++)
                    if (activeAreaList[y, x].aREA == Enums.AREA.MOVE ||
                        activeAreaList[y, x].aREA == Enums.AREA.ATTACK)
                        if (GameManager.GetUnit().GetMapUnitObj(new Vector2(x, -y)) &&
                            GameManager.GetUnit().GetMapUnitObj(new Vector2(x, -y)).GetComponent<UnitInfo>().aRMY == aRMY)
                            targetList.Add(GameManager.GetUnit().GetMapUnitObj(new Vector2(x, -y)));

            // 攻撃できる対象がいなければ終了
            return targetList.Count < 1 ? null : targetList;
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
            var simulationEvaluations = new List<Dictionary<string, int>>();
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
            }

            // 評価点リストを降順で並び替え
            simulationEvaluations.Sort((a, b) => b["evaluationPoint"] - a["evaluationPoint"]);

            return targetList[simulationEvaluations[0]["id"]];
        }

        /// <summary>
        /// ターゲット1が移動できる範囲内で、ターゲット2に攻撃できる場所のリストを返す
        /// </summary>
        /// <returns>The attack location list.</returns>
        /// <param name="activeAreaList">Active area list.</param>
        /// <param name="targetUnitObj1">Target unit obj1.</param>
        /// <param name="targetUnitObj2">Target unit obj2.</param>
        public List<Vector2> GetAttackLocationList(Struct.NodeMove[,] activeAreaList, GameObject targetUnitObj1, GameObject targetUnitObj2)
        {
            var targetUnitInfo1 = targetUnitObj1.GetComponent<UnitInfo>();
            var targetUnitPos2 = targetUnitObj2.transform.position;

            var attackLocationList = new List<Vector2>();

            // 下列の左側からチェックしていく
            for (int y = (int)targetUnitPos2.y - targetUnitInfo1.attackRange; y <= -(int)targetUnitPos2.y + targetUnitInfo1.attackRange; y++)
                for (int x = (int)targetUnitPos2.x - targetUnitInfo1.attackRange; x <= (int)targetUnitPos2.x + targetUnitInfo1.attackRange; x++)
                {
                    // 配列の外（マップ外）は飛ばす
                    if (!IsInField((int)x, -(int)y)) continue;

                    // 自分でない他ユニットがいるなら飛ばす
                    if (activeAreaList[-y, x].aREA != Enums.AREA.UNIT &&
                    GameManager.GetUnit().GetMapUnitObj(new Vector2(x, y)) != null) continue;

                    // 攻撃が届かない範囲を除く
                    if (targetUnitInfo1.attackRange < GameManager.GetCommonCalc().GetCellDistance(new Vector2(x, y), targetUnitPos2)) continue;

                    switch (activeAreaList[-y, x].aREA)
                    {
                        case Enums.AREA.MOVE:
                        case Enums.AREA.UNIT:
                            attackLocationList.Add(new Vector2(x, y));
                            break;
                    }
                }
            return attackLocationList;
        }

        /// <summary>
        /// ターゲットに攻撃できる場所リストの中から、一番条件の良い位置を選択する
        /// </summary>
        /// <returns>The attack location selection.</returns>
        /// <param name="locationList">Location list.</param>
        public Vector2 GetAttackLocationSelection(List<Vector2> locationList, GameObject targetUnitObj)
        {
            // 攻撃可能位置(セル)全てを評価する
            var simulationEvaluations = new List<Dictionary<string, int>>();
            Struct.CellInfo cellInfo;
            for (int i = 0; i < locationList.Count; i++)
            {
                // セルの評価
                cellInfo = GameManager.GetMap().field.cells[-(int)locationList[i].y, (int)locationList[i].x];

                // 評価ポイントの計算( 回避率 + 防御値 * 10 + 距離 * 10)
                simulationEvaluations.Add(new Dictionary<string, int>(){
                {"id",i},
                {"evaluationPoint", (int)(cellInfo.avoidanceBonus +
                 cellInfo.defenseBonus * 10 +
                GameManager.GetCommonCalc().GetCellDistance(locationList[i],targetUnitObj.transform.position) * 10)  }}
                    );
            }

            // 評価点リストを降順で並び替え
            simulationEvaluations.Sort((a, b) => b["evaluationPoint"] - a["evaluationPoint"]);

            // 一番評価の高い座標を返す
            return locationList[simulationEvaluations[0]["id"]];
        }

        /// <summary>
        /// 移動範囲内の中で、特定のユニットに近い移動場所を返す
        /// </summary>
        /// <returns>The near unit move position.</returns>
        /// <param name="targetUnitObj">Target unit object.</param>
        /// <param name="activeAreaList">Active area list.</param>
        public List<Vector2> GetNearUnitMovePos(GameObject targetUnitObj, Struct.NodeMove[,] activeAreaList)
        {
            // 一番近いPlayerUnitの座標を取得
            var list = NearUniMoveCosts(targetUnitObj, Enums.ARMY.ALLY);

            // 標的までのコストリストを取得
            var costList = GetMoveAreaCosts(list.First().Value);

            int endPosCost = -1;
            Vector2 endPos = Vector2.zero;
            for (int y = 0; y < GameManager.GetMap().field.height; y++)
                for (int x = 0; x < GameManager.GetMap().field.width; x++)
                    if (activeAreaList[y, x].aREA == Enums.AREA.MOVE &&
                   !GameManager.GetUnit().GetMapUnitObj(new Vector2(x, -y)))
                        if (costList[y, x] < endPosCost || endPosCost == -1)
                        {
                            // 移動できる最大値
                            endPosCost = costList[y, x];
                            endPos = new Vector2(x, -y);
                        }

            // 移動出来ないので空を返す
            if (endPos == Vector2.zero) return new List<Vector2>();

            // 目的地までの最短ルートを返す
            return GetMoveRoot(targetUnitObj, endPos);
        }
    }
}
