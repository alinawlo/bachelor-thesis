/*Prompt:

@"Create the Directional Light:
1. Right-click in the Hierarchy window.
2. Select Light > Directional Light. This will create a directional light source in your scene.
3. With the Directional Light object selected in the Hierarchy, set the Transform Rotation to (50, -30, 0) in the Inspector window.

*/

using UnityEditor;
using UnityEngine;

public class UnityEditorScript4
{
    [MenuItem("Edit/Story/Step4")]
    public static void RunTask()
    {
        GameObject lightObject = new GameObject("Directional Light");
        Light lightComponent = lightObject.AddComponent<Light>();
        lightComponent.type = LightType.Directional;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Camera cameraComponent = new Camera();
        cameraComponent.transform.position = new Vector3(0f, 1f, -10f);
        cameraComponent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}