/*Prompt:

@"Create the first bush:
a. Right-click in the Hierarchy, select 3D Object > Sphere.
b. Rename and tag the Sphere to ""Bush1"".
c. Set the Transform Position to (2, 0.5, 2) and Scale to (0.5, 0.5, 0.5).
d. In the Inspector window, click on the Sphere's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable bush color (e.g., RGB: 0, 100, 0)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript4 : EditorWindow
{
    [MenuItem("Edit/Story/Step4")]
    public static void RunTask()
    {
        GameObject bush1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bush1.name = "Bush1";
        bush1.tag = "Bush";

        bush1.transform.position = new Vector3(2f, 0.5f, 2f);
        bush1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        MeshRenderer meshRenderer = bush1.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = new Color(0f, 1f, 0f);
        }
    }
}