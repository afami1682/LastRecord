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
    public void AddMarker(ref PhaseManager phaseManager)
    {
        // アクティブエリアがあるなら、マーカを表示する
        if (phaseManager.activeAreaManager.activeAreaList != null)
            // 移動エリア内ならマーカを表示する
            if (phaseManager.activeAreaManager.activeAreaList[-(int)phaseManager.cursorPos.y, (int)phaseManager.cursorPos.x].aREA == Enums.AREA.MOVE)
            {
                // マーカの削除
                RemoveMarker();

                // 目標までのルートを取得
                phaseManager.moveRoot =
                GameManager.GetRoute().CheckShortestRoute(
                    phaseManager.activeAreaManager.activeAreaList,
                    phaseManager.focusUnitObj.transform.position,
                    phaseManager.cursorPos);

                // マーカの生成とスプライト変更
                Vector2 nextPos = phaseManager.focusUnitObj.transform.position;
                int spriteId = 0;
                Quaternion angle = Quaternion.identity;
                int moveRootCount = phaseManager.moveRoot.Count;
                if (moveRootCount != 0)
                {
                    if (phaseManager.moveRoot[0] == Vector2.down) angle.eulerAngles = new Vector3(180, 0, 0);
                    else if (phaseManager.moveRoot[0] == Vector2.left) angle.eulerAngles = new Vector3(0, 0, 90);
                    else if (phaseManager.moveRoot[0] == Vector2.right) angle.eulerAngles = new Vector3(0, 0, -90);
                    markerObj.GetComponent<SpriteRenderer>().sprite = markerSprites[spriteId];
                    Instantiate(markerObj, nextPos, angle).transform.parent = moveMarkerObj.transform;
                }
                for (int i = 0; i < moveRootCount; i++)
                {
                    if (phaseManager.moveRoot[i] == Vector2.up)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle = Quaternion.identity;
                            spriteId = 3;
                        }
                        else
                        {
                            if (phaseManager.moveRoot[i + 1] != Vector2.up)
                            {
                                angle.eulerAngles = phaseManager.moveRoot[i + 1] == Vector2.left ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle = Quaternion.identity;
                                spriteId = 1;
                            }
                        }
                    }
                    else if (phaseManager.moveRoot[i] == Vector2.down)
                    {
                        if (i + 1 == moveRootCount) { angle.eulerAngles = new Vector3(0, 0, 180); spriteId = 3; }
                        else
                        {
                            if (phaseManager.moveRoot[i + 1] != Vector2.down)
                            {
                                angle.eulerAngles = phaseManager.moveRoot[i + 1] == Vector2.left ? new Vector3(0, 0, 180) : new Vector3(180, 0, 0);
                                spriteId = 2;
                            }
                            else
                            {
                                angle.eulerAngles = new Vector3(0, 0, 180);
                                spriteId = 1;
                            }
                        }

                    }
                    else if (phaseManager.moveRoot[i] == Vector2.right)
                    {
                        if (i + 1 == moveRootCount)
                        {
                            angle.eulerAngles = new Vector3(0, 0, -90);
                            spriteId = 3;
                        }
                        else
                        {
                            if (phaseManager.moveRoot[i + 1] != Vector2.right)
                            {
                                angle.eulerAngles = phaseManager.moveRoot[i + 1] == Vector2.up ? new Vector3(0, 180, 90) : new Vector3(0, 0, -90);
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
                            if (phaseManager.moveRoot[i + 1] != Vector2.left)
                            {
                                angle.eulerAngles = phaseManager.moveRoot[i + 1] == Vector2.up ? new Vector3(0, 0, 90) : new Vector3(0, 180, -90);
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
                    Instantiate(markerObj, nextPos += phaseManager.moveRoot[i], angle).transform.parent = moveMarkerObj.transform;
                }
            }
            else if (phaseManager.activeAreaManager.activeAreaList[-(int)phaseManager.cursorPos.y, (int)phaseManager.cursorPos.x].aREA == Enums.AREA.UNIT)
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
