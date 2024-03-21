using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlendshapesRemover))]
public class BlendshapesRemoverEditor : Editor
{
    SerializedProperty bodyRendererProperty;
    SerializedProperty blendshapeTogglesProperty;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        // Cache the SerializedProperties.
        bodyRendererProperty = serializedObject.FindProperty("bodyRenderer");
        blendshapeTogglesProperty = serializedObject.FindProperty("blendshapeToggles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the actual values

        // Manually draw the Body Renderer property instead of using DrawDefaultInspector()
        EditorGUILayout.PropertyField(bodyRendererProperty);

        EditorGUILayout.HelpBox("This script will remove selected blendshapes at build time.", MessageType.Info);

        if (bodyRendererProperty.objectReferenceValue != null)
        {
            SkinnedMeshRenderer bodyRenderer = bodyRendererProperty.objectReferenceValue as SkinnedMeshRenderer;
            if (bodyRenderer.sharedMesh != null)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                for (int i = 0; i < blendshapeTogglesProperty.arraySize; i++)
                {
                    SerializedProperty toggleProp = blendshapeTogglesProperty.GetArrayElementAtIndex(i);
                    bool value = toggleProp.boolValue;
                    value = GUILayout.Toggle(value, bodyRenderer.sharedMesh.GetBlendShapeName(i));
                    toggleProp.boolValue = value; // Set the changed value back to property
                }
                GUILayout.EndScrollView();
            }
        }

        serializedObject.ApplyModifiedProperties(); // Save the changes back to the target
    }
}