using UnityEngine;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(BlendshapeRemoverApplier))]

public class BlendshapeRemoverApplier : Plugin<BlendshapeRemoverApplier>
{
    /// <summary>
    /// This name is used to identify the plugin internally, and can be used to declare BeforePlugin/AfterPlugin
    /// dependencies. If not set, the full type name will be used.
    /// </summary>
    public override string QualifiedName => "com.eidenz.blendshapesremover.editor";

    /// <summary>
    /// The plugin name shown in debug UIs. If not set, the qualified name will be shown.
    /// </summary>
    public override string DisplayName => "Blendshapes Remover";

    protected override void Configure()
    {
        InPhase(BuildPhase.Generating).Run("Remove selected blendshapes", ctx =>
        {
            BlendshapesRemover applier = ctx.AvatarRootObject.GetComponent<BlendshapesRemover>();
            if (applier != null)
            {
                // Retrieve blendshapeToggles and skinnedmeshrenderer from the applier
                bool[] blendshapeToggles = applier.GetBlendshapeToggles();
                SkinnedMeshRenderer bodyRenderer = applier.bodyRenderer;

                DuplicateAndRemoveBlendshapes(blendshapeToggles, bodyRenderer, ctx.AvatarRootObject);
            }
        });
    }

    // Call this method to duplicate and remove blendshapes based on the toggles
    public void DuplicateAndRemoveBlendshapes(bool[] blendshapeToggles, SkinnedMeshRenderer bodyRenderer, GameObject avi)
    {
        if (avi == null || blendshapeToggles == null)
            return;

        SkinnedMeshRenderer duplicatedRenderer = avi.transform.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
        if (duplicatedRenderer != null)
        {
            Mesh originalMesh = bodyRenderer.sharedMesh;
            Mesh newMesh = CopyAndModifyMesh(originalMesh, blendshapeToggles);

            duplicatedRenderer.sharedMesh = newMesh;
        }
    }

    // Copy the original mesh and modify it based on the blendshape toggles
    private Mesh CopyAndModifyMesh(Mesh originalMesh, bool[] blendshapeToggles)
    {
        Mesh newMesh = new Mesh();
        
        // Copy mesh data
        newMesh.vertices = originalMesh.vertices;
        newMesh.normals = originalMesh.normals;
        newMesh.tangents = originalMesh.tangents;
        newMesh.uv = originalMesh.uv;
        newMesh.uv2 = originalMesh.uv2;
        newMesh.uv3 = originalMesh.uv3;
        newMesh.uv4 = originalMesh.uv4;
        newMesh.colors = originalMesh.colors;
        newMesh.triangles = originalMesh.triangles;

        // It's crucial to copy bone weights and bind poses for SkinnedMeshRenderer to work correctly
        newMesh.boneWeights = originalMesh.boneWeights;
        newMesh.bindposes = originalMesh.bindposes;

        // Copy submeshes
        newMesh.subMeshCount = originalMesh.subMeshCount;
        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            newMesh.SetTriangles(originalMesh.GetTriangles(i), i);
        }

        // Copy blend shapes
        for (int i = 0; i < originalMesh.blendShapeCount; i++)
        {
            // Only copy the blend shape if it's not toggled off
            if (!blendshapeToggles[i])
            {
                CopyBlendShape(originalMesh, newMesh, i);
            }
        }

        // Recalculate bounds and normals if necessary
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        
        return newMesh;
    }

     private void CopyBlendShape(Mesh fromMesh, Mesh toMesh, int blendShapeIndex)
    {
        Vector3[] deltaVertices = new Vector3[fromMesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[fromMesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[fromMesh.vertexCount];

        string blendShapeName = fromMesh.GetBlendShapeName(blendShapeIndex);
        for (int frameIndex = 0; frameIndex < fromMesh.GetBlendShapeFrameCount(blendShapeIndex); frameIndex++)
        {
            float frameWeight = fromMesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);
            fromMesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
            toMesh.AddBlendShapeFrame(blendShapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
        }
    }
}