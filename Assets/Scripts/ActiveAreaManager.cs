using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAreaManager : MonoBehaviour
{
    // エリア描画用関連
    [HideInInspector]
    public GameObject attackAreaObj, activeAreaObj;
    public GameObject areaBlue;
    public GameObject areaRed;

    [HideInInspector]
    public Struct.NodeMove[,] activeAreaList; // 行動(移動、攻撃）可能エリアを管理する配列
    public Struct.NodeMove[,] attackAreaList; // 攻撃可能エリアを管理する配列

    void Start()
    {
        // エリア描画用関連の読み込み
        attackAreaObj = new GameObject("AttackArea");
        activeAreaObj = new GameObject("ActiveArea");

        attackAreaObj.transform.parent = transform;
        activeAreaObj.transform.parent = transform;
    }

    /// <summary>
    /// アクティブエリアの生成と表示
    /// </summary>
    /// <param name="checkUnitObj">Check unit object.</param>
    /// <param name="showArea">If set to <c>true</c> show area.</param>
    public void CreateActiveArea(GameObject checkUnitObj, bool showArea)
    {
        // アクティブリストの生成と検証
        activeAreaList = new Struct.NodeMove[GameManager.GetMap().field.height, GameManager.GetMap().field.width];

        // 移動エリアの検証
        GameManager.GetRoute().CheckMoveArea(ref activeAreaList, checkUnitObj);

        // エリアパネルの表示
        for (int y = 0; y < GameManager.GetMap().field.height; y++)
            for (int x = 0; x < GameManager.GetMap().field.width; x++)
                if (activeAreaList[y, x].aREA == Enums.AREA.MOVE || activeAreaList[y, x].aREA == Enums.AREA.UNIT)
                {
                    // 移動エリアの表示
                    if (showArea) Instantiate(areaBlue, new Vector3(x, -y, 0), Quaternion.identity).transform.parent = activeAreaObj.transform;

                    // 攻撃エリアの検証
                    GameManager.GetRoute().CheckAttackArea(ref activeAreaList, new Vector3(x, -y, 0), checkUnitObj.GetComponent<UnitInfo>().AttackRange);
                }

        // 攻撃エリアの表示
        if (showArea)
            for (int ay = 0; ay < GameManager.GetMap().field.height; ay++)
                for (int ax = 0; ax < GameManager.GetMap().field.width; ax++)
                    if (activeAreaList[ay, ax].aREA == Enums.AREA.ATTACK)
                        Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = activeAreaObj.transform;
    }

    /// <summary>
    /// 攻撃エリアの表示
    /// </summary>
    /// <param name="targetUnitObj">Target unit object.</param>
    public void CreateAttackArea(GameObject targetUnitObj, bool showArea)
    {
        // アクティブリストの生成と検証
        attackAreaList = new Struct.NodeMove[GameManager.GetMap().field.height, GameManager.GetMap().field.width];
        GameManager.GetRoute().CheckAttackArea(ref attackAreaList, targetUnitObj.transform.position, targetUnitObj.GetComponent<UnitInfo>().AttackRange);

        // 攻撃エリアの表示
        if (showArea)
            for (int ay = 0; ay < GameManager.GetMap().field.height; ay++)
                for (int ax = 0; ax < GameManager.GetMap().field.width; ax++)
                    if (attackAreaList[ay, ax].aREA == Enums.AREA.ATTACK)
                        Instantiate(areaRed, new Vector3(ax, -ay, 0), Quaternion.identity).transform.parent = attackAreaObj.transform;
    }

    /// <summary>
    /// 行動エリアの初期化と削除
    /// </summary>
    public void RemoveActiveArea()
    {
        activeAreaList = null;
        foreach (Transform a in activeAreaObj.transform) Destroy(a.gameObject);
    }

    /// <summary>
    /// 攻撃エリアの初期化と削除
    /// </summary>
    public void RemoveAttackArea()
    {
        attackAreaList = null;
        foreach (Transform a in attackAreaObj.transform) Destroy(a.gameObject);
    }
}
