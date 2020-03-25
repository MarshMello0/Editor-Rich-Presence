#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using UnityEditor.SceneManagement;
using UnityEditor;
[InitializeOnLoad]
public class UERP : MonoBehaviour
{
    private static string applicationId = "509465267630374935";
    private static string prefix = "<b>UERP</b>";
    private static Discord.Discord discord;

    //This gets called once everytime it recompiles / loads up
    private UERP()
    {
        Init();
    }
    [MenuItem("UERP/Init")]
    private static void Init()
    {
        Log("Starting up...");
        EditorApplication.update += Update;
        discord = new Discord.Discord(Int64.Parse(applicationId), (UInt64)Discord.CreateFlags.Default);
        UpdateActivity();
        Log("Started!");
    }
    private static void Update()
    {
        if (discord != null)
            discord.RunCallbacks();
        
    }
    private static void UpdateActivity()
    {
        var activityManager = discord.GetActivityManager();
        var lobbyManager = discord.GetLobbyManager();

        var activity = new Discord.Activity
        {
            State = "olleh",
            Details = "foo details",
            Timestamps =
            {
                Start = 5,
                End = 6,
            },
            Assets =
            {
                LargeImage = "foo largeImageKey",
                LargeText = "foo largeImageText",
                SmallImage = "foo smallImageKey",
                SmallText = "foo smallImageText",
            },
        };

        activityManager.UpdateActivity(activity, result =>
        {
            Log("Update Activity {0}" + result.ToString());
        });
    }
    private static void Log(object message)
    {
        Debug.Log(prefix + ": " + message);
    }
    private static void LogWarning(object message)
    {
        Debug.LogWarning(prefix + ": " + message);
    }
    private static void LogError(object message)
    {
        Debug.LogError(prefix + ": " + message);
    }

}
#endif