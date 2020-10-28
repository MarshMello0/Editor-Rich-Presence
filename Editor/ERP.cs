#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ERP.Discord;

namespace ERP
{
    [InitializeOnLoad]
    public static class ERP
    {
        private const string applicationId = "509465267630374935";
        private const string prefix = "<b>ERP</b>";

        public static Discord.Discord discord { get; private set; }
        

        public static string projectName { get; private set; }
        public static string sceneName { get; private set; }
        public static bool showSceneName  = true;
        public static bool showProjectName = true;
        public static bool resetOnSceneChange = false;
        public static bool debugMode = false;
        public static bool EditorClosed = true;
        public static long lastTimestamp = 0;

        public static bool Failed;
        static ERP()
        {
            ERPSettings.GetSettings();
            DelayStart();
        }
        public static async void DelayStart(int delay = 1000)
        {
            await Task.Delay(delay);
            Init();
        }
        public static void Init()
        {
            try
            {
                discord = new Discord.Discord(long.Parse(applicationId), (long)CreateFlags.Default);
            }
            catch (Exception e)
            {
                if (debugMode)
                    LogWarning("Expected Error, retrying\n" + e.ToString());
                if (!Failed)
                    DelayStart(2000);
                Failed = true;
                return;
            }

            if (EditorClosed)
            {
                EditorClosed = false;
                lastTimestamp = GetTimestamp();
            }
            projectName = Application.productName;
            sceneName = EditorSceneManager.GetActiveScene().name;
            UpdateActivity();
            
            EditorApplication.update += Update;
            EditorSceneManager.sceneOpened += SceneOpened;
            EditorApplication.quitting += Quitting;
            Log("Started!");
        }

        private static void Quitting()
        {
            Log("Quitting ERP");
            EditorClosed = true;
            ERPSettings.SaveSettings();
        }

        private static void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (resetOnSceneChange)
                lastTimestamp = GetTimestamp();
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

            var activityManager = discord.GetActivityManager();

            Activity activity = new Activity
            {
                State = showProjectName ? projectName : "",
                Details = showSceneName ? sceneName : "",
                Timestamps =
                {
                    Start = lastTimestamp
                },
                Assets =
                {
                    LargeImage = "logo",
                    LargeText = "Unity " + Application.unityVersion,
                    SmallImage = "marshmello",
                    SmallText = "ERP on Github",
                },
            };

            activityManager.UpdateActivity(activity, result =>
            {
                if (result != Result.Ok)
                    LogError("Error from discord (" + result.ToString() + ")");
                else if (debugMode)
                    Log("Discord Result = " + result.ToString());
            });

            ERPSettings.SaveSettings();
        }
        public static long GetTimestamp()
        {
            long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (debugMode)
                Log("Got timestamp: " + unixTimestamp);
            return unixTimestamp;
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