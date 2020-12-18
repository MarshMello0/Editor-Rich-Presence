#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ERP
{
    public class ERPWindow : EditorWindow
    {
        private static ERPWindow _window;

        [MenuItem("Window/Editor Rich Presence")]
        private static void Init()
        {
            _window = (ERPWindow)GetWindow(typeof(ERPWindow), false, "Editor Rich Presence");
            _window.Show();
        }
        private void OnGUI()
        {
            if (ERP.discord == null && !ERP.Failed)
                ERP.DelayStart();

            if (ERP.Failed)
            {
                GUILayout.Label($"ERP Failed to start", EditorStyles.boldLabel);
                if (GUILayout.Button("Retry"))
                    ERP.Init();
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