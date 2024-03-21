using UnityEditor;
using UnityEngine;

// Ensure the custom editor is targeting MMDBlendshapesCreator components
[CustomEditor(typeof(MMDBlendshapesCreator))]
public class MMDBlendshapesCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Add any custom GUI code here
        EditorGUILayout.HelpBox("This script will create MMD Blendshapes using VRChat visemes and other blendshapes present on your face mesh at build time.", MessageType.Info);

        // Check for the SkinnedMeshRenderer with blendshapes
        MMDBlendshapesCreator creator = (MMDBlendshapesCreator)target;
        if (creator.gameObject.transform.Find("Body") != null)
        {
            SkinnedMeshRenderer bodyRenderer = creator.gameObject.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
            if (bodyRenderer != null)
            {
                if (bodyRenderer.sharedMesh != null && bodyRenderer.sharedMesh.blendShapeCount > 0)
                {
                    // All Good
                    EditorGUILayout.HelpBox("Face mesh found and contains blendshapes.", MessageType.Info);
                }
                else
                {
                    // If the "Body" SkinnedMeshRenderer does not have blendshapes, show an error message
                    EditorGUILayout.HelpBox("Your face mesh does not contain any blendshapes. This script cannot work without them.", MessageType.Error);
                }
            }
            else
            {
                // If the "Body" object does not have a SkinnedMeshRenderer component
                EditorGUILayout.HelpBox("You face object must be called 'Body' for this script to work properly. Please modify your avatar face mesh's name first.", MessageType.Error);
            }
        }
        else
        {
            // If there is no "Body" object in the GameObject
            EditorGUILayout.HelpBox("You face object must be called 'Body' for this script to work properly. Please modify your avatar face mesh's name first.", MessageType.Error);
        }
    }
}
