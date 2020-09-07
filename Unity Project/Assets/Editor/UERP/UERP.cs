#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;

namespace UERP
{
    [InitializeOnLoad]
    public static class UERP
    {
        private const string applicationId = "509465267630374935";
        private const string prefix = "<b>UERP</b>";

        public static Discord.Discord discord { get; private set; }
        private static long lastTimestamp;

        public static string projectName { get; private set; }
        public static string sceneName { get; private set; }
        public static bool showSceneName  = true;
        public static bool showProjectName = true;
        public static bool resetOnSceneChange = false;
        public static bool debugMode = false;
        static UERP()
        {
            Init();
        }
        [MenuItem("UERP/Github")]
        private static void OpenGithub()
        {
            Application.OpenURL("https://github.com/MarshMello0/UERP");
        }
        public static void Init()
        {
            discord = new Discord.Discord(long.Parse(applicationId), (long)Discord.CreateFlags.Default);
            UERPSettings.GetSettings();
            projectName = Application.productName;
            sceneName = EditorSceneManager.GetActiveScene().name;
            lastTimestamp = long.Parse(GetTimestamp());
            UpdateActivity();
            
            EditorApplication.update += Update;
            EditorSceneManager.sceneOpened += SceneOpened;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            Log("Started!");
        }

        private static void PlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode || obj == PlayModeStateChange.EnteredEditMode)
            {
                if (debugMode)
                    Log("PlayMode State Changed = " + obj);
                UpdateActivity();
            }
        }

        private static void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (resetOnSceneChange)
                lastTimestamp = long.Parse(GetTimestamp());
            sceneName = EditorSceneManager.GetActiveScene().name;
            UpdateActivity();
        }

        private static void Update()
        {
            if (discord != null)
                discord.RunCallbacks();

        }
        public static void UpdateActivity()
        {
            if (debugMode)
                Log("Updating Activity");
            if (discord == null)
                Init();

            projectName = Application.productName;
            sceneName = EditorSceneManager.GetActiveScene().name;

            if (EditorApplication.isPlaying)
                sceneName = "Playing: " + sceneName;

            var activityManager = discord.GetActivityManager();

            var activity = new Activity
            {
                State = showProjectName? projectName: "",
                Details = showSceneName? sceneName : "",
                Timestamps =
            {
                Start = lastTimestamp
            },
                Assets =
            {
                LargeImage = "logo",
                LargeText = "Unity " + Application.unityVersion,
                SmallImage = "marshmello",
                SmallText = "UERP on Github",
            },
            };

            activityManager.UpdateActivity(activity, result =>
            {
                if (result != Result.Ok)
                    LogError("Error from discord (" + result.ToString() + ")");
                else if (debugMode)
                    Log("Discord Result = " + result.ToString());
            });

            UERPSettings.SaveSettings();
        }
        public static String GetTimestamp()
        {
            long unixTimestamp = (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (debugMode)
                Log("Got timestamp: " + unixTimestamp);
            return unixTimestamp.ToString();
        }
        public static void Log(object message)
        {
            Debug.Log(prefix + ": " + message);
        }
        public static void LogWarning(object message)
        {
            Debug.LogWarning(prefix + ": " + message);
        }
        public static void LogError(object message)
        {
            Debug.LogError(prefix + ": " + message);
        }

    }
}
#endif