using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レベルアップ時の+1ラベル
/// </summary>
public class RCSLevelUpLabel : MonoBehaviour
{

    public Color val; // テキストカラー
    const int MIN_VAL = 20; // 表示位置の最小値

    [HideInInspector]
    public float radius; // 半径
    [HideInInspector]
    public int statusListCount; // ステータスの数
    [HideInInspector]
    public Struct.NodeStatus[] statusList;
    [HideInInspector]
    public int statusValMax; //ステータス値の最大値
    public GameObject textPref;

    float margin = 0.8f;
     float marginX = 50f;

    /// <summary>
    /// Creates the text.
    /// </summary>
    public void CreateText(int idx, int addVal)
    {
        //各頂点座標
        Vector2 p;
        GameObject textObj;
        Text text;

        float rad = (90f - (360f / (float)statusListCount) * (idx)) * Mathf.Deg2Rad;
        float x = 0.5f + Mathf.Cos(rad) * statusValMax * (radius + margin);
        float y = 0.5f + Mathf.Sin(rad) * statusValMax * (radius + margin);

        // 各ステータスラベルの生成
        textObj = Instantiate(textPref) as GameObject;
        text = textObj.GetComponent<Text>();
        //text.text = string.Format("+ {0}", addVal);
        text.text = "↑";
        textObj.transform.SetParent(transform, false);
        textObj.transform.localScale = Vector3.one;

        // ラベルの色
        text.color = val;

        // 表示位置
        p = Vector2.zero;
        p.x += x;
        //if (idx <= statusListCount / 2)
        //{
            p.x += marginX;
        //}
        //else
        //{
        //    p.x -= marginX;
        //}
        p.y += y;
        textObj.transform.localPosition = new Vector3(p.x, p.y, 0);
    }

    /// <summary>
    ///ラベルの削除
    /// </summary>
    public void Reset()
    {
        foreach (Transform n in transform)
        {
            GameObject.Destroy(n.gameObject);
        }
    }

    /// <summary>
    /// 値のセット
    /// </summary>
    /// <returns>The volume.</returns>
    /// <param name="idx">Index.</param>
    private float GetVolume(int idx)
    {
        if (statusListCount - 1 < idx) return 0;
        Struct.NodeStatus v = statusList[idx]; // ステータス値の取得
        v.val = v.val > MIN_VAL ? v.val : MIN_VAL; // MIN_VAL未満ならMIN_VALとする
        v.val = v.val > statusValMax ? statusValMax : v.val; // 最大値より大きい値は返さない
        return v.val;
    }
}