using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レーダーチャートの罫線を描画
/// </summary>
public class RCSLine : Graphic
{
    [HideInInspector]
    public float radius; // 半径
    [HideInInspector]
    public int statusListCount; // ステータスの数
    [HideInInspector]
    public int statusValMax; //ステータス値の最大値
    [HideInInspector]
    public float LineWidth; // 線幅

    /// <summary>
    /// 描画
    /// </summary>
    /// <param name="vh">Vh.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        var v = UIVertex.simpleVert;
        v.color = color;

        //Outer Frame
        this.DrawFrame(vh, statusValMax);

        //Axis
        this.DrawAxis(vh, statusValMax);
    }

    /// <summary>
    /// 外周を描画
    /// </summary>
    private void DrawFrame(VertexHelper vh, float vol)
    {
        int currentVertCount = vh.currentVertCount;
        var v = UIVertex.simpleVert;
        v.color = color;

        //各頂点座標
        for (int i = 0; i < statusListCount; i++)
        {
            float deg = (360f / statusListCount) * 0.5f;
            float offset = (LineWidth / Mathf.Cos(deg * Mathf.Deg2Rad)) / 2f;

            float rad = (90f - (360f / (float)statusListCount) * i) * Mathf.Deg2Rad;

            float x1 = 0.5f + Mathf.Cos(rad) * ((radius / 100) * vol - offset);
            float y1 = 0.5f + Mathf.Sin(rad) * ((radius / 100) * vol - offset);
            float x2 = 0.5f + Mathf.Cos(rad) * ((radius / 100) * vol + offset);
            float y2 = 0.5f + Mathf.Sin(rad) * ((radius / 100) * vol + offset);

            Vector2 p1 = CreatePos(x1, y1);
            Vector2 p2 = CreatePos(x2, y2);

            v.position = p1;
            vh.AddVert(v);

            v.position = p2;
            vh.AddVert(v);

            vh.AddTriangle(
                (((i + 0) * 2) + 0) % (statusListCount * 2) + currentVertCount,
                (((i + 0) * 2) + 1) % (statusListCount * 2) + currentVertCount,
                (((i + 1) * 2) + 0) % (statusListCount * 2) + currentVertCount
            );

            vh.AddTriangle(
                (((i + 1) * 2) + 0) % (statusListCount * 2) + currentVertCount,
                (((i + 0) * 2) + 1) % (statusListCount * 2) + currentVertCount,
                (((i + 1) * 2) + 1) % (statusListCount * 2) + currentVertCount
            );
        }
    }

    /// <summary>
    /// 軸を描画
    /// </summary>
    private void DrawAxis(VertexHelper vh, float vol)
    {
        int currentVertCount = vh.currentVertCount;

        var v = UIVertex.simpleVert;
        v.color = color;
        for (int i = 0; i < statusListCount; i++)
        {
            float halfWidthDeg = 90 * LineWidth / (Mathf.PI * (radius / 100) * vol);

            float rad1 = (90f - halfWidthDeg - (360f / (float)statusListCount) * i) * Mathf.Deg2Rad;
            float rad2 = (90f + halfWidthDeg - (360f / (float)statusListCount) * i) * Mathf.Deg2Rad;

            float x3 = 0.5f + Mathf.Cos(rad1) * (radius / 100) * vol;
            float y3 = 0.5f + Mathf.Sin(rad1) * (radius / 100) * vol;
            float x4 = 0.5f + Mathf.Cos(rad2) * (radius / 100) * vol;
            float y4 = 0.5f + Mathf.Sin(rad2) * (radius / 100) * vol;
            float x1 = 0.5f + (x3 - x4) / 2f;
            float y1 = 0.5f + (y3 - y4) / 2f;
            float x2 = 0.5f + (x4 - x3) / 2f;
            float y2 = 0.5f + (y4 - y3) / 2f;

            Vector2 p1 = CreatePos(x1, y1);
            Vector2 p2 = CreatePos(x2, y2);
            Vector2 p3 = CreatePos(x3, y3);
            Vector2 p4 = CreatePos(x4, y4);

            v.position = p1;
            vh.AddVert(v);

            v.position = p2;
            vh.AddVert(v);

            v.position = p3;
            vh.AddVert(v);

            v.position = p4;
            vh.AddVert(v);

            vh.AddTriangle(
                ((i * 4) + 0) + currentVertCount,
                ((i * 4) + 3) + currentVertCount,
                ((i * 4) + 2) + currentVertCount
            );

            vh.AddTriangle(
                ((i * 4) + 0) + currentVertCount,
                ((i * 4) + 1) + currentVertCount,
                ((i * 4) + 3) + currentVertCount
            );
        }
    }

    /// <summary>
    /// uGUI座標を作成
    /// </summary>
    private Vector2 CreatePos(float x, float y)
    {
        Vector2 p = Vector2.zero;
        p.x -= rectTransform.pivot.x;
        p.y -= rectTransform.pivot.y;
        p.x += x;
        p.y += y;
        Debug.Log(rectTransform.rect.width);
        p.x *= rectTransform.rect.width;
        p.y *= rectTransform.rect.height;

        return p;
    }
}