using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine; // Ensure this namespace is included

namespace AICommand {

    static class OpenAIUtil
    {
        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request
            {
                model = "gpt-3.5-turbo",
                messages = new[] { msg },
                max_tokens = 300 // Example: setting max_tokens to 50
            };

            return JsonUtility.ToJson(req);
        }

        public static string InvokeChat(string prompt)
        {
            var settings = AISettings.instance;

            // POST
            using var post = UnityWebRequest.Post
            (OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");

            // Request timeout setting
            post.timeout = settings.timeout;

            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            // Request start
            var req = post.SendWebRequest();

            // Progress bar (Totally fake! Don't try this at home.)
            for (var progress = 0.0f; !req.isDone; progress += 0.01f)
            {
                EditorUtility.DisplayProgressBar
                ("AI Command", "Generating...", progress);
                System.Threading.Thread.Sleep(100);
                progress += 0.01f;
            }
            EditorUtility.ClearProgressBar();

            // Response extraction
            var json = post.downloadHandler.text;
            UnityEngine.Debug.Log("JSON: " + json);
            var data = JsonUtility.FromJson<OpenAI.Response>(json);
            return data.choices[0].message.content;
        }
    }

} // namespace AICommand
