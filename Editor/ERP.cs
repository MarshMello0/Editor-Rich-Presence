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
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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
        public static bool showSceneName = true;
        public static bool showProjectName = true;
        public static bool resetOnSceneChange = false;
        public static bool debugMode = false;
        public static bool EditorClosed = true;
        public static long lastTimestamp = 0;
        public static long lastSessionID = 0;
        public static bool Errored = false;

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
            if (Errored && lastSessionID == EditorAnalyticsSessionInfo.id)
            {
                if (debugMode)
                    LogWarning($"Error but in same session");
                return;
            }

            if (!DiscordRunning())
            {
                LogWarning("Can't find Discord's Process");
                Failed = true;
                Errored = true;
                ERPSettings.SaveSettings();
                return;
            }

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

            if (!resetOnSceneChange || EditorAnalyticsSessionInfo.id != lastSessionID)
            {
                lastTimestamp = GetTimestamp();
                ERPSettings.SaveSettings();
            }

            lastSessionID = EditorAnalyticsSessionInfo.id;

            projectName = Application.productName;
            sceneName = EditorSceneManager.GetActiveScene().name;
            UpdateActivity();

            EditorApplication.update += Update;
            EditorSceneManager.sceneOpened += SceneOpened;
            Log("Started!");
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
                    SmallText = "ERP on Unity Asset Store",
                },
            };

            activityManager.UpdateActivity(activity, result =>
            {
                if (result != Result.Ok)
                    LogError("Error from discord (" + result.ToString() + ")");
                else
                    Log("Discord Result = " + result.ToString());
            });

            ERPSettings.SaveSettings();
        }
        public static long GetTimestamp()
        {
            if (!resetOnSceneChange)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(EditorAnalyticsSessionInfo.elapsedTime);
                long timestamp = DateTimeOffset.Now.Add(timeSpan).ToUnixTimeSeconds();
                Log("Got time stamp: " + timestamp);
                return timestamp;
            }
            long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Log("Got time stamp: " + unixTimestamp);
            return unixTimestamp;
        }
        public static void Log(object message)
        {
            if (debugMode)
                Debug.Log(prefix + ": " + message);
        }
        public static void LogWarning(object message)
        {
            if (debugMode)
                Debug.LogWarning(prefix + ": " + message);
        }
        public static void LogError(object message)
        {
            Debug.LogError(prefix + ": " + message);
        }

        private static bool DiscordRunning()
        {
            Process[] processes = Process.GetProcessesByName("Discord");

            if (debugMode)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    Log($"({i}/{processes.Length - 1})Found Process {processes[i].ProcessName}");
                }
            }
            return processes.Length != 0;
        }

    }
}
#endif