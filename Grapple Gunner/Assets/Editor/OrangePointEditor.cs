using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(OrangePoint))]
public class OrangePointEditor : Editor
{
    SerializedProperty disabledMaterial;
    SerializedProperty infiniteUses;
    SerializedProperty numberUses;
    void OnEnable()
    {
        disabledMaterial = serializedObject.FindProperty("disabledMaterial");
        numberUses = serializedObject.FindProperty("numberUses");
        infiniteUses = serializedObject.FindProperty("infiniteUses");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(disabledMaterial);
        EditorGUILayout.PropertyField(infiniteUses);
        if (!infiniteUses.boolValue)
        {
            EditorGUILayout.PropertyField(numberUses);
        }

        serializedObject.ApplyModifiedProperties();
    }
}