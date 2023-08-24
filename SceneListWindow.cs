using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneListWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchQuery = "";
    private bool includeScenesNotInBuild = false;

    [MenuItem("Window/Scene List")]
    public static void ShowWindow()
    {
        SceneListWindow window = GetWindow<SceneListWindow>("Scene List");
        window.minSize = new Vector2(200, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene List", EditorStyles.boldLabel);

        GUILayout.Space(10);

        // Search bar
        searchQuery = EditorGUILayout.TextField("Search:", searchQuery);

        // Toggle button
        includeScenesNotInBuild = EditorGUILayout.Toggle("Project Scenes ", includeScenesNotInBuild);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        List<string> scenePaths = new List<string>();

        if (includeScenesNotInBuild)
        {
            string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene");

            scenePaths.AddRange(allSceneGuids.Select(AssetDatabase.GUIDToAssetPath));
        }
        else
        {
            scenePaths.AddRange(EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes));
        }

        foreach (string scenePath in scenePaths)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            bool isCurrentScene = EditorSceneManager.GetActiveScene().path == scenePath;

            if (!string.IsNullOrEmpty(searchQuery) && !sceneName.ToLower().Contains(searchQuery.ToLower()))
                continue;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            if (isCurrentScene)
                buttonStyle.normal.textColor = Color.green;

            if (GUILayout.Button(sceneName, buttonStyle, GUILayout.Height(30)))
            {
                if (!isCurrentScene)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(scenePath);
                }
            }
        }

        GUILayout.EndScrollView();
    }
}
