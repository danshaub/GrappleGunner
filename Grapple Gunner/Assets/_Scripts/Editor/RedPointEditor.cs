using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(RedPoint))]
public class RedPointEditor : Editor
{
    SerializedProperty useRaycastPosition;
    SerializedProperty grapplePosition;
    SerializedProperty grappleRotation;
    void OnEnable()
    {
        useRaycastPosition = serializedObject.FindProperty("useRaycastPosition");
        grapplePosition = serializedObject.FindProperty("grapplePosition");
        grappleRotation = serializedObject.FindProperty("grappleRotation");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(useRaycastPosition);
        if (!useRaycastPosition.boolValue)
        {
            EditorGUILayout.PropertyField(grapplePosition);
            EditorGUILayout.PropertyField(grappleRotation);
        }

        serializedObject.ApplyModifiedProperties();
    }
}