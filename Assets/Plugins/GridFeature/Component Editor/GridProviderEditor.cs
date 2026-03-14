#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(GridProvider))]
public class GridProviderEditor : Editor
{
    private GridProvider _gridProvider;
    private GridVisualizerSetting _gridVisualizerSetting;
    private GridEditorSetting _gridEditorSetting;

    private GridConfig GridConfig => _gridProvider.GridConfig;

    private SerializedObject _gridConfigSerializedObject;
    private SerializedProperty _cellsProperty;

    private bool _showGridCells = false;
    private bool _showGenerator = true;

    // Параметры для генератора прямоугольника
    private Vector2Int _rectSize = new Vector2Int(5, 5);
    private Vector2Int _rectOffset = Vector2Int.zero;

    private void OnEnable()
    {
        _gridProvider = (GridProvider)target;
        _gridVisualizerSetting = _gridProvider.GridVisualizerSetting;
        _gridEditorSetting = _gridProvider.GridEditorSetting;

        UpdateGridConfigReference();
    }

    private void UpdateGridConfigReference()
    {
        if (GridConfig != null)
        {
            _gridConfigSerializedObject = new SerializedObject(GridConfig);
            _cellsProperty = _gridConfigSerializedObject.FindProperty("_cells");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        UpdateGridConfigReference();

        if (GridConfig == null)
        {
            EditorGUILayout.HelpBox("Assign a GridConfig to visualize the grid.", MessageType.Warning);
            DrawCreateGridConfigButton();
            return;
        }

        if (_gridConfigSerializedObject != null)
        {
            DrawRectGenerator(); // <-- Новый блок генерации
            DrawGridConfiguration();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRectGenerator()
    {
        EditorGUILayout.Space();
        _showGenerator = EditorGUILayout.BeginFoldoutHeaderGroup(_showGenerator, "Rectangle Generator");

        if (_showGenerator)
        {
            EditorGUI.indentLevel++;
            _rectSize = EditorGUILayout.Vector2IntField("Size (Width/Height)", _rectSize);
            _rectOffset = EditorGUILayout.Vector2IntField("Start Offset", _rectOffset);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate (Replace)"))
            {
                if (EditorUtility.DisplayDialog("Confirm", "This will clear all existing cells. Continue?", "Yes", "No"))
                {
                    Undo.RecordObject(GridConfig, "Generate Rect Grid");
                    GridConfig.ClearAndFillRectangle(_rectSize.x, _rectSize.y, _rectOffset);
                    EditorUtility.SetDirty(GridConfig);
                    _gridEditorSetting.RefreshGrid();
                }
            }

            if (GUILayout.Button("Add Rectangle"))
            {
                Undo.RecordObject(GridConfig, "Add Rect to Grid");
                for (int x = 0; x < _rectSize.x; x++)
                {
                    for (int y = 0; y < _rectSize.y; y++)
                    {
                        GridConfig.AddCell(new Vector2Int(x + _rectOffset.x, y + _rectOffset.y));
                    }
                }
                EditorUtility.SetDirty(GridConfig);
                _gridEditorSetting.RefreshGrid();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    // ... (Остальные методы: DrawCreateGridConfigButton, DrawGridConfiguration, DrawGridCellsSection и т.д. остаются без изменений)

    private void DrawCreateGridConfigButton()
    {
        if (GUILayout.Button("Create New Grid Config"))
        {
            CreateNewGridConfig();
        }
    }

    private void DrawGridConfiguration()
    {
        _gridConfigSerializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Data", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_gridConfigSerializedObject.FindProperty("_position"));
        EditorGUILayout.PropertyField(_gridConfigSerializedObject.FindProperty("_rotation"));
        EditorGUILayout.PropertyField(_gridConfigSerializedObject.FindProperty("_cellSize"));
        EditorGUILayout.PropertyField(_gridConfigSerializedObject.FindProperty("_cellOffset"));
        EditorGUILayout.PropertyField(_gridConfigSerializedObject.FindProperty("_nodePrefab"));

        DrawGridCellsSection();

        _gridConfigSerializedObject.ApplyModifiedProperties();
    }

    private void DrawGridCellsSection()
    {
        EditorGUILayout.Space();
        _showGridCells = EditorGUILayout.Foldout(_showGridCells, "Raw Cells List (" + (GridConfig.Cells?.Count ?? 0) + ")", true);

        if (_showGridCells && _cellsProperty != null)
        {
            EditorGUI.indentLevel++;
            if (GUILayout.Button("Clear All Cells", GUILayout.Width(120)))
            {
                _cellsProperty.ClearArray();
            }

            for (int i = 0; i < _cellsProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_cellsProperty.GetArrayElementAtIndex(i), GUIContent.none);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    _cellsProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }

    private void CreateNewGridConfig()
    {
        var newConfig = CreateInstance<GridConfig>();
        string path = EditorUtility.SaveFilePanelInProject("Save Grid Config", "NewGridConfig.asset", "asset", "Save Grid Config");

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newConfig, path);
            AssetDatabase.SaveAssets();

            serializedObject.Update();
            serializedObject.FindProperty("_gridConfig").objectReferenceValue = newConfig;
            serializedObject.ApplyModifiedProperties();

            UpdateGridConfigReference();
        }
    }

    // Scene GUI methods (OnSceneGUI, AddButtonCells, RemoveButtonCells, etc.) 
    // остаются такими же, как в вашем коде...
    private void OnSceneGUI()
    {
        if (_gridEditorSetting == null || GridConfig == null)
            return;

        if (_gridEditorSetting.DrawAddButtons)
            AddButtonCells();

        if (_gridEditorSetting.DrawRemoveButtons)
            RemoveButtonCells();
    }

    private void RemoveButtonCells()
    {
        Handles.color = _gridEditorSetting.RemoveButtonColor;
        foreach (var pos in GridConfig.Cells)
        {
            var worldPosition = _gridEditorSetting.GetWorldPosition(pos);
            float size = GridConfig.CellSize * 0.3f;
            if (Handles.Button(worldPosition, GridConfig.RotationQuaternion, size, size, Handles.RectangleHandleCap))
            {
                RemoveCellFromConfig(pos);
            }
        }
    }

    private void AddButtonCells()
    {
        Handles.color = _gridEditorSetting.AddButtonColor;
        foreach (var pos in _gridEditorSetting.FindAdjacentPositions())
        {
            var worldPosition = _gridEditorSetting.GetWorldPosition(pos);
            float size = GridConfig.CellSize * 0.3f;
            if (Handles.Button(worldPosition, GridConfig.RotationQuaternion, size, size, Handles.RectangleHandleCap))
            {
                AddCellToConfig(pos);
            }
        }
    }

    private void RemoveCellFromConfig(Vector2Int cell)
    {
        _gridConfigSerializedObject.Update();
        for (int i = 0; i < _cellsProperty.arraySize; i++)
        {
            if (_cellsProperty.GetArrayElementAtIndex(i).vector2IntValue == cell)
            {
                _cellsProperty.DeleteArrayElementAtIndex(i);
                break;
            }
        }
        _gridConfigSerializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(GridConfig);
        _gridEditorSetting.RefreshGrid();
    }

    private void AddCellToConfig(Vector2Int cell)
    {
        _gridConfigSerializedObject.Update();
        _cellsProperty.arraySize++;
        _cellsProperty.GetArrayElementAtIndex(_cellsProperty.arraySize - 1).vector2IntValue = cell;
        _gridConfigSerializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(GridConfig);
        _gridEditorSetting.RefreshGrid();
    }
}
#endif
