/*Prompt:

@"Create the First Oak Tree:
1. In the Project window, navigate to Assets/SimpleNaturePack/Prefabs/.
2. Drag and drop the Tree_oak.prefab into the Hierarchy window.
3. With the Tree_oak object selected in the Hierarchy, set the Transform Position to (-2, 0.89, 0) in the Inspector window.
4. Set the Scale to (1, 1, 1) to keep the tree at its original size.

*/

using UnityEditor;
using UnityEngine;

public class UnityEditorScript2 : EditorWindow
{
    [MenuItem("Edit/Story/Step2")]
    public static void RunTask()
    {
        string prefabPath = "Assets/SimpleNaturePack/Prefabs/Tree_oak.prefab";
        GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (treePrefab != null)
        {
            GameObject oakTree = Instantiate(treePrefab, new Vector3(-2f, 0.89f, 0f), Quaternion.identity);
            oakTree.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("Tree_oak prefab not found at path: " + prefabPath);
        }
    }
}