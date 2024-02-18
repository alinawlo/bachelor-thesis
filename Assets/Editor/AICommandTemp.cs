using UnityEngine;
using UnityEditor;

public class UnityEditorScript : EditorWindow
{
    [MenuItem("Edit/Do Task")]
    private static void RunTask()
    {
        CreateGarden();
    }

    private static void CreateGarden()
    {
        // Create grass
        GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Plane);
        grass.transform.localScale = new Vector3(10f, 1f, 10f);
        grass.name = "Grass";

        // Create plants
        for (int i = 0; i < 3; i++)
        {
            GameObject plant = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plant.transform.position = new Vector3(i * 2f, 0.5f, i * 2f);
            plant.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            plant.name = "Plant_" + i;
        }

        // Create trees
        for (int i = 0; i < 3; i++)
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tree.transform.position = new Vector3(i * 2f, 1f, -i * 2f);
            tree.transform.localScale = new Vector3(1f, 2f, 1f);
            tree.name = "Tree_" + i;
        }

        // Create pathway
        GameObject pathway = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pathway.transform.position = new Vector3(0f, 0.01f, 0f);
        pathway.transform.localScale = new Vector3(3f, 0.02f, 3f);
        pathway.name = "Pathway";
    }
}