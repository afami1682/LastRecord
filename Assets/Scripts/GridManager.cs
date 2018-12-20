using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 座標0地点から、右下に向けてグリッドを表示する（2D専用）
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridManager : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh = ReGrid(mesh);

        // 最初は非表示にする
        changeActive();
    }

    Mesh ReGrid(Mesh mesh)
    {
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
        mesh.Clear();

        int sizeX = GameManager.GetMap().field.width;
        int sizeY = GameManager.GetMap().field.height;

        float gridSize = 1f;
        int drawGridX = sizeX * 2 + 2;// 描画するグリッドの数（横）
        int drawGridY = sizeY * 2 + 2;// 描画するグリッドの数（縦）
        float width = sizeX * gridSize; // 全体の長さ(X)
        float height = sizeY * gridSize; // 全体の長さ(Y)

        float gridPos = gridSize / 2; // グリッドの座標計算用変数
        int resolution = drawGridX + drawGridY;

        Vector3[] vertices = new Vector3[resolution];
        Vector2[] uvs = new Vector2[resolution];
        int[] lines = new int[resolution];
        Color[] colors = new Color[resolution];

        // グリッドの縦棒の座標設定
        for (int i = 0; i < drawGridX; i += 2)
        {
            vertices[i] = new Vector3(gridPos * i - 0.5f, 0.5f, 0);
            vertices[i + 1] = new Vector3(gridPos * i - 0.5f, -height + 0.5f, 0);
        }

        // グリッドの横棒の座標設定
        for (int i = 0; i < drawGridY; i += 2)
        {
            vertices[drawGridX + i] = new Vector3(-0.5f, -gridPos * i + 0.5f, 0);
            vertices[drawGridX + i + 1] = new Vector3(width - 0.5f, -gridPos * i + 0.5f, 0);
        }

        for (int i = 0; i < resolution; i++)
        {
            uvs[i] = Vector2.zero;
            lines[i] = i;
            colors[i] = Color.cyan;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        return mesh;
    }

    /// <summary>
    /// アクティブ状態を反転する
    /// </summary>
    public void changeActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}