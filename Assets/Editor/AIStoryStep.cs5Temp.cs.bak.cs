/*Prompt:

@"Create the XR Interaction Setup:
1. In the Project window, navigate to Assets/SimpleNaturePack/Prefabs/.
2. Drag and drop the XR_Interaction_Setup.prefab into the Hierarchy window.
3. With the XR_Interaction_Setup object selected in the Hierarchy, set the Transform Position to (0, 0, 0) in the Inspector window.
4. Set the Scale to (1, 1, 1) to keep the XR Interaction Setup at its original size.
*/

using UnityEditor;
using UnityEngine;

public class UnityEditorScript5 : EditorWindow
{
    [MenuItem("Edit/Story/Step5")]
    public static void RunTask()
    {
        string prefabPath = "Assets/SimpleNaturePack/Prefabs/XR_Interaction_Setup.prefab";
        GameObject xrInteractionSetupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (xrInteractionSetupPrefab != null)
        {
            GameObject xrInteractionSetup = Instantiate(xrInteractionSetupPrefab);
            xrInteractionSetup.transform.position = Vector3.zero;
            xrInteractionSetup.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("XR Interaction Setup prefab not found at path: " + prefabPath);
        }
    }
}