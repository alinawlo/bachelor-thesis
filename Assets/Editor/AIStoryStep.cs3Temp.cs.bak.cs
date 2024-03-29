/*Prompt:

@"Duplicate the first tree to create more trees:
a. Select ""TreeTrunk1"" in the Hierarchy, press Ctrl+D to duplicate it.
b. Rename and tag the duplicated Cube to ""TreeTrunk2"" and set its Transform Position to (3, 0.5, 3).
c. Select ""TreeTop1"" in the Hierarchy, press Ctrl+D to duplicate it.
d. Rename and tag the duplicated Sphere to ""TreeTop2"" and set its Transform Position to (3, 2, 3)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript3 : EditorWindow
{
    [MenuItem("Edit/Story/Step3")]
    private static void RunTask()
    {
        GameObject treeTrunk1 = GameObject.Find("TreeTrunk1");
        GameObject treeTop1 = GameObject.Find("TreeTop1");

        if (treeTrunk1 != null && treeTop1 != null)
        {
            GameObject treeTrunk2 = Instantiate(treeTrunk1, new Vector3(3f, 0.5f, 3f), Quaternion.identity);
            treeTrunk2.name = "TreeTrunk2";
            treeTrunk2.tag = "Tree";

            GameObject treeTop2 = Instantiate(treeTop1, new Vector3(3f, 2f, 3f), Quaternion.identity);
            treeTop2.name = "TreeTop2";
            treeTop2.tag = "Tree";
        }
        else
        {
            Debug.LogError("TreeTrunk1 or TreeTop1 not found in the scene.");
        }
    }
}