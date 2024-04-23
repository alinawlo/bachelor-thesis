/*Prompt:

@"Create the Outer Circle of Blue Flowers:
1. Drag and drop the 'Assets/Prefabs/Flowers_blue.prefab' into the Hierarchy.
2. Set the Transform Position to (0, 0.89, 0) to place the first flower at the center of the ground.
3. Set the Scale to (1, 1, 1).
4. Duplicate the flower 39 times.
5. Arrange the duplicated flowers in a circle around the first flower, maintaining an equal distance between each flower.

*/

using UnityEngine;
using UnityEditor;

public class UnityEditorScript2 : EditorWindow
{
    [MenuItem("Edit/Story/Step2")]
    public static void RunTask()
    {
        GameObject flowerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Flowers_blue.prefab");

        if (flowerPrefab != null)
        {
            Vector3 centerPosition = new Vector3(0, 0.89f, 0);
            float radius = 5f;
            int numFlowers = 40;
            float angleIncrement = 360f / numFlowers;

            for (int i = 0; i < numFlowers; i++)
            {
                float angle = i * angleIncrement;
                float x = centerPosition.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = centerPosition.z + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 flowerPosition = new Vector3(x, centerPosition.y, z);

                GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("Flower prefab not found.");
        }
    }
}