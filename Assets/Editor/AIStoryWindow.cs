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

    const string TempFilePath = "Assets/Editor/AIStoryStep";
    const string TempFilePathEnd = "Temp.cs";

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

    public string[] myArray;
//      = new string[] { 
// //Step 1 "create project"
// _story
// //Step 7 add lights but skips
//     };

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

    void OnGUI()
    {
        if (IsApiKeyOk)
        {
            //GUI.enabled = false;

            GUIStyle myTextAreaStyle = new GUIStyle(EditorStyles.textArea);
            myTextAreaStyle.wordWrap = true;
            myTextAreaStyle.stretchHeight = true;
            myTextAreaStyle.stretchWidth = true;
            myTextAreaStyle.fixedHeight = 130;
            myTextAreaStyle.fontSize = 14;
            _statement = EditorGUILayout.TextArea(_statement, myTextAreaStyle);

            GUIStyle myTextAreaStyle2 = new GUIStyle(EditorStyles.textArea);
            myTextAreaStyle2.wordWrap = true;
            myTextAreaStyle2.stretchHeight = true;
            myTextAreaStyle2.stretchWidth = true;
            myTextAreaStyle2.fixedHeight = 250;
            myTextAreaStyle2.fontSize = 18;
            _story = EditorGUILayout.TextArea(_story,myTextAreaStyle2);

            GUIStyle myTextAreaStyle3 = new GUIStyle(EditorStyles.textArea);
            myTextAreaStyle3.wordWrap = true;
            myTextAreaStyle3.stretchHeight = true;
            myTextAreaStyle3.stretchWidth = true;
            myTextAreaStyle3.fixedHeight = 425;
            myTextAreaStyle3.fontSize = 18;
            _prompt = EditorGUILayout.TextArea(_prompt,myTextAreaStyle3);


            //GUI.enabled = true;
            if (GUILayout.Button("Run")) {
              EditorCoroutineUtility.StartCoroutine(DoSomething(), this);
              myArray =
                new string[] {
                  run_cmd("Agents/OpenAI_LLM.py", "",_story).Split("/*step*/");
                };
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
              //AssetDatabase.DeleteAsset(storyStepScriptFileName);
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
    }

    #endregion

    private string run_cmd(string cmd, string args, string description) {
            string pythonPath = "/Users/ali/opt/anaconda3/bin/python"; // Path to python3 executable on macOS

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.Arguments = string.Format("{0} {1} \"{2}\"", cmd, args, description);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using(Process process = Process.Start(start)) {
                using(StreamReader reader = process.StandardOutput) {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
    }
}

} // namespace AICommand
