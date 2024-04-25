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
        public string pythonPath = null;

        public void Save() {
            Save(true);            
            // Set the environment variable for the API key
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", apiKey, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("PYTHON_PATH", pythonPath, EnvironmentVariableTarget.User);
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
            var pythonPath = settings.pythonPath;

            EditorGUI.BeginChangeCheck();

            key = EditorGUILayout.TextField("API Key", key);
            timeout = EditorGUILayout.IntField("Timeout", timeout);
            pythonPath = EditorGUILayout.TextField("Python Path", pythonPath);

            if (EditorGUI.EndChangeCheck())
            {
                settings.apiKey = key;
                settings.timeout = timeout;
                settings.pythonPath = pythonPath;
                settings.Save();
            }

            if (GUILayout.Button("Verify Python Path"))
            {
                // Attempt to run a simple Python command to verify the path
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.FileName = settings.pythonPath;
                psi.Arguments = "--version";  // Simple command to get the Python version
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                var process = System.Diagnostics.Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                    EditorUtility.DisplayDialog("Verification", "Python path is valid: " + output, "OK");
                else
                    EditorUtility.DisplayDialog("Verification", "Invalid Python path.", "OK");
            }

        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        => new AISettingsProvider();
    }

} // namespace AICommand
