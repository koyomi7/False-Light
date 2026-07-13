using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

public class FindMissingReferencesWindow : EditorWindow
{
    private class MissingRefData
    {
        public Object targetObject;
        public string propertyName;
        public string contextPath;
        public string expectedType;
    }

    private List<MissingRefData> foundReferences = new List<MissingRefData>();
    private Dictionary<string, bool> typeFoldouts = new Dictionary<string, bool>();
    private Vector2 scrollPosition;
    private GUIStyle richMiniLabel;
    
    private bool sortByType = true;
    private bool collapseAll = false;
    private bool expandAll = false;

    [MenuItem("Tools/Find Missing References")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingReferencesWindow>("Missing Refs");
    }

    private void OnGUI()
    {
        if (richMiniLabel == null)
        {
            richMiniLabel = new GUIStyle(EditorStyles.miniLabel);
            richMiniLabel.richText = true;
        }

        GUILayout.Label("Active Scene Missing References", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Single button for the active scene
        if (GUILayout.Button("Scan Active Scene", GUILayout.Height(30)))
        {
            ScanActiveScene();
        }

        EditorGUILayout.Space();

        if (foundReferences.Count > 0)
        {
            // Controls row
            EditorGUILayout.BeginHorizontal();
            
            sortByType = GUILayout.Toggle(sortByType, " Group by Type");
            
            if (sortByType)
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Expand All", GUILayout.Width(80)))
                {
                    ExpandAllGroups();
                }
                if (GUILayout.Button("Collapse All", GUILayout.Width(85)))
                {
                    CollapseAllGroups();
                }
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.Label($"Found {foundReferences.Count} missing references in {GetUniqueTypeCount()} categories:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (sortByType)
            {
                DrawGroupedByType();
            }
            else
            {
                DrawFlatList();
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Click 'Scan Active Scene' to begin.", EditorStyles.helpBox);
        }
    }

    private void DrawGroupedByType()
    {
        var grouped = foundReferences
            .OrderBy(x => x.expectedType)
            .ThenBy(x => x.contextPath)
            .GroupBy(x => x.expectedType);

        foreach (var group in grouped)
        {
            string typeName = group.Key;
            
            if (!typeFoldouts.ContainsKey(typeName))
            {
                typeFoldouts[typeName] = true;
            }

            EditorGUILayout.Space(4);
            
            // Reserve rect for the entire header row
            Rect headerRect = GUILayoutUtility.GetRect(0, 24, EditorStyles.helpBox);
            
            // Draw background
            GUI.Box(headerRect, "", EditorStyles.helpBox);
            
            // Draw foldout arrow with explicit positioning
            Rect foldoutRect = new Rect(headerRect.x + 6, headerRect.y + 2, headerRect.xMax - 84, headerRect.height - 4);
            typeFoldouts[typeName] = EditorGUI.Foldout(foldoutRect, typeFoldouts[typeName], "", true);
            
            // Draw label with proper offset to clear the arrow
            Rect labelRect = new Rect(headerRect.x + 26, headerRect.y + 2, headerRect.width - 110, headerRect.height - 4);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.richText = true;
            EditorGUI.LabelField(labelRect, $"<color=cyan>{typeName}</color> <color=grey>({group.Count()})</color>", headerStyle);
            
            // Draw Select All button right-aligned
            Rect buttonRect = new Rect(headerRect.xMax - 78, headerRect.y + 3, 72, 18);
            if (GUI.Button(buttonRect, "Select All"))
            {
                SelectGroupItems(group.ToList());
            }

            // Draw children if expanded
            if (typeFoldouts[typeName])
            {
                EditorGUI.indentLevel++;
                
                foreach (var data in group)
                {
                    DrawMissingRefItem(data);
                    EditorGUILayout.Space(2);
                }
                
                EditorGUI.indentLevel--;
            }
        }
    }

    private void DrawFlatList()
    {
        foreach (var data in foundReferences)
        {
            DrawMissingRefItem(data);
            EditorGUILayout.Space(2);
        }
    }

    private void DrawMissingRefItem(MissingRefData data)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(data.targetObject, typeof(Object), false, GUILayout.Width(250));
        
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            Selection.activeObject = data.targetObject;
            EditorGUIUtility.PingObject(data.targetObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"Expected Type: <color=cyan>{data.expectedType}</color>", richMiniLabel);
        EditorGUILayout.LabelField($"Path: {data.contextPath}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Missing Field: <color=red>{data.propertyName}</color>", richMiniLabel);

        EditorGUILayout.EndVertical();
    }

    private void SelectGroupItems(List<MissingRefData> items)
    {
        List<Object> objects = new List<Object>();
        foreach (var item in items)
        {
            if (item.targetObject != null)
            {
                objects.Add(item.targetObject);
            }
        }
        
        if (objects.Count > 0)
        {
            Selection.objects = objects.ToArray();
        }
    }

    private int GetUniqueTypeCount()
    {
        return foundReferences.Select(x => x.expectedType).Distinct().Count();
    }

    private void ExpandAllGroups()
    {
        var types = foundReferences.Select(x => x.expectedType).Distinct();
        foreach (var type in types)
        {
            typeFoldouts[type] = true;
        }
    }

    private void CollapseAllGroups()
    {
        var types = foundReferences.Select(x => x.expectedType).Distinct();
        foreach (var type in types)
        {
            typeFoldouts[type] = false;
        }
    }

    private void ScanActiveScene()
    {
        foundReferences.Clear();
        typeFoldouts.Clear();
        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.isLoaded)
        {
            Debug.LogWarning("No active scene is loaded!");
            return;
        }

        foreach (GameObject go in activeScene.GetRootGameObjects())
        {
            CheckGameObject(go, activeScene.name);
        }

        if (foundReferences.Count == 0) 
            Debug.Log("No missing references found in the active scene.");
    }

    private void CheckGameObject(GameObject go, string sceneName)
    {
        Component[] components = go.GetComponentsInChildren<Component>(true);

        foreach (Component comp in components)
        {
            if (comp == null)
            {
                foundReferences.Add(new MissingRefData
                {
                    targetObject = go,
                    propertyName = "Missing Script (MonoBehaviour)",
                    contextPath = $"{sceneName} > {GetFullPath(go)}",
                    expectedType = "MonoBehaviour"
                });
                continue;
            }

            CheckSerializedObject(comp, $"{sceneName} > {GetFullPath(comp.gameObject)}");
        }
    }

    private void CheckSerializedObject(Object obj, string contextPath)
    {
        SerializedObject serializedObject = new SerializedObject(obj);
        SerializedProperty iterator = serializedObject.GetIterator();
        
        System.Type objType = obj.GetType();

        while (iterator.NextVisible(true))
        {
            if (iterator.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0)
                {
                    string typeName = GetTypeNameFromProperty(iterator, objType, out string fieldName);

                    foundReferences.Add(new MissingRefData
                    {
                        targetObject = obj,
                        propertyName = iterator.name,
                        contextPath = contextPath,
                        expectedType = typeName
                    });
                }
            }
        }
    }

    private string GetTypeNameFromProperty(SerializedProperty prop, System.Type objType, out string fieldName)
    {
        fieldName = prop.name;
        
        // Handle arrays: "Array.data[0]" -> get the parent field name
        if (fieldName == "data" && prop.propertyPath.Contains(".Array.data["))
        {
            int dotIndex = prop.propertyPath.IndexOf(".Array.data[");
            if (dotIndex != -1) fieldName = prop.propertyPath.Substring(0, dotIndex);
        }

        // PRIMARY: Use SerializedProperty.type (works for built-in Unity components)
        string propType = prop.type;
        
        if (!string.IsNullOrEmpty(propType))
        {
            // Unity often stores as "PPtr<TypeName>" format
            if (propType.StartsWith("PPtr<") && propType.EndsWith(">"))
            {
                return propType.Substring(5, propType.Length - 6);
            }
            
            // Sometimes it's just the type name directly
            if (propType != "Object" && propType != "PPtr<Object>")
            {
                return propType;
            }
        }

        // FALLBACK: Use reflection for custom scripts
        System.Type resolvedType = ResolveFieldType(objType, fieldName);
        if (resolvedType != null)
        {
            return resolvedType.Name;
        }

        return "Unknown";
    }

    private System.Type ResolveFieldType(System.Type objType, string fieldName)
    {
        // Try direct field match (with hierarchy search)
        FieldInfo fieldInfo = objType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (fieldInfo != null) return GetCollectionElementType(fieldInfo.FieldType);

        // Try direct property match
        PropertyInfo propInfo = objType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (propInfo != null) return GetCollectionElementType(propInfo.PropertyType);

        // Handle Unity's 'm_' prefix for custom scripts
        if (fieldName.StartsWith("m_"))
        {
            string strippedName = fieldName.Substring(2);
            
            propInfo = objType.GetProperty(strippedName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            if (propInfo != null) return GetCollectionElementType(propInfo.PropertyType);

            fieldInfo = objType.GetField(strippedName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null) return GetCollectionElementType(fieldInfo.FieldType);
        }

        // Case-insensitive fallback
        fieldInfo = objType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
        if (fieldInfo != null) return GetCollectionElementType(fieldInfo.FieldType);

        return null;
    }

    private System.Type GetCollectionElementType(System.Type type)
    {
        if (type == null) return null;
        
        if (type.IsArray)
            return type.GetElementType();
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return type.GetGenericArguments()[0];
        
        return type;
    }

    private string GetFullPath(GameObject go)
    {
        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}