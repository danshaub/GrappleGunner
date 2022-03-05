using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonPoint))]
public class ButtonPointEditor : Editor
{
    SerializedProperty onButtonPress;
    SerializedProperty onButtonRelease;
    void OnEnable() {
        onButtonPress = serializedObject.FindProperty("onButtonPress");
        onButtonRelease = serializedObject.FindProperty("onButtonRelease");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(onButtonPress);
        EditorGUILayout.PropertyField(onButtonRelease);

        serializedObject.ApplyModifiedProperties();
    }
}