using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MapManager : MonoBehaviour
{
    static Field field;

    // 参照渡しで受け取ったフィールデータを更新
    public void LoadData(int mapId)
    {
        FieldBase fieldBase = new FieldBase();
        switch (mapId)
        {
            case 1:
                MapData1 mapData1 = gameObject.AddComponent<MapData1>();
                fieldBase = mapData1.getData();
                break;
            default:
                Debug.Log("マップデータが読み込まれてません");
                break;
        }

        // フィールドデータの読み込み
        field = new Field();
        field.name = fieldBase.name;
        field.width = fieldBase.width;
        field.height = fieldBase.height;
        field.cells = new CellInfo[field.height, field.width];

        // 各セルデータの追加
        for (int y = 0; y < field.height; y++)
        {
            for (int x = 0; x < field.width; x++)
            {
                switch (fieldBase.cells[y, x])
                {
                    // name, category, moveCost, defenseBonus, avoidanceBonus, recoveryBonus; 
                    case 0:
                    default:
                        field.cells[y, x] = new CellInfo("平地", 0, 1, 0, 0, 0);
                        break;
                    case 1:
                        field.cells[y, x] = new CellInfo("草むら", 0, 2, 10, 0, 0);
                        break;
                    case 2:
                        field.cells[y, x] = new CellInfo("壁", 1, 1, 5, 1, 0);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 外部からの呼び出し用
    /// </summary>
    /// <returns>The field data.</returns>
    public static Field GetFieldData()
    {
        return field;
    }
}

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
    public string name; // 名前
    public int category; // 種類
    public int moveCost; // 移動コスト
    public int avoidanceBonus; // 回避ボーナス
    public int defenseBonus; // 防御ボーナス
    public int magicalDefenseBonus; // 魔防御ボーナス

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="category">Category.</param>
    /// <param name="moveCost">Move cost.</param>
    /// <param name="avoidanceBonus">Avoidance bonus.</param>
    /// <param name="defenseBonus">Defense bonus.</param>
    /// <param name="magicalDefenseBonus">Magical defense bonus.</param>
    public CellInfo(string name, int category, int moveCost, int avoidanceBonus, int defenseBonus, int magicalDefenseBonus)
    {
        this.name = name;
        this.category = category;
        this.moveCost = moveCost;
        this.avoidanceBonus = avoidanceBonus;
        this.defenseBonus = defenseBonus;
        this.magicalDefenseBonus = magicalDefenseBonus;
    }
}