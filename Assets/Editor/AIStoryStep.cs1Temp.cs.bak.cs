/*Prompt:
@"Create the Ground:
1. In the Project window, navigate to Assets/SimpleNaturePack/Prefabs/.
2. Drag and drop the Ground_01.prefab into the Hierarchy window.
3. With the Ground_01 object selected in the Hierarchy, set the Transform Position to (0, 0, 0) in the Inspector window.
4. Set the Scale to (10, 1, 10) to create a large ground area for the garden.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript1 : EditorWindow
{
    [MenuItem("Edit/Story/Step1")]
    static void RunTask()
    {
        GameObject groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/SimpleNaturePack/Prefabs/Ground_01.prefab");

        GameObject groundObject = Instantiate(groundPrefab);
        groundObject.transform.position = Vector3.zero;
        groundObject.transform.localScale = new Vector3(10, 1, 10);
    }
}