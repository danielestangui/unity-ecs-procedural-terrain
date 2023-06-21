using UnityEditor;
using UnityEngine;
using Unity.Entities;

[InitializeOnLoad]
public class EditorTimeFunction : Editor
{

    static EditorTimeFunction()
    {
        EditorApplication.update += UpdateFunction;
    }

    private static void UpdateFunction()
    {
        Camera cam = Camera.current;
        if (cam != null ) 
        {
         
        } 
    }
}


