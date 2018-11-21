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
        mapManager = new MapManager(mapId);

        // ユニットの配置リストの初期化
        mapUnitData = new UnitInfo[MapManager.GetFieldData().height,
                                               MapManager.GetFieldData().width];
    }

    /// <summary>
    /// 配置リスト上の特定の座標のユニット情報を返す
    /// </summary>
    /// <returns>The map unit.</returns>
    /// <param name="pos">Position.</param>
    public static UnitInfo GetMapUnit(Vector3 pos)
    {
        return mapUnitData[-(int)pos.y, (int)pos.x];
    }

    /// <summary>
    /// ユニットの配置リストを返す
    /// </summary>
    /// <returns>The map unit data.</returns>
    public static UnitInfo[,] GetMapUnitData()
    {
        return mapUnitData;
    }

    /// <summary>
    /// 配置リストにユニット情報を登録する
    /// </summary>
    /// <param name="pos">Position.</param>
    /// <param name="unitInfo">Unit info.</param>
    public static void AddMapUnitData(Vector3 pos, UnitInfo unitInfo)
    {
        mapUnitData[-(int)pos.y, (int)pos.x] = unitInfo;
    }

    /// <summary>
    /// 配置リスト上でユニット情報を移動する
    /// </summary>
    /// <param name="oldPos">Old position.</param>
    /// <param name="newPos">New position.</param>
    public static void MoveMapUnitData(Vector3 oldPos, Vector3 newPos)
    {
        mapUnitData[-(int)newPos.y, (int)newPos.x] = mapUnitData[-(int)oldPos.y, (int)oldPos.x];
        mapUnitData[-(int)oldPos.y, (int)oldPos.x] = null;
    }

    /// <summary>
    /// 配置リスト上のユニット情報を削除する
    /// </summary>
    /// <param name="pos">Position.</param>
    public static void RemoveMapUnitData(Vector3 pos)
    {
        mapUnitData[-(int)pos.y, (int)pos.x] = null;
    }
}
