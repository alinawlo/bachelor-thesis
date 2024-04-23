/*Prompt:

@"Create the Inner Circle of Medium Rocks:
1. Drag and drop the 'Assets/Prefabs/Rock_medium.prefab' into the Hierarchy.
2. Set the Transform Position to (0, 0.89, 0) to place the first rock at the center of the ground.
3. Set the Scale to (1, 1, 1).
4. Duplicate the rock 7 times.
5. Arrange the duplicated rocks in a smaller circle within the circle of flowers, maintaining an equal distance between each rock.

*/

using UnityEditor;
using UnityEngine;

public class UnityEditorScript3 : EditorWindow
{
    [MenuItem("Edit/Story/Step3")]
    public static void RunTask()
    {
        string prefabPath = "Assets/Prefabs/Rock_medium.prefab";
        GameObject rockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (rockPrefab == null)
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
            return;
        }

        Vector3 centerPosition = new Vector3(0, 0.89f, 0);
        float radius = 2f;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8;
            Vector3 newPosition = centerPosition + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            GameObject rock = (GameObject)PrefabUtility.InstantiatePrefab(rockPrefab);
            rock.transform.position = newPosition;
            rock.transform.localScale = Vector3.one;
        }
    }
}