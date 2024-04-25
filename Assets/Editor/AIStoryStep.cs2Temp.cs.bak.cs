/*Prompt:

@"Create the first tree trunk:
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Rename and tag the Cube to ""Tree1_Trunk"".
c. Set the Transform Position to (2, 0.5, 2) and Scale to (0.2, 1, 0.2).
d. In the Inspector window, click on the Cube's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable tree trunk color (e.g., RGB: 139, 69, 19)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript2 : EditorWindow
{
    [MenuItem("Edit/Story/Step2")]
    public static void RunTask()
    {
        GameObject treeTrunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        treeTrunk.name = "Tree1_Trunk";
        treeTrunk.tag = "Tree";

        treeTrunk.transform.position = new Vector3(2f, 0.5f, 2f);
        treeTrunk.transform.localScale = new Vector3(0.2f, 1f, 0.2f);

        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = new Color(139f / 255f, 69f / 255f, 19f / 255f);
        
        MeshRenderer renderer = treeTrunk.GetComponent<MeshRenderer>();
        renderer.material = newMaterial;
    }
}