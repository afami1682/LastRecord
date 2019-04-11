using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    // ルートの算出に必要なフィールドデータ
    int fieldWidth, fieldHeight;

    GameObject[,] mapUnitObj;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="field">Field.</param>
    public UnitManager(Struct.Field field)
    {
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
    public GameObject GetMapUnitObj(Vector2 pos)
    {
        return mapUnitObj[-(int)pos.y, (int)pos.x];
    }

    /// <summary>
    /// 配置リスト上の特定の座標のユニット情報を返す
    /// </summary>
    /// <returns>The map unit info.</returns>
    /// <param name="pos">Position.</param>
    public UnitInfo GetMapUnitInfo(Vector2 pos)
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
    public void AddMapUnitObj(Vector2 pos, GameObject gameObject)
    {
        mapUnitObj[-(int)pos.y, (int)pos.x] = gameObject;
    }

    /// <summary>
    /// 配置リスト上でユニット情報を移動する
    /// </summary>
    /// <param name="oldPos">Old position.</param>
    /// <param name="newPos">New position.</param>
    public void MoveMapUnitObj(Vector2 oldPos, Vector2 newPos)
    {
        mapUnitObj[-(int)newPos.y, (int)newPos.x] = mapUnitObj[-(int)oldPos.y, (int)oldPos.x];
        if (oldPos != newPos) mapUnitObj[-(int)oldPos.y, (int)oldPos.x] = null;
    }

    /// <summary>
    /// 配置リスト上のユニット情報を削除する
    /// </summary>
    /// <param name="pos">Position.</param>
    public void RemoveMapUnitObj(Vector2 pos)
    {
        mapUnitObj[-(int)pos.y, (int)pos.x] = null;
    }

    /// <summary>
    /// 指定された軍のユニットリストを返す
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="unitKind">unitKind.</param>
    public List<GameObject> GetUnitList(Enums.UNIT_KIND unitKind)
    {
        List<GameObject> units = new List<GameObject>();
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null && mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().UnitKind == unitKind)
                    units.Add(mapUnitObj[y, x]);
        return units;
    }

    /// <summary>
    /// 指定された軍の未行動ユニットがいるかどうかチェックし、ランダムの1体を返す
    /// </summary>
    /// <returns>The un behavior unit.</returns>
    /// <param name="unitKind">unitKind.</param>
    public GameObject GetUnBehaviorRandomUnit(Enums.UNIT_KIND unitKind)
    {
        List<GameObject> units = new List<GameObject>();
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null &&
                    mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().UnitKind == unitKind &&
                    !mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().Acted)
                    units.Add(mapUnitObj[y, x]);
        return (units.Count != 0)? units[Random.Range(0, units.Count - 1)]: null;
    }

    /// <summary>
    /// 指定された軍の未行動ユニットがいるかどうかのチェック
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="unitKind">unitKind.</param>
    public List<GameObject> GetUnBehaviorUnits(Enums.UNIT_KIND unitKind)
    {
        List<GameObject> units = new List<GameObject>();
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null &&
                    mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().UnitKind == unitKind &&
                    !mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().Acted)
                    units.Add(mapUnitObj[y, x]);
        return units;
    }

    /// <summary>
    /// 指定された軍のユニットを全て未行動にする
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="unitKind">unitKind.</param>
    public void UnBehaviorUnitAll(Enums.UNIT_KIND unitKind)
    {
        for (int y = 0; y < fieldHeight; y++)
            for (int x = 0; x < fieldWidth; x++)
                if (mapUnitObj[y, x] != null &&
                    mapUnitObj[y, x].gameObject.GetComponent<UnitInfo>().UnitKind == unitKind)
                {
                    mapUnitObj[y, x].GetComponent<UnitInfo>().Acted = false;
                    mapUnitObj[y, x].GetComponent<UnitEffectController>().GrayScale(false);
                }
    }
}
