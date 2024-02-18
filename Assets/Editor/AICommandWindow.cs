using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AICommand {

public sealed class AICommandWindow : EditorWindow
{
    #region Temporary script file operations

    const string TempFilePath = "Assets/AICommandTemp.cs";

    bool TempFileExists => System.IO.File.Exists(TempFilePath);

    void CreateScriptAsset(string code)
    {
        // UnityEditor internal method: ProjectWindowUtil.CreateScriptAssetWithContent
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
        try
        {
          method.Invoke(null, new object[]{TempFilePath, code});
        }
        catch (System.Exception e)
        {
          Debug.LogWarning(e);
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

    void RunGenerator()
    {
        var code = OpenAIUtil.InvokeChat(SystemContext(), WrapPrompt(_prompt),
        "AI Command", "Generating");
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


        Debug.Log("AI command script:" + code);
        CreateScriptAsset(code);
    }

    #endregion

    #region Editor GUI

    string _prompt = "Create 100 cubes at random points.";

    const string ApiKeyErrorText =
      "API Key hasn't been set. Please check the project settings " +
      "(Edit > Project Settings > AI Command > API Key).";

    bool IsApiKeyOk
      => !string.IsNullOrEmpty(AISettings.instance.apiKey);

    [MenuItem("Window/AI Command")]
    static void Init() => GetWindow<AICommandWindow>(true, "AI Command");

    void OnGUI()
    {
        if (IsApiKeyOk)
        {
            GUIStyle myTextAreaStyle3 = new GUIStyle(EditorStyles.textArea);
            myTextAreaStyle3.wordWrap = true;
            myTextAreaStyle3.stretchHeight = true;
            myTextAreaStyle3.stretchWidth = true;
            myTextAreaStyle3.fixedHeight = 200;
            myTextAreaStyle3.fontSize = 18;
            _prompt = EditorGUILayout.TextArea(_prompt,myTextAreaStyle3);
            if (GUILayout.Button("Run")) RunGenerator();
        }
        else
        {
            EditorGUILayout.HelpBox(ApiKeyErrorText, MessageType.Error);
        }
    }

    #endregion

    #region Script lifecycle

    void OnEnable()
      => AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

    void OnDisable()
      => AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

    void OnAfterAssemblyReload()
    {
        if (!TempFileExists) return;
        try{
          if (EditorApplication.ExecuteMenuItem("Edit/Do Task")) {
          Debug.Log("Menu item executed successfully.");
          } else {
          Debug.LogWarning("Menu item execution failed.");
          }
          //AssetDatabase.DeleteAsset(TempFilePath);
        } catch (System.Exception ex) {
          Debug.LogError("Error executing menu item: " + ex.Message);
        }
    }

    #endregion
}

} // namespace AICommand
