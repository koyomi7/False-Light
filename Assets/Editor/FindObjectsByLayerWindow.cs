using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindObjectsByLayerWindow : EditorWindow
{
    private string layerName = "";
    private List<GameObject> foundObjects = new List<GameObject>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Find Objects by Layer")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectsByLayerWindow>("Find by Layer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Objects by Layer Name", EditorStyles.boldLabel);

        // Layer name input field
        EditorGUILayout.BeginHorizontal();
        layerName = EditorGUILayout.TextField("Layer Name:", layerName);
        if (GUILayout.Button("Find", GUILayout.Width(60)))
        {
            FindObjectsWithLayer();
        }
        EditorGUILayout.EndHorizontal();

        // Display results
        if (foundObjects.Count > 0)
        {
            GUILayout.Label($"Found {foundObjects.Count} objects:", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var obj in foundObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        else if (!string.IsNullOrEmpty(layerName))
        {
            GUILayout.Label("No objects found with this layer.", EditorStyles.helpBox);
        }
    }

    private void FindObjectsWithLayer()
    {
        foundObjects.Clear();
        
        // Get the layer index from the name
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' does not exist.");
            return;
        }

        // Find all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layerIndex)
            {
                foundObjects.Add(obj);
            }
        }

        // Sort the list alphabetically
        foundObjects.Sort((a, b) => a.name.CompareTo(b.name));

        Debug.Log($"Found {foundObjects.Count} objects with layer '{layerName}'");
    }
}