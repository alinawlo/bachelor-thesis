using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace AICommand {

    static class OpenAIShapesGenerator
    {
        static string CreateChatRequestBody(string sysContext, string prompt)
        {
            var sysMsg = new OpenAI.RequestMessage();
            sysMsg.role = "system";
            sysMsg.content = "Act as a Unity developer and write a Unity Editor script.";

            var msg1 = new OpenAI.RequestMessage();
            msg1.role = "user";
            msg1.content = "Add a menu item called \"Do Task\" under the \"Edit\" menu.\n";
            
            var msg1_2 = new OpenAI.RequestMessage();
            msg1_2.role = "user";
            msg1_2.content = "Use the UnityEditor.MenuItem attribute for creating the menu item.\n";

            var msg1_3 = new OpenAI.RequestMessage();
            msg1_3.role = "user";
            msg1_3.content = "The Script class extends EditorWindow and import UnityEditor.\n";

            var msg2 = new OpenAI.RequestMessage();
            msg2.role = "user";
            msg2.content = "When the \"Do Task\" menu item is clicked, it should immediately perform the specified task without opening any editor window.\n";

            var msg3 = new OpenAI.RequestMessage();
            msg3.role = "user";
            msg3.content = "Avoid using the GameObject.FindGameObjectsWithTag method in your script.\n";
            
            var msg3_2 = new OpenAI.RequestMessage();
            msg3_2.role = "user";
            msg3_2.content = "Instead of using a selected object, find game objects manually within the script using GameObject.Instantiate.\n";

            var msg3_3 = new OpenAI.RequestMessage();
            msg3_3.role = "user";
            msg3_3.content = "Do not use prefab game objects. Always create new game objects.\n";

            var msg3_4 = new OpenAI.RequestMessage();
            msg3_4.role = "user";
            msg3_4.content = "To color an object, do not create a new Material instance. Just change the value of the sharedMaterial's color of the object's MeshRenderer component\n";
                                                                                            
            var msg4 = new OpenAI.RequestMessage();
            msg4.role = "user";
            msg4.content = "Provide only the script body, with no explanations.\n";

            var msg4_2 = new OpenAI.RequestMessage();
            msg4_2.role = "user";
            msg4_2.content = "The class name should be UnityEditorScript and the method name should be RunTask.\n";

            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = "The task is described as follows:\n" + prompt;

            var req = new OpenAI.Request();
            req.model = "gpt-4;
            req.temperature = 0.7f;
            req.max_tokens = 1000;
            req.messages = new [] { sysMsg, msg1, msg1_2, msg1_3, msg2, msg3, msg3_2, msg3_3, msg3_4, msg4, msg4_2, msg };

            return JsonUtility.ToJson(req);
        }

        public static string InvokeChat(string sysContext, string prompt, string progressTitle, string progressPrefix)
        {
            var settings = AISettings.instance;

            // POST
            using var post = UnityWebRequest.Post
            (OpenAI.Api.Url, CreateChatRequestBody(sysContext, prompt), "application/json");

            // Request timeout setting
            post.timeout = settings.timeout;

            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            // Request start
            EditorUtility.DisplayProgressBar(progressTitle, progressPrefix+": Send request to OpenAI...", 0.01f);
            var req = post.SendWebRequest();
            System.Threading.Thread.Sleep(1000);

            // Progress bar (Totally fake! Don't try this at home.)
            for (var progress = 0.02f; !req.isDone;)
            {
                EditorUtility.DisplayProgressBar(progressTitle, progressPrefix+"...", progress);
                System.Threading.Thread.Sleep(100);
                progress += 0.005f;
            }
            EditorUtility.ClearProgressBar();
            
            // Response extraction
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<OpenAI.Response>(json);
            
            return data.choices[0].message.content;
        }
    }

} // namespace AICommand
