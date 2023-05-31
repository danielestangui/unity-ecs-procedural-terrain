using UnityEditor;
using UnityEngine;
using Unity.Entities;


public class MyEditorWindow : EditorWindow
{
    [MenuItem("Procedural Terrain Generator/Generate Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyEditorWindow), false, "Generate Window");
    }

    private void OnGUI()
    {
        // Implement the GUI for your editor window
        GUILayout.Label("Generate Window", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Grid"))
        {
            // Perform some action
        }
    }
}
