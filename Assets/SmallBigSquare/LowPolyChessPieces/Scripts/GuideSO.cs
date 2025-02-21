using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBS.LPCP
{
	[CreateAssetMenu(fileName = "Guide", menuName = "SBS/LPCP/GUIDE")]
	public class GuideSO : ScriptableObject
	{
        public string nameAndVer = "LOW POLY CHESS PIECES";
        public string smallName = "Low Poly Chess Pieces";
        public string docs = "";                                    
        public string onlineDocs = "";
        public string tuts = "";
        public string forum = "";
        public string mail = "smallbigsquare@gmail.com";
        public string rate = "https://assetstore.unity.com/packages/slug/310624";
        public string pub = "https://assetstore.unity.com/publishers/90279";
    }
}

namespace SBS.LPCP
{
    using System.Xml.Linq;
    using Unity.VisualScripting;
#if UNITY_EDITOR
    using UnityEditor;
    using TARGET = GuideSO;

	[CustomEditor(typeof(TARGET))]
    public class GuideSOEditor : Editor
	{
        private static GUIStyle titleStyle;
        private static GUIStyle subTitleStyle;
        private static GUIStyle regionStyle;
        private static GUIStyle bodyStyle;

        public static void setStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 20;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;

                subTitleStyle = new GUIStyle(EditorStyles.label);
                subTitleStyle.fontSize = 15;
                subTitleStyle.fontStyle = FontStyle.Bold;
                subTitleStyle.alignment = TextAnchor.MiddleCenter;

                regionStyle = new GUIStyle(EditorStyles.label);
                regionStyle.fontSize = 16;
                regionStyle.fontStyle = FontStyle.Bold;

                bodyStyle = new GUIStyle(EditorStyles.label);
                bodyStyle.wordWrap = true;
                bodyStyle.fontSize = 13;
            }
        }

        public override void OnInspectorGUI()
		{
            setStyles();
            TARGET t = (TARGET)target;

            GUILayout.Label(t.nameAndVer, titleStyle);
            GUILayout.Label("GUIDE", subTitleStyle);
            GUILayout.Space(20);           
            
            GUILayout.Label("Thank you for using " + t.smallName + "!", regionStyle);
            
            GUILayout.Space(20);

            GUILayout.Label("Need help?", regionStyle);
            
            if (GUILayout.Button(new GUIContent("E-Mail Me", t.mail)) == true)
            {
                Application.OpenURL("mailto:" + t.mail);
            }

            GUILayout.Space(20);

            GUILayout.Label("Rate the asset. THANKS!", regionStyle);
            GUILayout.Label("If you like the asset please rate it. It will help me to create more high quality assets.", bodyStyle);

            if (GUILayout.Button(new GUIContent("Rate This Asset", "Unity Asset Store")) == true)
            {
                Application.OpenURL(t.rate);
            }

            GUILayout.Space(20);

            GUILayout.Label("Want more?", regionStyle);
            GUILayout.Label("Check out my other assets. You may like them as well.", bodyStyle);

            if (GUILayout.Button(new GUIContent("MORE", "Unity Asset Store")) == true)
            {
                Application.OpenURL(t.pub);
            }

            GUILayout.Space(50);

            GUILayout.Label("For URP users", regionStyle); 
            GUILayout.Label("Select->Window / Rendering / Render Pipeline Converter/", bodyStyle);
            GUILayout.Label("Check->Material Upgrade", bodyStyle);
            GUILayout.Label("Click->Initialize and Convert", bodyStyle);

            GUILayout.Space(30);

            GUILayout.Label("For HDRP users", regionStyle);
            GUILayout.Label("Select->Edit / Rendering / Materials / Convert All Built-in materials to HDRP", bodyStyle);            
        }
    }

#endif
}