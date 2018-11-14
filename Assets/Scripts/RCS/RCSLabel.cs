using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// レーダーチャートのラベル表示
/// </summary>
public class RCSLabel : MonoBehaviour
{
    [HideInInspector]
    public float radius; // 半径
    [HideInInspector]
    public int statusListCount; // ステータスの数
    [HideInInspector]
    public List<NodeStatus> statusList;
    [HideInInspector]
    public int statusValMax; //ステータス値の最大値
    public GameObject textPref;

    float margin = 0.6f;

    /// <summary>
    /// Creates the text.
    /// </summary>
    public void CreateText()
    {
        radius += margin;

        //各頂点座標
        Vector2 p;
        GameObject textObj;
        Text text;
        for (int i = 1; i <= statusListCount; i++)
        {
            float rad = (90f - (360f / (float)statusListCount) * (i - 1)) * Mathf.Deg2Rad;
            float x = 0.5f + Mathf.Cos(rad) * statusValMax * radius;
            float y = 0.5f + Mathf.Sin(rad) * statusValMax * radius;

            // 各ステータスラベルの生成
            textObj = Instantiate(textPref) as GameObject;
            text = textObj.GetComponent<Text>();
            text.text = statusList[i - 1].label;
            textObj.transform.SetParent(transform, false);
            textObj.transform.localScale = Vector3.one;

            // 表示位置
            p = Vector2.zero;
            p.x -= text.rectTransform.pivot.x;
            p.y -= text.rectTransform.pivot.y;
            p.x += x;
            p.y += y;
            textObj.transform.localPosition = new Vector3(p.x, p.y, 0);
        }
    }
}