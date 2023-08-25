using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CombineObjectsWithSameMaterial : EditorWindow
{
    private string combinedObjectName = "CombinedObject";

    [MenuItem("Window/Combine Objects")]
    public static void ShowWindow()
    {
        GetWindow<CombineObjectsWithSameMaterial>("Combine Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Combine Objects with Same Material", EditorStyles.boldLabel);

        combinedObjectName = EditorGUILayout.TextField("Combined Object Name:", combinedObjectName);

        if (GUILayout.Button("Combine"))
        {
            CombineObjects();
        }
    }

    private void CombineObjects()
    {
        Material lastMaterial = null;
        CombineInstance[] combineInstances = new CombineInstance[0];
        int combinedCount = 0;

        GameObject[] staticMeshObjects = FindStaticMeshObjects();

        foreach (GameObject meshObject in staticMeshObjects)
        {
            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();

            if (meshFilter == null || meshRenderer == null)
                continue;

            if (lastMaterial == null || lastMaterial != meshRenderer.sharedMaterial)
            {
                if (lastMaterial != null)
                {
                    GameObject combinedObject = CreateCombinedObject(combineInstances, lastMaterial, combinedCount);
                    DeleteObjects(staticMeshObjects);
                    combinedObject.name = combinedObjectName + combinedCount;
                    Selection.activeGameObject = combinedObject;
                    return;
                }

                lastMaterial = meshRenderer.sharedMaterial;
                combineInstances = new CombineInstance[staticMeshObjects.Length];
            }

            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = meshFilter.sharedMesh;
            combineInstance.transform = meshFilter.transform.localToWorldMatrix;
            combineInstances[combinedCount] = combineInstance;
            combinedCount++;
        }

        if (lastMaterial != null)
        {
            GameObject combinedObject = CreateCombinedObject(combineInstances, lastMaterial, combinedCount);
            DeleteObjects(staticMeshObjects);
            combinedObject.name = combinedObjectName + combinedCount;
            Selection.activeGameObject = combinedObject;
        }
    }

    private GameObject[] FindStaticMeshObjects()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();
        System.Collections.Generic.List<GameObject> staticMeshObjects = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject rootObject in rootObjects)
        {
            staticMeshObjects.AddRange(rootObject.GetComponentsInChildren<MeshFilter>(true)
                .Where(meshFilter => meshFilter.gameObject.isStatic)
                .Select(meshFilter => meshFilter.gameObject));
        }

        return staticMeshObjects.ToArray();
    }

    private GameObject CreateCombinedObject(CombineInstance[] combineInstances, Material material, int combinedCount)
    {
        GameObject combinedObject = new GameObject(combinedObjectName + combinedCount);
        MeshFilter meshFilterCombined = combinedObject.AddComponent<MeshFilter>();
        meshFilterCombined.sharedMesh = new Mesh();
        meshFilterCombined.sharedMesh.CombineMeshes(combineInstances, true);
        MeshRenderer meshRendererCombined = combinedObject.AddComponent<MeshRenderer>();
        meshRendererCombined.sharedMaterial = material;
        return combinedObject;
    }

    private void DeleteObjects(GameObject[] objectsToDelete)
    {
        foreach (GameObject obj in objectsToDelete)
        {
            DestroyImmediate(obj);
        }
    }
}
