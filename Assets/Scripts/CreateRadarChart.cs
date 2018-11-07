using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CreateRadarChart : MonoBehaviour
{
    //頂点数
    private int VerticesCount = 5;

    //半径
    private float Radius = 5f;

    private void Start()
    {
        if (VerticesCount < 3)
            return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //原点座標
        vertices.Add(Vector3.zero);

        //各頂点座標
        float rad, x, y;
        for (int i = 1; i <= this.VerticesCount; i++)
        {
            rad = (90f - (360f / (float)this.VerticesCount) * (i - 1)) * Mathf.Deg2Rad;
            x = Mathf.Cos(rad);
            y = Mathf.Sin(rad);
            vertices.Add(new Vector3(x * i, y * i, 0));
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i == this.VerticesCount ? 1 : i + 1);
        }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
    }
}