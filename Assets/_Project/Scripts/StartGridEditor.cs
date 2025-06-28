using UnityEngine;
using UnityEditor;
using _Project.Scripts;

[CustomEditor(typeof(StartGrid))]
public class StartGridEditor : Editor
{
    private int _newGridSize;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var startGrid = (StartGrid)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Runtime Grid Resizing", EditorStyles.boldLabel);

        if (Application.isPlaying)
        {
            _newGridSize = EditorGUILayout.IntField("New Grid Size", _newGridSize);

            if (GUILayout.Button("Resize Grid"))
            {
                startGrid.ResizeGridFromEditor(_newGridSize);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("You must be in Play Mode to resize the grid at runtime.", MessageType.Info);
        }
    }
}