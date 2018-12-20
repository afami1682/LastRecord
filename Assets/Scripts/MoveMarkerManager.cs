using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarkerManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject moveMarkerObj;
    public GameObject markerObj;
    public Sprite[] markerSprites;

    void Start()
    {
        moveMarkerObj = new GameObject("MoveMarker");
        moveMarkerObj.transform.parent = transform;
    }

    /// <summary>
    /// markerの表示
    /// </summary>
    public void AddMarker(ref PhaseManager phaseManager, ref RouteManager routeManager, ref GameObject focusUnitObj, ref List<Vector3> moveRoot, ref Struct.NodeMove[,] activeAreaList, ref Vector3 cursorPos)
    {
        // アクティブエリアがあるなら、マーカを表示する
        if (activeAreaList != null)
            // 移動エリア内ならマーカを表示する
            if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.MOVE)
            {
                // マーカの削除
                RemoveMarker();

                // 目標までのルートを取得
                routeManager.CheckShortestRoute(ref phaseManager, cursorPos);

                // マーカの生成とスプライト変更
                Vector3 nextPos = focusUnitObj.transform.position;
                int spriteId = 0;
                Quaternion angle = Quaternion.identity;
                int moveRootCount = moveRoot.Count;
                if (moveRootCount != 0)
                {
                    if (moveRoot[0] == Vector3.down) angle.eulerAngles = new Vector3(180, 0, 0);
                    else if (moveRoot[0] == Vector3.left) angle.eulerAngles = new Vector3(0, 0, 90);
                    else if (moveRoot[0] == Vector3.right) angle.eulerAngles = new Vector3(0, 0, -90);
                    markerObj.GetComponent<SpriteRenderer>().sprite = markerSprites[spriteId];
                    Instantiate(markerObj, nextPos, angle).transform.parent = moveMarkerObj.transform;
                    Instantiate(markerObj, nextPos, angle).transform.parent = moveMarkerObj.transform;
                }
                for (int i = 0; i < moveRootCount; i++)
                {
                    if (moveRoot[i] == Vector3.up)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle = Quaternion.identity;
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.up)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.left ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle = Quaternion.identity;
                                spriteId = 1;
                            }
                        }
                    }
                    else if (moveRoot[i] == Vector3.down)
                    {
                        if (i + 1 == moveRootCount) { angle.eulerAngles = new Vector3(0, 0, 180); spriteId = 3; }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.down)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.left ? new Vector3(0, 0, 180) : new Vector3(180, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, 180);
                                spriteId = 1;
                            }
                        }

                    }
                    else if (moveRoot[i] == Vector3.right)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, -90);
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.right)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.up ? new Vector3(0, 180, 90) : new Vector3(0, 0, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, -90);
                                spriteId = 1;
                            }
                        }
                    }
                    else
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, 90);
                            spriteId = 3;
                        }
                        else
                        {
                            if (moveRoot[i + 1] != Vector3.left)
                            {
                                angle.eulerAngles = moveRoot[i + 1] == Vector3.up ? new Vector3(0, 0, 90) : new Vector3(0, 180, -90);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, 90);
                                spriteId = 1;
                            }
                        }
                    }
                    markerObj.GetComponent<SpriteRenderer>().sprite = markerSprites[spriteId];
                    Instantiate(markerObj, nextPos += moveRoot[i], angle).transform.parent = moveMarkerObj.transform;
                }
            }
            else if (activeAreaList[-(int)cursorPos.y, (int)cursorPos.x].aREA == Enums.AREA.UNIT)
                RemoveMarker(); // カーソルがユニット上なら表示しない
    }


    /// <summary>
    /// マーカの削除
    /// </summary>
    public void RemoveMarker() { foreach (Transform r in moveMarkerObj.transform) Destroy(r.gameObject); }

    /// <summary>
    /// マーカーの表示非表示
    /// </summary>
    /// <param name="val">If set to <c>true</c> value.</param>
    public void SetActive(bool val) { moveMarkerObj.SetActive(val); }

}
