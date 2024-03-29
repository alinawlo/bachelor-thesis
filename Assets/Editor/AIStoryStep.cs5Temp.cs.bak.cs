/*Prompt:

@"Duplicate the first bush to create more bushes:
a. Select ""Bush1"" in the Hierarchy, press Ctrl+D to duplicate it.
b. Rename and tag the duplicated Sphere to ""Bush2"" and set its Transform Position to (4, 0.5, 4)."

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript5 : EditorWindow
{
    [MenuItem("Edit/Story/Step5")]
    public static void RunTask()
    {
        GameObject bush1 = GameObject.Find("Bush1");
        if (bush1 != null)
        {
            GameObject bush2 = GameObject.Instantiate(bush1, new Vector3(4f, 0.5f, 4f), Quaternion.identity);
            bush2.name = "Bush2";
            bush2.tag = "Bush";
        }
        else
        {
            Debug.LogError("Could not find the object named 'Bush1'.");
        }
    }
}