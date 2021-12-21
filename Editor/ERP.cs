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
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace ERP
{
    [InitializeOnLoad]
    public static class ERP
    {
        private const string _applicationId = "509465267630374935";
        private const string _prefix = "<b>Editor Rich Presence</b>";

        public static Discord.Discord discord { get; private set; }


        public static string ProjectName { get; private set; }
        public static string SceneName { get; private set; }
        public static bool ShowSceneName
        {
            set
            {
                if (value == _showSceneName)
                    return;
                
                _showSceneName = value;
                ERPSettings.SaveSettings();
                UpdateActivity();
            }
            get
            {
                return _showSceneName;
            } 
        }
        public static bool ShowProjectName
        {
            set
            {
                if (value == _showProjectName)
                    return;
                
                _showProjectName = value;
                ERPSettings.SaveSettings();
                UpdateActivity();
            }
            get
            {
                return _showProjectName;
            } 
        }
        public static bool ResetOnSceneChange
        {
            set
            {
                if (value == _resetOnSceneChange)
                    return;
                
                _resetOnSceneChange = value;
                ERPSettings.SaveSettings();
                UpdateActivity();
            }
            get
            {
                return _resetOnSceneChange;
            } 
        }
        public static bool DebugMode
        {
            set
            {
                if (value == _debugMode)
                    return;
                
                _debugMode = value;
                ERPSettings.SaveSettings();
            }
            get
            {
                return _debugMode;
            }
        }
        
        public static bool EditorClosed = true;
        public static long lastTimestamp = 0;
        public static long lastSessionID = 0;
        public static bool Errored = false;

        public static bool Failed;

        private static bool _showSceneName = true;
        private static bool _showProjectName = true;
        private static bool _resetOnSceneChange = false;
        private static bool _debugMode = false;
        
        static ERP()
        {
            ERPSettings.GetSettings();
            DelayStart();
        }

        public static void LoadSettings(ERPSettings settings)
        {
            _showSceneName = settings.showSceneName;
            _showProjectName = settings.showProjectName;
            _resetOnSceneChange = settings.resetOnSceneChange;
            _debugMode = settings.debugMode;
            EditorClosed = settings.EditorClosed;
            lastTimestamp = settings.LastTimestamp;
            lastSessionID = settings.LastSessionID;
            Errored = settings.Errored;
            Log("Applied Settings from file");
            GetNames();
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
                if (DebugMode)
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
                if (discord == null)
                {
                    Log("Creating new discord");
                    discord = new Discord.Discord(long.Parse(_applicationId), (long)CreateFlags.Default);
                }
                else
                {
                    LogError("Discord isn't null but we are trying to creating a new one");
                }
            }
            catch (Exception e)
            {
                if (DebugMode)
                    LogWarning($"Expected Error, retrying\n{e}");
                if (!Failed)
                    DelayStart(2000);
                Failed = true;
                return;
            }

            if (!ResetOnSceneChange || EditorAnalyticsSessionInfo.id != lastSessionID)
            {
                lastTimestamp = GetTimestamp();
                ERPSettings.SaveSettings();
            }

            lastSessionID = EditorAnalyticsSessionInfo.id;

            GetNames();
            UpdateActivity();

            EditorApplication.update += Update;
            EditorSceneManager.sceneOpened += SceneOpened;
            Log("Started!");

            if (ERPWindow.Window != null)
            {
                ERPWindow.Window.Repaint();
            }
        }

        private static void GetNames()
        {
            ProjectName = Application.productName;
            SceneName = SceneManager.GetActiveScene().name;
        }

        private static void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (ResetOnSceneChange)
                lastTimestamp = GetTimestamp();
            SceneName = EditorSceneManager.GetActiveScene().name;
            UpdateActivity();
        }

        private static void Update()
        {
            if (discord != null)
                discord.RunCallbacks();

        }
        
        private static void UpdateActivity()
        {
            Log("Updating Activity");
            if (discord == null)
                Init();

            if (Failed)
                return;

            ProjectName = Application.productName;
            SceneName = EditorSceneManager.GetActiveScene().name;

            var activityManager = discord.GetActivityManager();

            Activity activity = new Activity
            {
                State = ShowProjectName ? ProjectName : "",
                Details = ShowSceneName ? SceneName : "",
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
            if (!ResetOnSceneChange)
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
            if (DebugMode)
                Debug.Log(_prefix + ": " + message);
        }
        
        public static void LogWarning(object message)
        {
            if (DebugMode)
                Debug.LogWarning(_prefix + ": " + message);
        }
        
        public static void LogError(object message)
        {
            Debug.LogError(_prefix + ": " + message);
        }
        
        private static bool DiscordRunning()
        {
            Process[] processes = Process.GetProcessesByName("Discord");

            if (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("DiscordPTB");

                if (processes.Length == 0)
                {
                    processes = Process.GetProcessesByName("DiscordCanary");
                }
            }

            if (DebugMode)
            {
                Log($"Found a total of {processes.Length} processes");
            }
            return processes.Length != 0;
        }
    }
}
#endif