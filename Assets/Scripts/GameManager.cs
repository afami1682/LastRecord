using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapId = 0; // マップID
    static MapManager mapManager;
    static UnitManager unitManager;
    static RouteManager routeManager;

    private void Awake()
    {
        // 各マネージャーの初期化
        mapManager = new MapManager(mapId);
        unitManager = new UnitManager(mapManager.field);
        routeManager = new RouteManager(mapManager.field);
    }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The map.</returns>
    public static MapManager GetMap() { return mapManager; }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The unit.</returns>
    public static UnitManager GetUnit() { return unitManager; }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The route.</returns>
    public static RouteManager GetRoute() { return routeManager; }
}
