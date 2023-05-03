using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneratorBehaviour))]
public class GeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            ((GeneratorBehaviour) target).Generate();
            ((GeneratorBehaviour) target).PrintGrid();
        }

        DrawDefaultInspector();
    }
}
