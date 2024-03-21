using UnityEngine;
using UnityEditor;

public class EidenzToolsComponentAdder
{
    [MenuItem("GameObject/EidenzTools/MMDBlendshapesCreator", false, 10)]
    static void AddMMDBlendshapesCreatorToGameObject(MenuCommand menuCommand)
    {
        GameObject go = Selection.activeGameObject;

        if (go != null)
        {
            go.AddComponent<MMDBlendshapesCreator>();
        }
        else
        {
            Debug.LogWarning("You must select a game object to add the EidenzTools | MMDBlendshapesCreator component.");
        }
    }

    // Validate the menu item defined by the function above
    // The menu item will be disabled if no game object is selected
    [MenuItem("GameObject/EidenzTools/MMDBlendshapesCreator", true)]
    static bool ValidateAddMMDBlendshapesCreatorToGameObject(MenuCommand menuCommand)
    {
        return Selection.activeGameObject != null;
    }

    [MenuItem("GameObject/EidenzTools/BlendshapesRemover", false, 10)]
    static void AddBlendshapesRemoverToGameObject(MenuCommand menuCommand)
    {
        GameObject go = Selection.activeGameObject;

        if (go != null)
        {
            go.AddComponent<BlendshapesRemover>();
        }
        else
        {
            Debug.LogWarning("You must select a game object to add the EidenzTools | BlendshapesRemover component.");
        }
    }

    // Validate the menu item defined by the function above
    // The menu item will be disabled if no game object is selected
    [MenuItem("GameObject/EidenzTools/BlendshapesRemover", true)]
    static bool ValidateAddBlendshapesRemoverToGameObject(MenuCommand menuCommand)
    {
        return Selection.activeGameObject != null;
    }
}