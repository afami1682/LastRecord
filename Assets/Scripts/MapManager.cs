using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MapManager
{
    static Struct.Field field;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="mapId">ステージId</param>
    public MapManager(int mapId)
    {
        Struct.FieldBase fieldBase = new Struct.FieldBase();
        switch (mapId)
        {
            case 1:
                StageData1 stageData = new StageData1();
                fieldBase = stageData.getFieldData();
                break;
            default:
                Debug.Log("マップデータが読み込まれてません");
                break;
        }

        // フィールドデータの読み込み
        field = new Struct.Field();
        field.name = fieldBase.name;
        field.width = fieldBase.width;
        field.height = fieldBase.height;
        field.cells = new Struct.CellInfo[field.height, field.width];

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
                        field.cells[y, x] = new Struct.CellInfo("平地", 0, 1, 0, 0, 0);
                        break;
                    case 1:
                        field.cells[y, x] = new Struct.CellInfo("草むら", 0, 2, 10, 0, 0);
                        break;
                    case 2:
                        field.cells[y, x] = new Struct.CellInfo("壁", 1, 1, 5, 1, 0);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 外部からの呼び出し用
    /// </summary>
    /// <returns>The field data.</returns>
    public static Struct.Field GetFieldData()
    {
        return field;
    }
}