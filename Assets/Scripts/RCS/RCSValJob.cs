using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// レーダーチャートの表示（Jobステータスの最大値）
/// </summary>
public class RCSValJob : Graphic
{
    [HideInInspector]
    public float radius; // 半径
    [HideInInspector]
    public List<Struct.NodeStatus> statusJobList; // ステータスリスト
    [HideInInspector]
    public int statusListCount; // ステータスの数
    [HideInInspector]
    public int statusValMax; //ステータス値の最大値

    /// <summary>
    /// 描画
    /// </summary>
    /// <param name="vh">Vh.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); // 描画のクリア
        var v = UIVertex.simpleVert; // 各頂点の情報
        v.color = color; // 色

        // 中心座標の登録
        Vector2 center = CreatePos(0.5f, 0.5f);
        v.position = center;
        vh.AddVert(v);

        //各頂点座標
        Vector2 p;
        for (int i = 1; i <= statusListCount; i++)
        {
            float rad = (90f - (360f / (float)statusListCount) * (i - 1)) * Mathf.Deg2Rad;
            float x = 0.5f + Mathf.Cos(rad) * GetVolume(i - 1) * radius;
            float y = 0.5f + Mathf.Sin(rad) * GetVolume(i - 1) * radius;

            p = CreatePos(x, y);
            v.position = p;
            vh.AddVert(v);
            vh.AddTriangle(
                0,
                i,
                i == statusListCount ? 1 : i + 1
            );
        }
    }

    /// <summary>
    /// 頂点の作成
    /// </summary>
    /// <returns>The position.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private Vector2 CreatePos(float x, float y)
    {
        Vector2 p = Vector2.zero;
        p.x -= rectTransform.pivot.x;
        p.y -= rectTransform.pivot.y;
        p.x += x;
        p.y += y;
        //p.x *= rectTransform.rect.width;
        //p.y *= rectTransform.rect.height;
        return p;
    }

    /// <summary>
    /// 値のセット
    /// </summary>
    /// <returns>The volume.</returns>
    /// <param name="idx">Index.</param>
    private float GetVolume(int idx)
    {
        if (statusListCount - 1 < idx) return 0;
        Struct.NodeStatus v = statusJobList[idx]; // ステータス値の取得
        return v.jobValMax > statusValMax ? statusValMax : v.jobValMax; // 最大値より大きい値は返さない
    }
}
