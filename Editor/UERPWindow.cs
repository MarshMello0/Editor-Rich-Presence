using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UERP
{
    public class UERPWindow : EditorWindow
    {
        private static UERPWindow _window;

        [MenuItem("Window/UERP")]
        private static void Init()
        {
            _window = (UERPWindow)GetWindow(typeof(UERPWindow), false, "UERP");
            _window.Show();
        }
        private void OnGUI()
        {
            if (UERP.discord == null && !UERP.Failed)
                UERP.DelayStart();

            if (UERP.Failed)
            {
                GUILayout.Label($"UERP Failed to start", EditorStyles.boldLabel);
                if (GUILayout.Button("Retry"))
                    UERP.Init();
                return;
            }
            GUILayout.Label("Unity Editor Rich Presence", EditorStyles.boldLabel);

            GUILayout.Label("Current Project: " + UERP.projectName);
            GUILayout.Label("Current Scene: " + UERP.sceneName);
            GUILayout.Label(string.Empty);
            GUILayout.Label($"Scene Name Visible: {UERP.showSceneName}");
            GUILayout.Label($"Project Name Visible: {UERP.showProjectName}");
            GUILayout.Label($"Reset Timestap on scene change: {UERP.resetOnSceneChange}");

            if (ToggleButton("Hide Scene name","Show Scene name", ref UERP.showSceneName))
            {
                UERP.UpdateActivity();
            }
            if (ToggleButton("Hide Project name", "Show Project name", ref UERP.showProjectName))
            {
                UERP.UpdateActivity();
            }
            if (ToggleButton("Don't reset timestap on scene change", "Reset timestap on scene change", ref UERP.resetOnSceneChange))
            {
                UERP.UpdateActivity();
            }
            if (ToggleButton("Disable Debug Mode", "Enable Debug Mode", ref UERP.debugMode))
            {
                UERPSettings.SaveSettings();
            }
            GUILayout.Label(string.Empty);
            if (GUILayout.Button("Open UERP on Github"))
            {
                Application.OpenURL("https://github.com/MarshMello0/UERP");
            }
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