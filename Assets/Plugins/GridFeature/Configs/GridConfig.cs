using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Config/Grid", order = 0)]
public sealed class GridConfig : ScriptableObject
{
    [Header("Grid Dimensions")]
    [Tooltip("World origin position")]
    [SerializeField] private Vector3 _position;

    [Tooltip("Rotation of the grid in degrees")]
    [SerializeField] private Vector3 _rotation;

    [Header("Cell Properties")]
    [Tooltip("Size of each individual cell in world units")]
    [Min(0)]
    [SerializeField] private float _cellSize = 1f;

    [Tooltip("Spacing between cells in world units (X for horizontal, Y for vertical spacing)")]
    [SerializeField] private Vector2 _cellOffset;

    [Header("Visual Settings")]
    [Tooltip("Prefab to use for grid node visualization")]
    [SerializeField] private GameObject _nodePrefab;

    [Header("Grid Cells")]
    [Tooltip("List of cell coordinates in grid space")]
    [SerializeField] private List<Vector2Int> _cells = new();

    public Vector3 Position => _position;
    public Vector3 Rotation => _rotation;
    public float CellSize => _cellSize;
    public Vector2 CellOffset => _cellOffset;
    public GameObject NodePrefab => _nodePrefab;
    public IReadOnlyList<Vector2Int> Cells => _cells.AsReadOnly();
    public IReadOnlyList<Vector3> CellsWorld => GetCellsWorldPositions();
    public Quaternion RotationQuaternion => Quaternion.Euler(_rotation);

    public List<Vector3> GetCellsWorldPositions()
    {
        var positionOrigin = Position;
        var totalCellSize = new Vector2(CellSize, CellSize) + CellOffset;

        var worldPositions = Cells.Select(index =>
        {
            var vector3 = RotationQuaternion * new Vector3(index.x * totalCellSize.x, index.y * totalCellSize.y, 0) + positionOrigin;
            return vector3 + RotationQuaternion * new Vector3(CellSize, CellSize, 0) * 0.5f;
        }).ToList();

        return worldPositions;
    }

    public void AddCell(Vector2Int cell)
    {
        if (!_cells.Contains(cell))
        {
            _cells.Add(cell);
        }
    }

    public bool RemoveCell(Vector2Int cell)
    {
        return _cells.Remove(cell);
    }

    public void ClearCells()
    {
        _cells.Clear();
    }

    public bool ContainsCell(Vector2Int cell)
    {
        return _cells.Contains(cell);
    }

    private void OnValidate()
    {
        RemoveDuplicateCells();
    }

    public void ClearAndFillRectangle(int width, int height, Vector2Int offset)
    {
        _cells.Clear();
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _cells.Add(new Vector2Int(x + offset.x, y + offset.y));
            }
        }
    }

    private void RemoveDuplicateCells()
    {
        if (_cells == null || _cells.Count == 0)
            return;

        var originalCount = _cells.Count;
        var uniqueCells = new HashSet<Vector2Int>(_cells);

        if (uniqueCells.Count != originalCount)
        {
            _cells = uniqueCells.ToList();
            Debug.LogWarning($"Removed {originalCount - uniqueCells.Count} duplicate cells from {name}");
        }
    }
}
