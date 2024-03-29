/*Prompt:

@"Create the first flower:
a. Right-click in the Hierarchy, select 3D Object > Sphere.
b. Rename and tag the Sphere to ""Flower1"".
c. Set the Transform Position to (2.5, 0.1, 2.5) and Scale to (0.1, 0.1, 0.1).
d. In the Inspector window, click on the Sphere's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable flower color (e.g., RGB: 255, 0, 255)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript6 : EditorWindow
{
    [MenuItem("Edit/Story/Step6")]
    public static void RunTask()
    {
        GameObject flower1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flower1.name = "Flower1";
        flower1.tag = "Flower";

        flower1.transform.position = new Vector3(2.5f, 0.1f, 2.5f);
        flower1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0f, 1f); // RGB: 255, 0, 255

        flower1.GetComponent<MeshRenderer>().material = material;
    }
}