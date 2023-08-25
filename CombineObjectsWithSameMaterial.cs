using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CombineObjectsWithSameMaterial : EditorWindow
{
    private string combinedObjectName = "CombinedObject";
    private Material targetMaterial;
    private bool materialFoldout = true;
    private bool transformFoldout = true;

    [MenuItem("Window/Combine Objects")]
    public static void ShowWindow()
    {
        GetWindow<CombineObjectsWithSameMaterial>("Combine Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Combine Objects with Same Material", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        DrawSeparator();

        // Material Section
        materialFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(materialFoldout, "Material");
        if (materialFoldout)
        {
            targetMaterial = EditorGUILayout.ObjectField("Target Material:", targetMaterial, typeof(Material), false) as Material;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        DrawSeparator();

        // Transform Section
        transformFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(transformFoldout, "Transform");
        if (transformFoldout)
        {
            combinedObjectName = EditorGUILayout.TextField("Combined Object Name:", combinedObjectName);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        DrawSeparator();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Combine All Objects", "Combine all objects with the same material"), GUILayout.Width(200)))
        {
            CombineObjects(null);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Combine Objects with Target Material", "Combine objects with the target material"), GUILayout.Width(200)))
        {
            CombineObjects(targetMaterial);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }


    private void CombineObjects(Material materialFilter)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Dictionary<Material, List<MeshFilter>> materialToObjectMap = new Dictionary<Material, List<MeshFilter>>();

        foreach (MeshFilter meshFilter in FindStaticMeshFilters())
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                continue;

            Material material = meshRenderer.sharedMaterial;

            if (materialFilter != null && material != materialFilter)
                continue;

            if (!materialToObjectMap.ContainsKey(material))
            {
                materialToObjectMap[material] = new List<MeshFilter>();
            }

            materialToObjectMap[material].Add(meshFilter);
        }

        int combinedCount = 0;
        foreach (KeyValuePair<Material, List<MeshFilter>> pair in materialToObjectMap)
        {
            GameObject combinedObject = CreateCombinedObject(pair.Value.ToArray(), pair.Key, combinedCount);
            DeleteObjects(pair.Value.Select(meshFilter => meshFilter.gameObject).ToArray());
            combinedObject.name = combinedObjectName + combinedCount;
            Selection.activeGameObject = combinedObject;
            combinedCount++;
        }
    }

    private MeshFilter[] FindStaticMeshFilters()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();
        List<MeshFilter> staticMeshFilters = new List<MeshFilter>();

        foreach (GameObject rootObject in rootObjects)
        {
            staticMeshFilters.AddRange(rootObject.GetComponentsInChildren<MeshFilter>(true)
                .Where(meshFilter => meshFilter.gameObject.isStatic));
        }

        return staticMeshFilters.ToArray();
    }

    private GameObject CreateCombinedObject(MeshFilter[] meshFilters, Material material, int combinedCount)
    {
        GameObject combinedObject = new GameObject(combinedObjectName + combinedCount);
        MeshFilter meshFilterCombined = combinedObject.AddComponent<MeshFilter>();
        meshFilterCombined.sharedMesh = new Mesh();

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

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
    private void DrawSeparator()
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
    }
}
