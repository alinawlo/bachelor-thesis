using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using System.IO;
using System.Diagnostics;
using System;


namespace AICommand {

public sealed class AIStoryWindow : EditorWindow
{
    #region Temporary script file operations

    const string TempFilePath = "Assets/Editor/AIStoryStep.cs";
    bool TempFileExists => System.IO.File.Exists(TempFilePath);
    const string TempFilePathEnd = "Temp.cs";
    const string directoryPath = "Assets/SimpleNaturePack/Prefabs";


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
         " - There is no selected object. Find game objects manually.\n" +
         " - I only need the script body. Don’t add any explanation.\n" +
         " - Use UnityEditor.\n" +
         "The task is described as follows:\n" + input;


    public string[] myArray;

    void RunGenerator(string prompt, int i, int maxRun)
    {
            UnityEngine.Debug.Log(prompt);
            var code = OpenAIUtil.InvokeChat(SystemContext(), WrapPrompt(prompt),
            "AI Story", "Generate Step "+i+"/"+maxRun);
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
        var code = OpenAIUtil2.InvokeChat(WrapToEdit(prompt));
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
        method.Invoke(null, new object[]{TempFilePath, code});
    }

    #endregion

    #region Editor GUI

    string _statement = "As a AI-powered Software Developer,\nI want to demonstrate the power of AI in software development through a Unity demo, showcasing how machine learning can improve user experience,\nso that I can inspire and educate others on the potential of this technology.";
    static string _story ="";
    // = "As a Unity 3D developer, I want to create a simple start scene of a garden using cubes and spheres as 3D objects, with no specific color scheme. The garden should include grass, simple plants, simple trees, a pathway, and lanterns next to the pathway. The scene should be set during daytime. Please provide a step-by-step guide on how to create this simple garden scene with suggested overall size, dimensions of the garden elements, and arrangement of the elements. The response should be as technical and detailed as possible, without the need for Unity Editor Scripts. No specific functionality, interactivity, or navigation options are required.";
    private string _prompt = "";

    const string ApiKeyErrorText =
      "API Key hasn't been set. Please check the project settings " +
      "(Edit > Project Settings > AI Command > API Key).";

    bool IsApiKeyOk
      => !string.IsNullOrEmpty(AISettings.instance.apiKey);

    [MenuItem("Window/AI Story")]
    static void Init() => GetWindow<AIStoryWindow>(true, "AI Story");

    private int _selectedLlmIndex = 0;
    private readonly string[] llms = new string[] { "gpt-4", "llama-2", "Option 3" };
    private int _selectedModeIndex = 0;
    private readonly string[] modes = new string[] { "New Scene", "New Object", "Edit Object" };

    void OnGUI()
    {
        if (IsApiKeyOk)
        {
            //GUI.enabled = false;

            EditorGUILayout.LabelField("Select a Functionality:", EditorStyles.boldLabel);
            _selectedModeIndex = EditorGUILayout.Popup("Functionality:", _selectedModeIndex, modes);

            EditorGUILayout.LabelField("Select a Language Model:", EditorStyles.boldLabel);
            _selectedLlmIndex = EditorGUILayout.Popup("Model:", _selectedLlmIndex, llms);

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
            _prompt = EditorGUILayout.TextArea(_prompt,myTextAreaStyle3);


            //GUI.enabled = true;
            if (GUILayout.Button("Run")) {
              string[] fileNames = GetFileNames(directoryPath);

                if(_selectedModeIndex==0){
                  EditorCoroutineUtility.StartCoroutine(DoSomething(), this);
                  myArray = run_cmd("Agents/OpenAI_LLM.py", "",_story, fileNames).Split("/*step*/");
                }else if(_selectedModeIndex==1){
                  EditorCoroutineUtility.StartCoroutine(DoSomething(), this);
                  myArray = new string [] {run_cmd("Agents/OpenAI_LLM_Object.py", "",_story, fileNames)};
                }else if(_selectedModeIndex==2){
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

    private System.Collections.IEnumerator DoSomething()
    {
        UnityEngine.Debug.Log("Coroutine started!");
        _prompt = "";
        doRepaint();
        // Progress bar (Totally fake! Don't try this at home.)
        // pre processed due to missing access to GPT4 via API
        var progress = 0.1f;
        for (var i = 0; i < 3; i++)
        {
            EditorUtility.DisplayProgressBar("AI Story", "Generate steps based on story...", progress);
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
            RunGenerator(prompt, i, maxRun);
            yield return null;
        }
        _prompt = "";
        doRepaint();
        yield return null;
        UnityEngine.Debug.Log("Coroutine ended!");
    }

    private System.Collections.IEnumerator DoRender()
    {
        int maxRun = myArray.Length;
        for (int i = 1; i <= maxRun; i++)
        {
          var storyStepScriptFileName = TempFilePath+i+TempFilePathEnd;
          if (System.IO.File.Exists(storyStepScriptFileName)) {
            try{
              if (EditorApplication.ExecuteMenuItem("Edit/Story/Step"+i)) {
                UnityEngine.Debug.Log("Menu item executed successfully.");
              } else {
                UnityEngine.Debug.LogWarning("Menu item execution failed.");
              }
              System.Threading.Thread.Sleep(1000);
              AssetDatabase.DeleteAsset(storyStepScriptFileName);
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
      EditorCoroutineUtility.StartCoroutine(DoRender(), this);
      
      if(_selectedModeIndex==2){
        UnityEngine.Debug.Log("Temp File: " + TempFileExists);
        if (!TempFileExists) return;
          EditorApplication.ExecuteMenuItem("Edit/Do Task");
          AssetDatabase.DeleteAsset(TempFilePath);
      }
    }
    

    #endregion

    private string run_cmd(string cmd, string args, string description, string[] fileNames) {
            string pythonPath = "/Users/ali/opt/anaconda3/bin/python"; // Path to python3 executable on macOS

            string fileNamesString = String.Join(",", fileNames);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.Arguments = string.Format("{0} {1} \"{2}\" \"{3}\"", cmd, args, description,fileNamesString);
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
}

} // namespace AICommand
