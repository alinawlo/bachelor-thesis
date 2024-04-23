/*Prompt:

@"Create the Oak Tree:
1. Drag and drop the 'Assets/Prefabs/Tree_oak.prefab' into the Hierarchy.
2. Set the Transform Position to (0, 0.89, 0) to place the tree at the center of the rock circle.
3. Set the Scale to (1, 1, 1) to keep the tree's original size.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript4 : EditorWindow
{
    [MenuItem("Edit/Story/Step4")]
    public static void RunTask()
    {
        GameObject oakTreePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tree_oak.prefab");
        GameObject oakTree = Instantiate(oakTreePrefab, Vector3.zero, Quaternion.identity);
        oakTree.transform.position = new Vector3(0, 0.89f, 0);
        oakTree.transform.localScale = new Vector3(1, 1, 1);
    }
}