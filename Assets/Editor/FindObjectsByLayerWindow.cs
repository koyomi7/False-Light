using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindObjectsByLayerWindow : EditorWindow
{
    // Changed from string to int to work directly with Unity's layer system
    private int selectedLayerIndex = 0; 
    private List<GameObject> foundObjects = new List<GameObject>();
    private Vector2 scrollPosition;
    
    // Tracks if the user has clicked "Find" yet, so we don't show errors on startup
    private bool hasSearched = false;

    [MenuItem("Tools/Find Objects by Layer")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectsByLayerWindow>("Find by Layer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Objects by Layer", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        
        // Replaced TextField with LayerField (Creates the dropdown automatically)
        selectedLayerIndex = EditorGUILayout.LayerField("Layer:", selectedLayerIndex);
        
        if (GUILayout.Button("Find", GUILayout.Width(60)))
        {
            FindObjectsWithLayer();
            hasSearched = true;
        }
        EditorGUILayout.EndHorizontal();

        // Display results
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Found {foundObjects.Count} objects:", EditorStyles.boldLabel);
            
            // Bonus: Added a "Select All" button
            if (GUILayout.Button("Select All", GUILayout.Width(80)))
            {
                Selection.objects = foundObjects.ToArray();
            }
            EditorGUILayout.EndHorizontal();

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
        else if (hasSearched)
        {
            // Convert the int back to a string just for the warning message
            string layerName = LayerMask.LayerToName(selectedLayerIndex);
            GUILayout.Label($"No objects found on layer '{layerName}'.", EditorStyles.helpBox);
        }
    }

    private void FindObjectsWithLayer()
    {
        foundObjects.Clear();

        // Because we use LayerField, the index is guaranteed to be valid (0-31),
        // so we no longer need to check if the string matches an existing layer.
        
        // Find all GameObjects in the scene (including inactive ones)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == selectedLayerIndex)
            {
                foundObjects.Add(obj);
            }
        }

        // Sort the list alphabetically
        foundObjects.Sort((a, b) => a.name.CompareTo(b.name));

        string layerName = LayerMask.LayerToName(selectedLayerIndex);
        Debug.Log($"Found {foundObjects.Count} objects with layer '{layerName}'");
    }
}