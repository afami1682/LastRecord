using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        public int mapId = 0; // マップID
        public MapManager mapData;

        // マップに配置してるUnitオブジェクト
        static GameObject[,] mapUnitObj;

        private void Awake()
        {
            // マップデータの読み込み
            mapData = new MapManager(mapId);

            // ユニットの配置リストの初期化
            mapUnitObj = new GameObject[MapManager.GetFieldData().height,
                                                   MapManager.GetFieldData().width];
        }

        /// <summary>
        /// 配置リスト上の特定の座標のユニットオブジェクトを返す
        /// </summary>
        /// <returns>The map unit object.</returns>
        /// <param name="pos">Position.</param>
        public static GameObject GetMapUnitObj(Vector3 pos)
        {
            return mapUnitObj[-(int)pos.y, (int)pos.x];
        }

        /// <summary>
        /// 配置リスト上の特定の座標のユニット情報を返す
        /// </summary>
        /// <returns>The map unit info.</returns>
        /// <param name="pos">Position.</param>
        public static UnitInfo GetMapUnitInfo(Vector3 pos)
        {
            return mapUnitObj[-(int)pos.y, (int)pos.x] != null ? mapUnitObj[-(int)pos.y, (int)pos.x].GetComponent<UnitInfo>() : null;
        }

        /// <summary>
        /// ユニットの配置リストを返す
        /// </summary>
        /// <returns>The map unit object.</returns>
        public static GameObject[,] GetMapUnitObj()
        {
            return mapUnitObj;
        }

        /// <summary>
        /// 配置リストにユニット情報を登録する
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="gameObject">Game object.</param>
        public static void AddMapUnitObj(Vector3 pos, GameObject gameObject)
        {
            mapUnitObj[-(int)pos.y, (int)pos.x] = gameObject;
        }

        /// <summary>
        /// 配置リスト上でユニット情報を移動する
        /// </summary>
        /// <param name="oldPos">Old position.</param>
        /// <param name="newPos">New position.</param>
        public static void MoveMapUnitObj(Vector3 oldPos, Vector3 newPos)
        {
            mapUnitObj[-(int)newPos.y, (int)newPos.x] = mapUnitObj[-(int)oldPos.y, (int)oldPos.x];
            mapUnitObj[-(int)oldPos.y, (int)oldPos.x] = null;
        }

        /// <summary>
        /// 配置リスト上のユニット情報を削除する
        /// </summary>
        /// <param name="pos">Position.</param>
        public static void RemoveMapUnitObj(Vector3 pos)
        {
            mapUnitObj[-(int)pos.y, (int)pos.x] = null;
        }
    }
}
