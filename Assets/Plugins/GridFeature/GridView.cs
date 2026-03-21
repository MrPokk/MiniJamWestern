using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridView : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public void Instantiate(GridConfig gridConfig)
    {
        _meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        transform.position = gridConfig.Position;
        transform.rotation = gridConfig.RotationQuaternion;

        // Настраиваем материал
        Material mat = new Material(Shader.Find("Custom/GridCell"));

        // Передаем два разных цвета
        mat.SetColor("_FillColor", gridConfig.FillColor);
        mat.SetColor("_BorderColor", gridConfig.BorderColor);
        mat.SetFloat("_Thickness", gridConfig.NodeLineWidth);

        _meshRenderer.material = mat;

        BuildMesh(gridConfig);
    }

    private void BuildMesh(GridConfig gridConfig)
    {
        Mesh mesh = new Mesh { name = "GridMesh" };
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float cellSize = gridConfig.CellSize;
        Vector2 total = new Vector2(cellSize, cellSize) + gridConfig.CellOffset;

        int vCount = 0;
        foreach (var cell in gridConfig.Cells)
        {
            // Математика как в твоем GridVisualizerSetting (DrawCell)
            float x = cell.x * total.x;
            float y = cell.y * total.y;

            // 4 вершины ячейки в локальных координатах
            vertices.Add(new Vector3(x, y, 0));                        // Лево-Низ (Min)
            vertices.Add(new Vector3(x, y + cellSize, 0));             // Лево-Верх
            vertices.Add(new Vector3(x + cellSize, y + cellSize, 0));  // Право-Верх (Max)
            vertices.Add(new Vector3(x + cellSize, y, 0));             // Право-Низ

            // UV координаты для шейдера (чтобы он знал где края)
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

            int b = vCount * 4;
            // Создаем два треугольника для одного квадрата
            triangles.Add(b); triangles.Add(b + 1); triangles.Add(b + 2);
            triangles.Add(b); triangles.Add(b + 2); triangles.Add(b + 3);

            vCount++;
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        _meshFilter.mesh = mesh;
    }
}
