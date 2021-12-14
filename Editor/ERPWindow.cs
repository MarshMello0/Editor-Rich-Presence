#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ERP
{
    public class ERPWindow : EditorWindow
    {
        private static ERPWindow _window;
        
        private static Font _fontRegular;
        private static Font _fontHeader;
        private static GUIStyle _headerStyle;
        private static GUIStyle _textStyle;
        private static Texture _unityLogo;
        private static Texture _background;

        [MenuItem("Window/Editor Rich Presence")]
        private static void Init()
        {
            _window = (ERPWindow)GetWindow(typeof(ERPWindow), false, "Editor Rich Presence");
            _window.Show();
        }
        
        /*
        private void OnGUI()
        {
            if (ERP.discord == null && !ERP.Failed)
                ERP.DelayStart();

            if (ERP.Failed | ERP.Errored)
            {
                GUILayout.Label($"ERP Failed to start", EditorStyles.boldLabel);
                if (GUILayout.Button("Retry"))
                {
                    ERP.Errored = false;
                    ERP.Failed = false;
                    ERP.Init();
                }
                return;
            }
            GUILayout.Label("Editor Rich Presence", EditorStyles.boldLabel);

            GUILayout.Label("Current Project: " + ERP.projectName);
            GUILayout.Label("Current Scene: " + ERP.sceneName);
            GUILayout.Label(string.Empty);
            GUILayout.Label($"Scene Name Visible: {ERP.showSceneName}");
            GUILayout.Label($"Project Name Visible: {ERP.showProjectName}");
            GUILayout.Label($"Reset Timestap on scene change: {ERP.resetOnSceneChange}");

            if (ToggleButton("Hide Scene name", "Show Scene name", ref ERP.showSceneName))
            {
                ERP.UpdateActivity();
                ERPSettings.SaveSettings();
            }
            if (ToggleButton("Hide Project name", "Show Project name", ref ERP.showProjectName))
            {
                ERP.UpdateActivity();
                ERPSettings.SaveSettings();
            }
            if (ToggleButton("Don't reset timestap on scene change", "Reset timestap on scene change", ref ERP.resetOnSceneChange))
            {
                ERP.UpdateActivity();
                ERPSettings.SaveSettings();
            }
            if (ToggleButton("Disable Debug Mode", "Enable Debug Mode", ref ERP.debugMode))
            {
                ERPSettings.SaveSettings();
            }
            GUILayout.Label(string.Empty);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("GitHub Repository"))
            {
                Application.OpenURL("https://github.com/MarshMello0/Editor-Rich-Presence");
            }
            if (GUILayout.Button("Asset Store Page"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/editor-rich-presence-178736");
            }
            GUILayout.EndHorizontal();

        }
        */

        private static void LoadAssets()
        {
            _fontRegular = AssetDatabase.LoadAssetAtPath<Font>("Assets\\ERP\\Fonts\\Montserrat-Regular.ttf");
            _fontHeader = AssetDatabase.LoadAssetAtPath<Font>("Assets\\ERP\\Fonts\\Montserrat-SemiBold.ttf");
            _unityLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets\\ERP\\Images\\UnityLogo.png");
            _background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets\\ERP\\Images\\Background.png");
        }

        private static void CreateStyles()
        {
            _headerStyle = new GUIStyle()
            {
                fontSize = 16,
                font = _fontHeader,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0,0,10,0),
                normal =
                {
                    textColor = Color.white
                }
            };
            _textStyle = new GUIStyle()
            {
                fontSize = 16,
                font = _fontRegular,
                normal =
                {
                    textColor = Color.white
                },
                padding = new RectOffset(5,0,0,0)
            };
        }
        
        private void OnGUI()
        {
            LoadAssets();
            CreateStyles();

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _background, ScaleMode.StretchToFill);
            
            GUILayout.Label("Editor Rich Presence", _headerStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(_unityLogo, GUILayout.Height(60f), GUILayout.Width(60f));
            
            GUILayout.BeginVertical();
            GUILayout.Label("Unity", _textStyle);
            GUILayout.Label(ERP.projectName, _textStyle);
            TimeSpan difference = DateTime.Now.TimeOfDay - TimeSpan.FromTicks(ERP.lastTimestamp);
            GUILayout.Label($"{difference.Hours}:{difference.Minutes} elapsed", _textStyle);
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Scene name visibility", _textStyle);
            GUILayout.Label("Project name visibility", _textStyle);
            GUILayout.Label("Reset time on scene change", _textStyle);
            GUILayout.Label("Debug mode", _textStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.EnumPopup(string.Empty, test);
            test = (Visibility)EditorGUILayout.EnumPopup(string.Empty, test);
            Test3 = (Status)EditorGUILayout.EnumPopup(string.Empty, Test3);
            test2 = (Bool)EditorGUILayout.EnumPopup(string.Empty, test2);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("Unity Asset Store");
            GUILayout.Button("Source Code");
            EditorGUILayout.EndHorizontal();
            
        }


        private Visibility test;
        private Bool test2;
        private Status Test3;
        
        private bool ToggleButton(string trueText, string falseText, ref bool value)
        {
            if (value && GUILayout.Button(trueText))
            {
                value = false;
                return true;
            }
            else if (!value && GUILayout.Button(falseText))
            {
                value = true;
                return true;
            }
            return false;
        }

        enum Visibility
        {
            Visible,
            Hidden
        }

        enum Bool
        {
            True,
            False
        }

        enum Status
        {
            Enabled,
            Disable
        }
    }
}
#endif