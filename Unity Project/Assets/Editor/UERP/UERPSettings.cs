using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace UERP
{
    [Serializable]
    public class UERPSettings
    {
        private static string path = Directory.GetCurrentDirectory() + "/.uerp";
        public bool showSceneName;
        public bool showProjectName;
        public bool resetOnSceneChange;
        public bool debugMode;

        public UERPSettings(){}

        public UERPSettings(bool showSceneName, bool showProjectName, bool resetOnSceneChange, bool debugMode)
        {
            this.showSceneName = showSceneName;
            this.showProjectName = showProjectName;
            this.resetOnSceneChange = resetOnSceneChange;
            this.debugMode = debugMode;
        }

        public static void GetSettings()
        {
            if (File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UERPSettings));
                FileStream stream = new FileStream(path, FileMode.Open);
                UERPSettings settings = serializer.Deserialize(stream) as UERPSettings;
                ApplySettings(settings);
                stream.Close();
            }
        }

        private static void ApplySettings(UERPSettings settings)
        {
            UERP.showSceneName = settings.showSceneName;
            UERP.showProjectName = settings.showProjectName;
            UERP.resetOnSceneChange = settings.resetOnSceneChange;
            UERP.debugMode = settings.debugMode;
            if (UERP.debugMode)
                UERP.Log("Applyed Settings from file");
        }

        public static void SaveSettings()
        {
            UERPSettings settings = new UERPSettings(UERP.showSceneName, UERP.showProjectName, UERP.resetOnSceneChange, UERP.debugMode);

            XmlSerializer serializer = new XmlSerializer(typeof(UERPSettings));
            var stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, settings);
            stream.Close();
            if (UERP.debugMode)
                UERP.Log("Saved Settings");
        }
    }

}
