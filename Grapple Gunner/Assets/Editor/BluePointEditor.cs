using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BluePoint))]
public class BluePointEditor : Editor
{
    SerializedProperty worldRespawnPosition;
    SerializedProperty pointVisual;
    SerializedProperty canStore;
    void OnEnable()
    {
        worldRespawnPosition = serializedObject.FindProperty("worldRespawnPosition");
        pointVisual = serializedObject.FindProperty("pointVisual");
        canStore = serializedObject.FindProperty("canStore");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(worldRespawnPosition);
        EditorGUILayout.PropertyField(pointVisual);
        EditorGUILayout.PropertyField(canStore);

        serializedObject.ApplyModifiedProperties();
    }
}