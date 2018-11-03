using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapId = 0; // マップID
    public MapManager mapManager;

    // マップ上のユニット配置リスト
    static UnitInfo[,] mapUnitData;

    private void Awake()
    {
        // マップデータの読み込み
        mapManager.LoadData(mapId);

        // ユニットの配置リストの初期化
        mapUnitData = new UnitInfo[MapManager.GetFieldData().height,
                                               MapManager.GetFieldData().width];
    }

    public static UnitInfo GetMapUnit(Vector3 pos)
    {
        return mapUnitData[-(int)pos.y, (int)pos.x];
    }

    public static UnitInfo[,] GetMapUnitData()
    {
        return mapUnitData;
    }

    public static void AddMapUnitData(Vector3 pos, UnitInfo unitInfo)
    {
        mapUnitData[-(int)pos.y, (int)pos.x] = unitInfo;
    }

    public static void MoveMapUnitData(Vector3 oldPos, Vector3 newPos)
    {
        mapUnitData[-(int)newPos.y, (int)newPos.x] = mapUnitData[-(int)oldPos.y, (int)oldPos.x];
        mapUnitData[-(int)oldPos.y, (int)oldPos.x] = null;
    }

    public static void RemoveMapUnitData(Vector3 pos)
    {
        mapUnitData[-(int)pos.y, (int)pos.x] = null;
    }
}
