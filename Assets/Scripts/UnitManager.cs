using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    // ルートの算出に必要なフィールドデータ
    Struct.Field field;
    int fieldWidth, fieldHeight;

    GameObject[,] mapUnitObj;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public UnitManager(Struct.Field field)
    {
        this.field = field;
        this.fieldWidth = field.width;
        this.fieldHeight = field.height;

        // ユニットの配置リストの初期化
        mapUnitObj = new GameObject[fieldHeight, fieldWidth];
    }

    /// <summary>
    /// 配置リスト上の特定の座標のユニットオブジェクトを返す
    /// </summary>
    /// <returns>The map unit object.</returns>
    /// <param name="pos">Position.</param>
    public GameObject GetMapUnitObj(Vector3 pos)
    {
        return mapUnitObj[-(int)pos.y, (int)pos.x];
    }

    /// <summary>
    /// 配置リスト上の特定の座標のユニット情報を返す
    /// </summary>
    /// <returns>The map unit info.</returns>
    /// <param name="pos">Position.</param>
    public UnitInfo GetMapUnitInfo(Vector3 pos)
    {
        return mapUnitObj[-(int)pos.y, (int)pos.x] != null ? mapUnitObj[-(int)pos.y, (int)pos.x].GetComponent<UnitInfo>() : null;
    }

    /// <summary>
    /// ユニットの配置リストを返す
    /// </summary>
    /// <returns>The map unit object.</returns>
    public GameObject[,] GetMapUnitObj()
    {
        return mapUnitObj;
    }

    /// <summary>
    /// 配置リストにユニット情報を登録する
    /// </summary>
    /// <param name="pos">Position.</param>
    /// <param name="gameObject">Game object.</param>
    public void AddMapUnitObj(Vector3 pos, GameObject gameObject)
    {
        mapUnitObj[-(int)pos.y, (int)pos.x] = gameObject;
    }

    /// <summary>
    /// 配置リスト上でユニット情報を移動する
    /// </summary>
    /// <param name="oldPos">Old position.</param>
    /// <param name="newPos">New position.</param>
    public void MoveMapUnitObj(Vector3 oldPos, Vector3 newPos)
    {
        mapUnitObj[-(int)newPos.y, (int)newPos.x] = mapUnitObj[-(int)oldPos.y, (int)oldPos.x];
        mapUnitObj[-(int)oldPos.y, (int)oldPos.x] = null;
    }

    /// <summary>
    /// 配置リスト上のユニット情報を削除する
    /// </summary>
    /// <param name="pos">Position.</param>
    public void RemoveMapUnitObj(Vector3 pos)
    {
        mapUnitObj[-(int)pos.y, (int)pos.x] = null;
    }

    /// <summary>
    /// 指定された軍のユニットリストを返す
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="army">Army.</param>
    public List<GameObject> GetUnitList(Enums.ARMY army)
    {
        List<GameObject> units = new List<GameObject>();
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null && mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().aRMY == army)
                    units.Add(mapUnitObj[y, x]);
        return units;
    }

    /// <summary>
    /// 指定された軍の未行動ユニットがいるかどうかのチェック
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="army">Army.</param>
    public List<GameObject> GetUnBehaviorUnits(Enums.ARMY army)
    {
        List<GameObject> units = new List<GameObject>();
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null &&
                    mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().aRMY == army &&
                    !mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().isMoving())
                    units.Add(mapUnitObj[y, x]);
        return units;
    }

    /// <summary>
    /// 指定された軍の未行動ユニットがいるかどうかのチェック
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="army">Army.</param>
    //public static GameObject GetRandomUnBehaviorUnit(Enums.ARMY army)
    //{
    //    List<GameObject> unit = new List<GameObject>();
    //    for (int y = 0; y < MapManager.GetFieldData().height; y++)
    //        for (int x = 0; x < MapManager.GetFieldData().width; x++)
    //            if (mapUnitObj[y, x] != null &&
    //                mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().aRMY == army &&
    //                !mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().isMoving())
    //                units.Add(mapUnitObj[y, x]);
    //    return unit;
    //}

    /// <summary>
    /// 指定された軍のユニットを全て未行動にする
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="army">Army.</param>
    public void UnBehaviorUnitAll(Enums.ARMY army)
    {
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null &&
                    mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().aRMY == army)
                {
                    mapUnitObj[y, x].GetComponent<UnitInfo>().Moving(false);
                    mapUnitObj[y, x].GetComponent<EffectController>().GrayScale(false);
                }
    }
}
