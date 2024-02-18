using UnityEditor;
using UnityEngine;

public class UnityEditorScript : EditorWindow
{
    [MenuItem("Edit/Do Task")]
    private static void RunTask()
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = randomPosition;
        }
    }
}