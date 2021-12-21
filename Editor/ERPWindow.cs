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
        enum Visibility
        {
            Visible,
            Hidden
        }

        enum Status
        {
            Enabled,
            Disable
        }
        
        public static ERPWindow Window;
        
        private static Font _fontRegular;
        private static Font _fontHeader;
        private static GUIStyle _headerStyle;
        private static GUIStyle _textStyle;
        private static Texture _unityLogo;
        private static Texture _background;
        
        private static Visibility _sceneNameVisibility;
        private static Visibility _projectNameVisibility;
        private static Status _resetOnSceneChange;
        private static Status _debugMode;

        [MenuItem("Window/Editor Rich Presence")]
        private static void Init()
        {
            Window = (ERPWindow)GetWindow(typeof(ERPWindow), false, "Editor Rich Presence");
            Window.Show();
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

        private void OnInspectorUpdate() => Repaint();

        private void OnGUI()
        {
            LoadAssets();
            CreateStyles();

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), _background, ScaleMode.StretchToFill);
            
            GUILayout.Label("Editor Rich Presence", _headerStyle);

            if (ERP.Errored)
            {
                ErrorUI();
                Links();
                return;
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(_unityLogo, GUILayout.Height(60f), GUILayout.Width(60f));
            
            GUILayout.BeginVertical();
            GUILayout.Label("Unity", _textStyle);
            ERP.Log($"{ERP.SceneName}|{ERP.ProjectName}");
            if (ERP.ShowSceneName)
                GUILayout.Label(ERP.SceneName, _textStyle);
            if (ERP.ShowProjectName)
                GUILayout.Label(ERP.ProjectName, _textStyle);
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
            _sceneNameVisibility = (Visibility)EditorGUILayout.EnumPopup(string.Empty, _sceneNameVisibility);
            _projectNameVisibility = (Visibility)EditorGUILayout.EnumPopup(string.Empty, _projectNameVisibility);
            _resetOnSceneChange = (Status)EditorGUILayout.EnumPopup(string.Empty, _resetOnSceneChange);
            _debugMode = (Status)EditorGUILayout.EnumPopup(string.Empty, _debugMode);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            Links();

            CheckVariables();
        }

        private static void Links()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Unity Asset Store"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/editor-rich-presence-178736");
            }

            if (GUILayout.Button("Source Code"))
            {
                Application.OpenURL("https://github.com/MarshMello0/Editor-Rich-Presence");
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void CheckVariables()
        {
            ERP.ShowSceneName = _sceneNameVisibility == Visibility.Visible;
            ERP.ShowProjectName = _projectNameVisibility == Visibility.Visible;
            ERP.ResetOnSceneChange = _resetOnSceneChange == Status.Enabled;
            ERP.DebugMode = _debugMode == Status.Enabled;
        }

        private void ErrorUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("An error has occurred", _textStyle);
            if (GUILayout.Button("Retry"))
            {
                ERP.Errored = false;
                ERP.Init();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
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
    }
}
#endif