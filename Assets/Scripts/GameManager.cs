﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapId; // マップID
    static MapManager mapManager;
    static UnitManager unitManager;
    static RouteManager routeManager;
    static EnemyAIManager enemyAIManager;
    static AI.CommonCalc aICommonCalc;
    static CommonCalc commonCalc;

    private void Awake()
    {
        // 各マネージャーの初期化
        mapManager = new MapManager(mapId);
        unitManager = new UnitManager(mapManager.field);
        routeManager = new RouteManager(mapManager.field);
        enemyAIManager = new EnemyAIManager(mapManager.field);
        aICommonCalc = new AI.CommonCalc(mapManager.field);
        commonCalc = new CommonCalc();
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

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The enemy ai.</returns>
    public static EnemyAIManager GetEnemyAI() { return enemyAIManager; }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The AIC ommon calculate.</returns>
    public static AI.CommonCalc GetAICommonCalc() { return aICommonCalc; }

    /// <summary>
    /// 外部呼び出し用
    /// </summary>
    /// <returns>The calculate.</returns>
    public static CommonCalc GetCommonCalc() { return commonCalc; }
}
