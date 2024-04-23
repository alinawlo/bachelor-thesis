/*Prompt:
@"Create the Ground:
1. Drag and drop the 'Assets/Prefabs/Ground_01.prefab' into the Hierarchy.
2. Set the Transform Position to (0, 0, 0) to place the ground at the center of the scene.
3. Set the Scale to (100, 1, 100) to make it a large ground area.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript1 : EditorWindow
{
    [MenuItem("Edit/Story/Step1")]
    public static void RunTask()
    {
        GameObject groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Ground_01.prefab");

        GameObject ground = Instantiate(groundPrefab);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(100f, 1f, 100f);
    }
}