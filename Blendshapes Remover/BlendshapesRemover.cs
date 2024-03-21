using UnityEngine;
using VRC.SDKBase;

public class BlendshapesRemover : MonoBehaviour, IEditorOnly
{
    public SkinnedMeshRenderer bodyRenderer;

    [SerializeField]
    private bool[] blendshapeToggles;

    void Reset()
    {
        InitializeBlendShapeToggles();
    }

    void OnValidate()
    {
        InitializeBlendShapeToggles();
    }

    private void InitializeBlendShapeToggles()
    {
        if (bodyRenderer != null && bodyRenderer.sharedMesh != null)
        {
            int blendShapeCount = bodyRenderer.sharedMesh.blendShapeCount;
            if (blendshapeToggles == null || blendshapeToggles.Length != blendShapeCount)
            {
                blendshapeToggles = new bool[blendShapeCount];
            }
        }
    }

    public void SetBlendshapeToggles(bool[] toggles)
    {
        blendshapeToggles = toggles;
    }

    public bool[] GetBlendshapeToggles()
    {
        // Initialize or resize the toggles array as needed
        if (bodyRenderer != null && (blendshapeToggles == null || blendshapeToggles.Length != bodyRenderer.sharedMesh.blendShapeCount))
        {
            blendshapeToggles = new bool[bodyRenderer.sharedMesh.blendShapeCount];
        }
        return blendshapeToggles;
    }
}