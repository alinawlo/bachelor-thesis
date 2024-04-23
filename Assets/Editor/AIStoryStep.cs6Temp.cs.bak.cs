/*Prompt:

@"Create the XR Interaction Setup:
1. Drag and drop the 'Assets/Prefabs/XR_Interaction_Setup.prefab' into the Hierarchy.
2. Set the Transform Position to (0, 0, 0) to place it at the center of the scene.
3. Set the Scale to (1, 1, 1) to keep its original size."
*/

using UnityEditor;
using UnityEngine;

public class UnityEditorScript6 : EditorWindow
{
    [MenuItem("Edit/Story/Step6")]
    public static void RunTask()
    {
        string prefabPath = "Assets/Prefabs/XR_Interaction_Setup.prefab";
        GameObject xrInteractionSetupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (xrInteractionSetupPrefab != null)
        {
            GameObject xrInteractionSetup = Instantiate(xrInteractionSetupPrefab);
            xrInteractionSetup.transform.position = Vector3.zero;
            xrInteractionSetup.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
        }
    }
}