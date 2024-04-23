/*Prompt:

@"Create the Second Oak Tree:
1. In the Project window, navigate to Assets/SimpleNaturePack/Prefabs/.
2. Drag and drop another Tree_oak.prefab into the Hierarchy window.
3. With the second Tree_oak object selected in the Hierarchy, set the Transform Position to (2, 0.89, 0) in the Inspector window.
4. Set the Scale to (1, 1, 1) to keep the tree at its original size.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript3 : EditorWindow
{
    [MenuItem("Edit/Story/Step3")]
    public static void RunTask()
    {
        string prefabPath = "Assets/SimpleNaturePack/Prefabs/Tree_oak.prefab";
        GameObject oakTreePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        GameObject oakTree = Instantiate(oakTreePrefab, new Vector3(2f, 0.89f, 0f), Quaternion.identity);
        oakTree.transform.localScale = Vector3.one;
    }
}