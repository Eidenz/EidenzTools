using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(MMDBlendshapeCreatorApplier))]

public class MMDBlendshapeCreatorApplier : Plugin<MMDBlendshapeCreatorApplier>
{
    /// <summary>
    /// This name is used to identify the plugin internally, and can be used to declare BeforePlugin/AfterPlugin
    /// dependencies. If not set, the full type name will be used.
    /// </summary>
    public override string QualifiedName => "com.eidenz.mmdblendshapecreator.editor";

    /// <summary>
    /// The plugin name shown in debug UIs. If not set, the qualified name will be shown.
    /// </summary>
    public override string DisplayName => "MMD Blendshapes Creator";

    protected override void Configure()
    {
        InPhase(BuildPhase.Generating).Run("Generate MMD blendshapes", ctx =>
        {
            MMDBlendshapesCreator apply = ctx.AvatarRootObject.GetComponent<MMDBlendshapesCreator>();
            if (apply != null)
            {
                DuplicateAndModify(ctx.AvatarRootObject);
            }
        });
    }

    private Dictionary<string, string> blendshapeMapping = new Dictionary<string, string>
    {
        {"A", "あ"}, {"I", "い"}, {"U", "う"}, {"E", "え"}, {"O", "お"},
        {"vrc.aa", "あ"}, {"vrc.ih", "い"}, {"vrc.uh", "う"}, {"vrc.e", "え"}, {"vrc.oh", "お"},
        {"Sad", "悲しい"}, {"Sadness", "悲しい"}, {"Angry", "怒り"}, {"Anger", "怒り"},
        {"Happy", "喜び"}, {"Cheerful", "喜び"},
        {"Wink", "ウィンク"}, {"Wink Right", "右ウィンク"}, {"Wink Left", "左ウィンク"}
    };

    public void DuplicateAndModify(GameObject avi)
    {
        SkinnedMeshRenderer[] renderers = avi.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in renderers)
        {
            Mesh originalMesh = renderer.sharedMesh;
            Mesh modifiedMesh = new Mesh();
            CopyMesh(originalMesh, modifiedMesh);
            ProcessBlendShapes(originalMesh, modifiedMesh);
            renderer.sharedMesh = modifiedMesh;
        }
    }

    private void CopyMesh(Mesh originalMesh, Mesh newMesh)
    {
        newMesh.vertices = originalMesh.vertices;
        newMesh.normals = originalMesh.normals;
        newMesh.tangents = originalMesh.tangents;
        newMesh.uv = originalMesh.uv;
        newMesh.uv2 = originalMesh.uv2;
        newMesh.uv3 = originalMesh.uv3;
        newMesh.uv4 = originalMesh.uv4;
        newMesh.colors = originalMesh.colors;
        newMesh.triangles = originalMesh.triangles;
        newMesh.bindposes = originalMesh.bindposes;
        newMesh.boneWeights = originalMesh.boneWeights;
        newMesh.subMeshCount = originalMesh.subMeshCount;
        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            newMesh.SetTriangles(originalMesh.GetTriangles(i), i);
        }
    }

    void ProcessBlendShapes(Mesh originalMesh, Mesh newMesh)
    {
        // First, copy all existing blendshapes from the original to the new mesh
        CopyExistingBlendShapes(originalMesh, newMesh);

        // Then, add or overwrite with the new, renamed blendshapes
        foreach (var kvp in blendshapeMapping)
        {
            string targetName = NormalizeBlendShapeName(kvp.Key);
            bool found = false;
            
            for (int i = 0; i < originalMesh.blendShapeCount && !found; i++)
            {
                string originalName = NormalizeBlendShapeName(originalMesh.GetBlendShapeName(i));
                if (targetName.Equals(originalName))
                {
                    // Rename and add blendshape if it matches the mapping
                    CopyAndRenameBlendShape(originalMesh, newMesh, i, kvp.Value);
                    found = true;
                }
            }

            if (!found)
            {
                Debug.LogWarning($"No blendshape matching '{kvp.Key}' found to rename to '{kvp.Value}'.");
            }
        }
    }

    void CopyExistingBlendShapes(Mesh fromMesh, Mesh toMesh)
    {
        for (int i = 0; i < fromMesh.blendShapeCount; i++)
        {
            string blendShapeName = fromMesh.GetBlendShapeName(i);
            CopyAndRenameBlendShape(fromMesh, toMesh, i, blendShapeName);
        }
    }

    void CopyAndRenameBlendShape(Mesh fromMesh, Mesh toMesh, int blendShapeIndex, string newName)
    {
        if (toMesh.GetBlendShapeIndex(newName) != -1)
        {
            // If the blendshape already exists in the new mesh, don't attempt to add it again
            Debug.LogWarning($"Blendshape '{newName}' already exists in the mesh. Existing blendshapes are being copied.");
            return;
        }

        Vector3[] vertices = new Vector3[fromMesh.vertexCount];
        Vector3[] normals = new Vector3[fromMesh.vertexCount];
        Vector3[] tangents = new Vector3[fromMesh.vertexCount];

        for (int frameIndex = 0; frameIndex < fromMesh.GetBlendShapeFrameCount(blendShapeIndex); frameIndex++)
        {
            float frameWeight = fromMesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);
            fromMesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, vertices, normals, tangents);
            toMesh.AddBlendShapeFrame(newName, frameWeight, vertices, normals, tangents);
        }
    }

    string NormalizeBlendShapeName(string name)
    {
        var parts = System.Text.RegularExpressions.Regex.Split(name, @"[^a-zA-Z0-9]");
        string lastNamePart = parts.LastOrDefault();
        return lastNamePart?.ToLower() ?? "";
    }
}