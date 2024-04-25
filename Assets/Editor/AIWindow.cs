using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;


namespace AICommand {

    public sealed class AIStoryWindow : EditorWindow
    {
        #region Temporary script file operations

        const string TempFilePath = "Assets/Editor/AIStoryStep.cs";
        private bool TempFileExists => System.IO.File.Exists(TempFilePath+"edit.cs");
        const string TempFilePathEnd = "Temp.cs";
        const string directoryPath = "Assets/Prefabs";


        void CreateStoryAsset(string storyStepFileNr, string storyStepCode)
        {
            // UnityEditor internal method: ProjectWindowUtil.CreateScriptAssetWithContent
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
            try
            {
                method.Invoke(null, new object[]{TempFilePath+storyStepFileNr+TempFilePathEnd, storyStepCode});
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e);
            }
        }

        #endregion

        #region Script generator

        static string WrapPrompt(string input)
        => "The task is described as follows:\n" + input;
        
        static string SystemContext()
        => "Write a Unity Editor script. And follow these instructions:\n" +
            " - It provides its functionality as a menu item placed \"Edit\" > \"Do Task\".\n" +
            " - It does not provide any editor window. It immediately does the task when the menu item is invoked.\n" +
            " - Do not use GameObject.FindGameObjectsWithTag.\n" +
            " - Do not use Shader.Find(\"Standard\").\n" +
            " - Use renderer.sharedMaterial instead of renderer.material when creating objects.\n" +
            " - There is no selected object. Find game objects manually.\n" +
            " - I only need the script body. Do not add any explanation.\n" +
            " - Class name and method name should have different names.\n" +
            " - Use UnityEditor.MenuItem.\n";

        static string WrapToEdit(string input)
        => "Write a Unity Editor script.\n" +
            " - It provides its functionality as a menu item placed \"Edit\" > \"Do Task\".\n" +
            " - It doesn’t provide any editor window. It immediately does the task when the menu item is invoked.\n" +
            " - Do not use GameObject.FindGameObjectsWithTag(), use GameObject.FindObjectsOfType() instead.\n" +
            " - Use renderer.sharedMaterial instead of renderer.material when creating objects.\n" +
            " - Use MeshRenderer instead of Renderer when coloring objects.\n" +
            " - There is no selected object. Find game objects manually.\n" +
            " - I only need the script body. Don’t add any explanation.\n" +
            " - Use UnityEditor.\n" +
            "The task is described as follows:\n" + input;


        private string[] myArray;

        void RunGenerator(string prompt, int i, int maxRun, bool shapes)
        {
            UnityEngine.Debug.Log(prompt);
            
            var code = OpenAIScriptGenerator.InvokeChat(SystemContext(), WrapPrompt(prompt),
                "Scene Generator", "Generate Step "+i+"/"+maxRun);  
            
            if(shapes){
                code = OpenAIShapesGenerator.InvokeChat(SystemContext(), WrapPrompt(prompt),
                "Scene Generator", "Generate Step "+i+"/"+maxRun);
            }

            // Define the regular expression pattern
            string pattern = @"```csharp\n(.*?)\n```|(?:```)(.*?)(?:```)";
            // Match the pattern against the input string
            MatchCollection matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

            // Loop through the matches and extract the code
            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    // Code is inside ```csharp``` tag
                    code = match.Groups[1].Value;
                }
                else if (match.Groups[2].Success)
                {
                    // Code is inside ``` tag
                    code = match.Groups[2].Value;
                }
            }
            code = code.Replace("Edit/Do Task", "Edit/Story/Step"+i);
            code = code.Replace("UnityEditorScript", "UnityEditorScript"+i);
            code = code.Replace("C#", "");
            code = code.Replace("c#", "");

            UnityEngine.Debug.Log("AI command script:" + code);
            CreateStoryAsset(""+i, "/*Prompt:\n"+prompt+"*/\n\n"+code);  
        }

        void RunGeneratorEdit(string prompt)
        {   
            UnityEngine.Debug.Log("AI command script:" + prompt);
            var code = OpenAIUtil.InvokeChat(WrapToEdit(prompt));
            // Define the regular expression pattern
            string pattern = @"```csharp\n(.*?)\n```|(?:```)(.*?)(?:```)";
            // Match the pattern against the input string
            MatchCollection matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

            EditorGUILayout.HelpBox(pattern, MessageType.Error);
            // Loop through the matches and extract the code
            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    // Code is inside ```csharp``` tag
                    code = match.Groups[1].Value;
                    EditorGUILayout.HelpBox(code, MessageType.Error);
                }
                else if (match.Groups[2].Success)
                {
                    // Code is inside ``` tag
                    code = match.Groups[2].Value;
                    EditorGUILayout.HelpBox("Code is inside: " + code, MessageType.Error);
                }
            }
            code = code.Replace("C#", "");
            code = code.Replace("c#", "");

            //UnityEngine.Debug.Log("AI command script:" + code);
            CreateScriptAsset(code);
        }

        void CreateScriptAsset(string code)
        {
            // UnityEditor internal method: ProjectWindowUtil.CreateScriptAssetWithContent
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
            method.Invoke(null, new object[]{TempFilePath+"edit.cs", code});
        }

        #endregion

        #region Editor GUI

        static string _story ="";
        private string _prompt = "";

        const string ApiKeyErrorText =
        "API Key hasn't been set. Please check the project settings " +
        "(Edit > Project Settings > Scene Generator (AI) > API Key).";

        bool IsApiKeyOk
        => !string.IsNullOrEmpty(AISettings.instance.apiKey);

        [MenuItem("Window/Scene Generator (AI)")]
        static void Init() => GetWindow<AIStoryWindow>(true, "Scene Generator (AI)");

        private int _selectedModeIndex = 0;
        private readonly string[] modes = new string[] { "New Scene", "New Object", "Edit Object" };

        private bool shapes = false;

        void OnGUI()
        {
            if (IsApiKeyOk)
            {
                EditorGUILayout.LabelField("Conifgurations:", EditorStyles.boldLabel);
                _selectedModeIndex = EditorGUILayout.Popup("Mode:", _selectedModeIndex, modes);

                if (_selectedModeIndex != 2){
                    shapes = EditorGUILayout.Toggle("3D Shapes:", shapes);
                    if(!shapes){
                        EditorGUILayout.LabelField("(using prefabs)");
                    }
                    else{            
                        EditorGUILayout.LabelField("");
                    }
                }
                else
                {
                    // Ensure shapes is set to false when in 'Edit Object' mode
                    shapes = false;
                }

                EditorGUILayout.LabelField("Enter Your Prompt:", EditorStyles.boldLabel);

                GUIStyle myTextAreaStyle2 = new GUIStyle(EditorStyles.textArea);
                myTextAreaStyle2.wordWrap = true;
                myTextAreaStyle2.stretchHeight = true;
                myTextAreaStyle2.stretchWidth = true;
                myTextAreaStyle2.fixedHeight = 250;
                myTextAreaStyle2.fontSize = 18;
                _story = EditorGUILayout.TextArea(_story,myTextAreaStyle2);

                EditorGUILayout.LabelField("Running Step:", EditorStyles.boldLabel);

                GUIStyle myTextAreaStyle3 = new GUIStyle(EditorStyles.textArea);
                myTextAreaStyle3.wordWrap = true;
                myTextAreaStyle3.stretchHeight = true;
                myTextAreaStyle3.stretchWidth = true;
                myTextAreaStyle3.fixedHeight = 425;
                myTextAreaStyle3.fontSize = 18;

                // Disable GUI to make the text area read-only
                GUI.enabled = false;
                _prompt = EditorGUILayout.TextArea(_prompt, myTextAreaStyle3);
                GUI.enabled = true;


                //GUI.enabled = true;
                if (GUILayout.Button("Run")) {
                    EditorCoroutineUtility.StartCoroutine(ClearTempFiles(),this);

                    string[] fileNames = GetFileNames(directoryPath);
                    //SaveJsonToFile(_story);
                    
                    if(shapes){
                        if(_selectedModeIndex==0){
                            EditorCoroutineUtility.StartCoroutine(GenerateSteps(shapes), this);
                            myArray = run_agent("Agents/OpenAI_LC_Shapes.py", "",_story, fileNames).Split("/*step*/");
                        }else if(_selectedModeIndex==1){
                            EditorCoroutineUtility.StartCoroutine(GenerateSteps(shapes), this);
                            myArray = new string [] {run_agent("Agents/OpenAI_LC_Shapes_Object.py", "",_story, fileNames)};
                        }
                    } else{
                        if(_selectedModeIndex==0){
                            EditorCoroutineUtility.StartCoroutine(GenerateSteps(shapes), this);
                            myArray = run_agent("Agents/OpenAI_LLM.py", "",_story, fileNames).Split("/*step*/");
                        }else if(_selectedModeIndex==1){
                            EditorCoroutineUtility.StartCoroutine(GenerateSteps(shapes), this);
                            myArray = new string [] {run_agent("Agents/OpenAI_LLM_Object.py", "",_story, fileNames)};
                        }
                    }
                        
                    if(_selectedModeIndex==2){                  
                        RunGeneratorEdit(_story);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(ApiKeyErrorText, MessageType.Error);
            }
        }

        public void doRepaint() {
            this.Repaint();
        }

        private System.Collections.IEnumerator GenerateSteps(bool shapes)
        {
            UnityEngine.Debug.Log("Coroutine started!");
            _prompt = "";
            doRepaint();
            // Progress bar, not accurate
            var progress = 0.1f;
            for (var i = 0; i < 3; i++)
            {
                EditorUtility.DisplayProgressBar("Scene Generator", "Generate steps based on story...", progress);
                System.Threading.Thread.Sleep(1000);
                progress += 0.3f;
            }
            EditorUtility.ClearProgressBar();

            yield return null;

            int maxRun = myArray.Length;

            for (int i = 1; i <= maxRun; i++)
            {
                string prompt = myArray[i-1];
                _prompt = prompt;
                doRepaint();
                yield return null;
                RunGenerator(prompt, i, maxRun, shapes);
                yield return null;
            }
            _prompt = "";
            doRepaint();
            yield return null;
            UnityEngine.Debug.Log("Coroutine ended!");
        }

        private System.Collections.IEnumerator ExecuteScripts()
        {
            int maxRun = myArray.Length;
            for (int i = 1; i <= maxRun; i++){
                var storyStepScriptFileName = TempFilePath+i+TempFilePathEnd;
                if (System.IO.File.Exists(storyStepScriptFileName)) {
                    try{
                        if (EditorApplication.ExecuteMenuItem("Edit/Story/Step"+i)) {
                        UnityEngine.Debug.Log("Menu item executed successfully.");
                        } else {
                        UnityEngine.Debug.LogWarning("Menu item execution failed.");
                        }
                        System.Threading.Thread.Sleep(1000);
                        //get filename from path
                        string fileName = Path.GetFileName(storyStepScriptFileName);
                        AssetDatabase.RenameAsset(storyStepScriptFileName, fileName+".bak");
                    } catch (System.Exception ex) {
                        UnityEngine.Debug.LogError("Error executing menu item: " + ex.Message);
                    }
                }
            }
            yield return new WaitForSeconds(10);
        }

        #endregion

        #region Script lifecycle

        void OnEnable()
        => AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

        void OnDisable()
        => AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

        void OnAfterAssemblyReload()
        {
            EditorCoroutineUtility.StartCoroutine(ExecuteScripts(), this);
            
            if(_selectedModeIndex==2){
                UnityEngine.Debug.Log("Temp File: " + TempFileExists);
                if (!TempFileExists) return;
                EditorApplication.ExecuteMenuItem("Edit/Do Task");
                AssetDatabase.DeleteAsset(TempFilePath+"edit.cs");
            }
        }
        

        #endregion

        private string run_agent(string agent, string args, string description, string[] fileNames) {
            string pythonPath = AISettings.instance.pythonPath;
            string[] filteredFiles = fileNames.Where(x => !x.Contains(".meta")).ToArray();
            string fileNamesString = String.Join(",", filteredFiles);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            if(shapes){
                start.Arguments = string.Format("{0} {1} \"{2}\"", agent, args, description);
            }else{
                start.Arguments = string.Format("{0} {1} \"{2}\" \"{3}\"", agent, args, description,fileNamesString);
            }
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using(Process process = Process.Start(start)) {
                using(StreamReader reader = process.StandardOutput) {
                string result = reader.ReadToEnd();
                return result;
                }
            }
        }

        private static string[] GetFileNames(string directoryPath) {
            try
            {
                return Directory.GetFiles(directoryPath);
            }
            catch (IOException e)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(e.ToString());
                return new string[0];
            }
        }
        
        System.Collections.IEnumerator ClearTempFiles() {
            string editorFolderPath = "Assets/Editor";
            
            if (Directory.Exists(editorFolderPath))
            {
                string[] files = Directory.GetFiles(editorFolderPath);
                
                foreach (string file in files)
                {
                if (Path.GetFileName(file).StartsWith("AIStoryStep") && !Path.GetFileName(file).EndsWith("edit.cs"))
                {
                    File.Delete(file);
                    UnityEngine.Debug.Log("Deleted file: " + file);
                }
                }
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("All files in 'Assets/Editor' have been deleted.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("'Assets/Editor' directory does not exist or was not found.");
            }
            yield return new WaitForSeconds(10);
        }

    }
} // namespace AICommand
