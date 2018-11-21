using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// レーダーチャートのラベル表示
/// </summary>
public class RCSLabelVal : MonoBehaviour
{
    public Color val; // 通常時のテキストカラー
    public Color max; // カンスト時のテキストカラー
    const int MIN_VAL = 20; // 表示位置の最小値

    [HideInInspector]
    public float radius; // 半径
    [HideInInspector]
    public int statusListCount; // ステータスの数
    [HideInInspector]
    public List<Struct.NodeStatus> statusList;
    [HideInInspector]
    public int statusValMax; //ステータス値の最大値
    public GameObject textPref;

    /// <summary>
    /// Creates the text.
    /// </summary>
    public void CreateText()
    {
        //各頂点座標
        Vector2 p;
        GameObject textObj;
        Text text;
        for (int i = 1; i <= statusListCount; i++)
        {
            float rad = (90f - (360f / (float)statusListCount) * (i - 1)) * Mathf.Deg2Rad;
            float x = 0.5f + Mathf.Cos(rad) * GetVolume(i - 1) * radius;
            float y = 0.5f + Mathf.Sin(rad) * GetVolume(i - 1) * radius;

            // 各ステータスラベルの生成
            textObj = Instantiate(textPref) as GameObject;
            text = textObj.GetComponent<Text>();
            text.text = statusList[i - 1].val.ToString();
            textObj.transform.SetParent(transform, false);
            textObj.transform.localScale = Vector3.one;

            // ラベルの色
            text.color = statusList[i - 1].val >= statusList[i - 1].jobValMax ? max : val;

            // 表示位置
            p = Vector2.zero;
            p.x += x;
            p.y += y;
            textObj.transform.localPosition = new Vector3(p.x, p.y, 0);
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