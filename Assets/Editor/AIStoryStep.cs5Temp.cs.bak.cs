/*Prompt:

@"Create the Directional Light:
1. Right-click in the Hierarchy.
2. Select Light > Directional Light. This will create a light source in your scene.
3. Set the Transform Rotation to (50, -30, 0) to simulate sunlight.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript5 : EditorWindow
{
    [MenuItem("Edit/Story/Step5")]
    private static void RunTask()
    {
        GameObject lightGameObject = new GameObject("Directional Light");
        lightGameObject.transform.rotation = Quaternion.Euler(50, -30, 0);
        lightGameObject.AddComponent<Light>().type = LightType.Directional;

        Debug.Log("Task completed: Directional Light created.");
    }
}