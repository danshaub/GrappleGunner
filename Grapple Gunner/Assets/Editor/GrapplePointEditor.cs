using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrapplePoint))]
[CanEditMultipleObjects]
public class GrapplePointEditor : Editor
{
    SerializedProperty type;
    SerializedProperty useRaycastPosition;
    SerializedProperty grapplePosition;
    SerializedProperty grappleRotation;
    SerializedProperty teleportParent;
    SerializedProperty teleportOffset;
    SerializedProperty disabledMaterial;
    SerializedProperty numberUses;
    SerializedProperty grappleMesh;
    SerializedProperty onButtonPress;


    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        useRaycastPosition = serializedObject.FindProperty("useRaycastPosition");
        grapplePosition = serializedObject.FindProperty("grapplePosition");
        grappleRotation = serializedObject.FindProperty("grappleRotation");
        teleportParent = serializedObject.FindProperty("teleportParent");
        teleportOffset = serializedObject.FindProperty("teleportOffset");
        disabledMaterial = serializedObject.FindProperty("disabledMaterial");
        numberUses = serializedObject.FindProperty("numberUses");
        grappleMesh = serializedObject.FindProperty("grappleMesh");
        onButtonPress = serializedObject.FindProperty("onButtonPress");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(useRaycastPosition);

        if(!useRaycastPosition.boolValue){
            EditorGUILayout.PropertyField(grapplePosition);
            EditorGUILayout.PropertyField(grappleRotation);
            EditorGUILayout.PropertyField(grappleMesh);
        }

        switch (type.enumValueIndex){
            case 3:
                EditorGUILayout.PropertyField(teleportParent);
                EditorGUILayout.PropertyField(teleportOffset);
                EditorGUILayout.PropertyField(disabledMaterial);
                EditorGUILayout.PropertyField(numberUses);
                break;
            case 5:
                EditorGUILayout.PropertyField(onButtonPress);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}