/*Prompt:

@"Create the first tree:
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Rename and tag the Cube to ""TreeTrunk1"".
c. Set the Transform Position to (1, 0.5, 1) and Scale to (0.2, 1, 0.2).
d. In the Inspector window, click on the Cube's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable tree trunk color (e.g., RGB: 139, 69, 19).
e. Right-click on ""TreeTrunk1"" in the Hierarchy, select 3D Object > Sphere.
f. Rename and tag the Sphere to ""TreeTop1"".
g. Set the Transform Position to (0, 1.5, 0) and Scale to (1, 1, 1).
h. In the Inspector window, click on the Sphere's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable tree color (e.g., RGB: 34, 139, 34)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript2 : EditorWindow
{
    [MenuItem("Edit/Story/Step2")]
    public static void RunTask()
    {
        GameObject treeTrunk1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        treeTrunk1.name = "TreeTrunk1";
        treeTrunk1.tag = "Tree";

        treeTrunk1.transform.position = new Vector3(1f, 0.5f, 1f);
        treeTrunk1.transform.localScale = new Vector3(0.2f, 1f, 0.2f);

        Material trunkMaterial = new Material(Shader.Find("Standard"));
        trunkMaterial.color = new Color32(139, 69, 19, 255);
        treeTrunk1.GetComponent<Renderer>().material = trunkMaterial;

        GameObject treeTop1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        treeTop1.name = "TreeTop1";
        treeTop1.tag = "Tree";

        treeTop1.transform.position = new Vector3(0f, 1.5f, 0f);
        treeTop1.transform.localScale = Vector3.one;

        Material topMaterial = new Material(Shader.Find("Standard"));
        topMaterial.color = new Color32(34, 139, 34, 255);
        treeTop1.GetComponent<Renderer>().material = topMaterial;
    }
}