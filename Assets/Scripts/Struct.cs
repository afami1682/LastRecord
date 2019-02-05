using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 構造体
/// </summary>
public class Struct
{
    /// <summary>
    /// ベースのフィールドデータ
    /// </summary>
    public struct FieldBase
    {
        public string name;
        public int width;
        public int height;
        public int[,] cells;
    }

    /// <summary>
    /// フィールドデータ
    /// </summary>
    public struct Field
    {
        public string name;
        public int width;
        public int height;
        public CellInfo[,] cells;
    }

    /// <summary>
    /// セル情報
    /// </summary>
    public struct CellInfo
    {
        public int cellId; // セルのId
        public string name; // 名前
        public int category; // 種類
        public int moveCost; // 移動コスト
        public int avoidanceBonus; // 回避ボーナス
        public int defenseBonus; // 防御ボーナス
        public int magicalDefenseBonus; // 魔防御ボーナス

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="category">Category.</param>
        /// <param name="moveCost">Move cost.</param>
        /// <param name="avoidanceBonus">Avoidance bonus.</param>
        /// <param name="defenseBonus">Defense bonus.</param>
        /// <param name="magicalDefenseBonus">Magical defense bonus.</param>
        public CellInfo(int cellId, string name, int category, int moveCost, int avoidanceBonus, int defenseBonus, int magicalDefenseBonus)
        {
            this.cellId = cellId;
            this.name = name;
            this.category = category;
            this.moveCost = moveCost;
            this.avoidanceBonus = avoidanceBonus;
            this.defenseBonus = defenseBonus;
            this.magicalDefenseBonus = magicalDefenseBonus;
        }
    }

    /// <summary>
    /// アクティブエリアを管理する際のノード
    /// </summary>
    public struct NodeMove
    {
        public int cost;
        public Enums.AREA aREA;
    }


    /// <summary>
    /// 移動ルートを計算する際のノード
    /// </summary>
    public struct NodeRoot
    {
        public Enums.MOVE move;
        public int cost;

        public NodeRoot(Enums.MOVE move, int cost)
        {
            this.move = move;
            this.cost = cost;
        }
    }

    /// <summary>
    /// ソート用
    /// </summary>
    public struct SortObj
    {
        public int val;
        public GameObject obj;

        public SortObj(int val, GameObject obj)
        {
            this.val = val;
            this.obj = obj;
        }
    }

    /// <summary>
    /// ユニット詳細画面のレーダーチャート表示用
    /// </summary>
    public struct NodeStatus
    {
        public string label; // ラベル
        public int val; // ステータスの値
        public int jobValMax; // Job毎のステータスの最大値

        public NodeStatus(string label, int val, int JobValMax)
        {
            this.label = label;
            this.val = val;
            this.jobValMax = JobValMax;
        }
    }

    /// <summary>
    /// ユニットのクラスデータ
    /// </summary>
    public struct UnitClassData
    {
        public string className; // クラス名
        public int vitality; // 体力
        public int attack; // 攻撃
        public int technical; // 技
        public int speed; // 速さ
        public int defense; // 防御
        public int resist; // 魔防
        public int luck; // 幸運
        public int move; // 移動
    }
}
