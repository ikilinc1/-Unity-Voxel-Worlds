using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TreeCreator))]
public class TreeDesigner : Editor
{
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreeCreator handle = (TreeCreator)target;

        if (GUILayout.Button("Realign Blocks"))
        {
            handle.ReAlignBlocks();
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
        EditorGUILayout.TextArea(handle.blockDetails, GUILayout.Height(800));
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Update Details"))
        {
            handle.GetDetails();
        }


    }

}
