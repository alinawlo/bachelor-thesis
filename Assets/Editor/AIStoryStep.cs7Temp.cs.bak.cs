/*Prompt:

@"Duplicate the first flower to create more flowers:
a. Select ""Flower1"" in the Hierarchy, press Ctrl+D to duplicate it.
b. Rename and tag the duplicated Sphere to ""Flower2"" and set its Transform Position to (3.5, 0.1, 3.5)."
*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript7 : EditorWindow
{
    [MenuItem("Edit/Story/Step7")]
    private static void RunTask()
    {
        GameObject flower1 = GameObject.Find("Flower1");
        if (flower1 != null)
        {
            GameObject flower2 = GameObject.Instantiate(flower1, new Vector3(3.5f, 0.1f, 3.5f), Quaternion.identity);
            flower2.name = "Flower2";
            flower2.tag = "Flower";
        }
        else
        {
            Debug.LogError("Flower1 not found in the scene. Please make sure the object exists with the correct name.");
        }
    }
}