#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace ERP
{
    [Serializable]
    public class ERPSettings
    {
        private static string path = Directory.GetCurrentDirectory() + "/.erp";
        public bool showSceneName;
        public bool showProjectName;
        public bool resetOnSceneChange;
        public bool debugMode;
        public bool EditorClosed;
        public long LastTimestamp;
        public long LastSessionID;
        public bool Errored;

        public ERPSettings() { }

        public ERPSettings(bool showSceneName, bool showProjectName, bool resetOnSceneChange, bool debugMode, bool editorClosed, long lastTimestamp, long lastSessionID, bool errored)
        {
            this.showSceneName = showSceneName;
            this.showProjectName = showProjectName;
            this.resetOnSceneChange = resetOnSceneChange;
            this.debugMode = debugMode;
            EditorClosed = editorClosed;
            LastTimestamp = lastTimestamp;
            LastSessionID = lastSessionID;
            Errored = errored;
        }

        public static void GetSettings()
        {
            if (File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ERPSettings));
                FileStream stream = new FileStream(path, FileMode.Open);
                ERPSettings settings = serializer.Deserialize(stream) as ERPSettings;
                ApplySettings(settings);
                stream.Close();
            }
        }

        private static void ApplySettings(ERPSettings settings)
        {
            ERP.showSceneName = settings.showSceneName;
            ERP.showProjectName = settings.showProjectName;
            ERP.resetOnSceneChange = settings.resetOnSceneChange;
            ERP.debugMode = settings.debugMode;
            ERP.EditorClosed = settings.EditorClosed;
            ERP.lastTimestamp = settings.LastTimestamp;
            ERP.lastSessionID = settings.LastSessionID;
            ERP.Errored = settings.Errored;
            ERP.Log("Applied Settings from file");
        }

        public static void SaveSettings()
        {
            ERPSettings settings = new ERPSettings(ERP.showSceneName, ERP.showProjectName, ERP.resetOnSceneChange, ERP.debugMode, ERP.EditorClosed, ERP.lastTimestamp, ERP.lastSessionID, ERP.Errored);

            XmlSerializer serializer = new XmlSerializer(typeof(ERPSettings));
            var stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, settings);
            stream.Close();
            ERP.Log("Saved Settings");
        }
    }

}
#endif