using UnityEngine;
using UnityEditor;
using System;

namespace AICommand {

    [FilePath("UserSettings/AISettings.asset",
            FilePathAttribute.Location.ProjectFolder)]
    public sealed class AISettings : ScriptableSingleton<AISettings>
    {
        public string apiKey = null;
        public int timeout = 0;
        public void Save() {
            Save(true);            // Set the environment variable for the API key
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", apiKey, EnvironmentVariableTarget.User);
        }
        void OnDisable() => Save();
    }

    sealed class AISettingsProvider : SettingsProvider
    {
        public AISettingsProvider()
        : base("Project/Scene Generator (AI)", SettingsScope.Project) {}

        public override void OnGUI(string search)
        {
            var settings = AISettings.instance;

            var key = settings.apiKey;
            var timeout = settings.timeout;

            EditorGUI.BeginChangeCheck();

            key = EditorGUILayout.TextField("API Key", key);
            timeout = EditorGUILayout.IntField("Timeout", timeout);

            if (EditorGUI.EndChangeCheck())
            {
                settings.apiKey = key;
                settings.timeout = timeout;
                settings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        => new AISettingsProvider();
    }

} // namespace AICommand
