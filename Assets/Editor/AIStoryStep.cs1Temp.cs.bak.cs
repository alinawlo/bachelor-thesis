/*Prompt:
@"Create the ground (grass):
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Rename and tag the Cube to ""Ground"".
c. Set the Transform Position to (0, 0, 0) and Scale to (10, 0.1, 10).
d. In the Inspector window, click on the Cube's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable grass color (e.g., RGB: 0, 200, 0)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript1 : EditorWindow
{
    [MenuItem("Edit/Story/Step1")]
    private static void RunTask()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.tag = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(10, 0.1f, 10);

        MeshRenderer renderer = ground.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = new Color32(0, 200, 0, 255);
    }
}