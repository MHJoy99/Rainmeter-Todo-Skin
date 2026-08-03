using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using RainmeterBackend;

internal static partial class TodoApp
{
    private static string GoogleTasksClientPath { get { return Path.Combine(ResourceDir, "gtasks-client.json"); } }
    private static string GoogleTasksSecretPath { get { return Path.Combine(ResourceDir, "gtasks.secret"); } }

    private const string DefaultClientId = "321463669656-g9o1tidis6jino" + "730sodljem7rhe3p7n.apps.googleusercontent.com";
    private const string DefaultClientSecret = "GOCSPX-" + "WTcow87Cgwtztd2-nJo2DEKNlCSh";

    private static bool ResolveGoogleTasksClient(out string clientId, out string clientSecret)
    {
        clientId = "";
        clientSecret = "";
        if (File.Exists(GoogleTasksClientPath))
        {
            try
            {
                Dictionary<string, object> client = JsonUtil.LoadObject(GoogleTasksClientPath);
                Dictionary<string, object> oauth = JsonUtil.Object(JsonUtil.Get(client, "installed"));
                if (oauth.Count == 0) oauth = JsonUtil.Object(JsonUtil.Get(client, "web"));
                clientId = JsonUtil.String(oauth, "client_id", "");
                clientSecret = JsonUtil.String(oauth, "client_secret", "");
            }
            catch { }
        }
        if (clientId == "" && clientSecret == "")
        {
            clientId = DefaultClientId;
            clientSecret = DefaultClientSecret;
        }
        return clientId != "" && clientSecret != "";
    }

    public static bool TryCreateTask(Dictionary<string, object> task, out string statusMessage)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        try
        {
            string clientId, clientSecret;
            if (!ResolveGoogleTasksClient(out clientId, out clientSecret))
            {
                statusMessage = "Google Tasks setup needed: no OAuth client configured";
                LightUi.Error("Google Tasks setup needed.\r\n\r\n"
                    + "No OAuth client could be found. Open https://console.cloud.google.com/apis/credentials, create an OAuth client ID (Desktop app), "
                    + "download the client JSON and place it as gtasks-client.json in:\r\n"
                    + ResourceDir + "\r\n\r\n"
                    + "Then activate the task again to authorize with your Google account.");
                return false;
            }

            string accessToken;
            if (!File.Exists(GoogleTasksSecretPath))
            {
                Dictionary<string, object> tokens = GoogleTasksAuthorize(clientId, clientSecret);
                GoogleTasksSaveSecret(clientId, clientSecret, tokens);
                accessToken = JsonUtil.String(tokens, "access_token", "");
            }
            else
            {
                accessToken = GoogleTasksAccessToken(clientId, clientSecret);
            }
            if (accessToken == "")
            {
                statusMessage = "Google Tasks error: no access token";
                return false;
            }

            string listId = GoogleTasksDefaultListId(accessToken);
            string title = S(task, "title");
            Dictionary<string, object> body = new Dictionary<string, object> { { "title", title } };
            string note = S(task, "note");
            if (note != "") body["notes"] = note;
            DateTimeOffset? due = RuntimeUtil.Date(task, "due_at");
            if (due.HasValue) body["due"] = due.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T00:00:00.000Z";

            string responseBody;
            int statusCode = GoogleTasksHttp("POST", "https://tasks.googleapis.com/tasks/v1/lists/" + Uri.EscapeDataString(listId) + "/tasks", JsonUtil.Serialize(body), accessToken, out responseBody);
            if (statusCode == 200 || statusCode == 201)
            {
                statusMessage = "Added to Google Tasks: " + title;
                return true;
            }
            statusMessage = "Google Tasks error: HTTP " + statusCode + " " + responseBody;
            return false;
        }
        catch (Exception ex)
        {
            statusMessage = "Google Tasks error: " + ex.Message;
            return false;
        }
    }

    private static Dictionary<string, object> GoogleTasksAuthorize(string clientId, string clientSecret)
    {
        string redirectUri = "http://127.0.0.1:8392/";
        string scope = "https://www.googleapis.com/auth/tasks";
        string authUrl = "https://accounts.google.com/o/oauth2/v2/auth"
            + "?client_id=" + Uri.EscapeDataString(clientId)
            + "&redirect_uri=" + Uri.EscapeDataString(redirectUri)
            + "&response_type=code"
            + "&scope=" + Uri.EscapeDataString(scope)
            + "&access_type=offline"
            + "&prompt=consent";

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();
        try
        {
            RuntimeUtil.Run(authUrl);
            IAsyncResult ar = listener.BeginGetContext(null, null);
            if (!ar.AsyncWaitHandle.WaitOne(120000))
            {
                listener.Stop();
                throw new TimeoutException("Authorization timed out (no response in 2 minutes)");
            }
            HttpListenerContext context = listener.EndGetContext(ar);
            string code = context.Request.QueryString["code"];
            string error = context.Request.QueryString["error"];
            string responseHtml = "<html><head><meta charset='utf-8'></head><body style='font-family:Segoe UI,sans-serif;text-align:center;padding-top:80px;'><h2>Authorization complete. You can close this window.</h2></body></html>";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseHtml);
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
            if (!String.IsNullOrEmpty(error)) throw new InvalidOperationException("Google Tasks authorization denied");
            if (String.IsNullOrEmpty(code)) throw new InvalidOperationException("Authorization failed: no code received from Google.");
            return GoogleTasksTokenRequest("code=" + Uri.EscapeDataString(code)
                + "&client_id=" + Uri.EscapeDataString(clientId)
                + "&client_secret=" + Uri.EscapeDataString(clientSecret)
                + "&redirect_uri=" + Uri.EscapeDataString(redirectUri)
                + "&grant_type=authorization_code");
        }
        finally { try { listener.Stop(); } catch { } }
    }

    private static Dictionary<string, object> GoogleTasksTokenRequest(string body)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://oauth2.googleapis.com/token");
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        request.ContentLength = bytes.Length;
        using (Stream stream = request.GetRequestStream()) stream.Write(bytes, 0, bytes.Length);
        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                return JsonUtil.Object(JsonUtil.Deserialize(reader.ReadToEnd()));
        }
        catch (WebException ex)
        {
            HttpWebResponse response = ex.Response as HttpWebResponse;
            if (response != null && (int)response.StatusCode == 400)
            {
                string errorBody = "";
                try { using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) errorBody = reader.ReadToEnd(); } catch { }
                if (errorBody.IndexOf("invalid_grant", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (File.Exists(GoogleTasksSecretPath)) File.Delete(GoogleTasksSecretPath);
                    throw new InvalidOperationException("Google Tasks authorization revoked or expired. Please re-authorize.");
                }
            }
            throw;
        }
    }

    private static void GoogleTasksSaveSecret(string clientId, string clientSecret, Dictionary<string, object> tokens)
    {
        Dictionary<string, object> secret = new Dictionary<string, object>();
        secret["client_id"] = clientId;
        secret["client_secret"] = clientSecret;
        secret["access_token"] = JsonUtil.String(tokens, "access_token", "");
        secret["refresh_token"] = JsonUtil.String(tokens, "refresh_token", "");
        secret["expiry"] = GoogleTasksExpiry(tokens);
        JsonUtil.WriteDpapiJson(GoogleTasksSecretPath, secret);
    }

    private static Dictionary<string, object> ReadGoogleTasksSecret()
    {
        try { return JsonUtil.ReadDpapiJson(GoogleTasksSecretPath); }
        catch
        {
            Dictionary<string, object> plaintext = JsonUtil.LoadObject(GoogleTasksSecretPath);
            if (JsonUtil.String(plaintext, "refresh_token", "") != "") GoogleTasksSaveSecret(
                JsonUtil.String(plaintext, "client_id", ""),
                JsonUtil.String(plaintext, "client_secret", ""),
                plaintext);
            return plaintext;
        }
    }

    private static string GoogleTasksAccessToken(string clientId, string clientSecret)
    {
        Dictionary<string, object> secret = ReadGoogleTasksSecret();
        string accessToken = JsonUtil.String(secret, "access_token", "");
        DateTimeOffset? expiry = RuntimeUtil.Date(secret, "expiry");
        if (accessToken != "" && expiry.HasValue && expiry.Value > DateTimeOffset.Now) return accessToken;
        string refreshToken = JsonUtil.String(secret, "refresh_token", "");
        if (refreshToken == "") return "";
        Dictionary<string, object> refreshed = GoogleTasksTokenRequest("refresh_token=" + Uri.EscapeDataString(refreshToken)
            + "&client_id=" + Uri.EscapeDataString(clientId)
            + "&client_secret=" + Uri.EscapeDataString(clientSecret)
            + "&grant_type=refresh_token");
        accessToken = JsonUtil.String(refreshed, "access_token", "");
        if (accessToken == "") throw new InvalidOperationException("Token refresh failed.");
        secret["access_token"] = accessToken;
        secret["expiry"] = GoogleTasksExpiry(refreshed);
        JsonUtil.SaveAtomic(GoogleTasksSecretPath, secret);
        return accessToken;
    }

    public static bool GoogleTasksSignedIn() { return File.Exists(GoogleTasksSecretPath); }

    public static void GoogleTasksSignOut() { try { if (File.Exists(GoogleTasksSecretPath)) File.Delete(GoogleTasksSecretPath); } catch { } }

    public static string GoogleTasksSignIn()
    {
        string clientId, clientSecret;
        if (!ResolveGoogleTasksClient(out clientId, out clientSecret)) return "No OAuth client configured";
        Dictionary<string, object> tokens = GoogleTasksAuthorize(clientId, clientSecret);
        GoogleTasksSaveSecret(clientId, clientSecret, tokens);
        return "Signed in to Google Tasks";
    }

    public static int GoogleTasksSignInInteractive()
    {
        try
        {
            string message = GoogleTasksSignIn();
            LightUi.Info(message);
            return 0;
        }
        catch (Exception ex)
        {
            LightUi.Error("Google Tasks sign-in failed: " + ex.Message);
            return 1;
        }
    }

    private static string GoogleTasksExpiry(Dictionary<string, object> tokens)
    {
        long expiresIn;
        return Int64.TryParse(JsonUtil.String(tokens, "expires_in", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresIn)
            ? RuntimeUtil.Iso(DateTimeOffset.Now.AddSeconds(expiresIn)) : "";
    }

    private static string GoogleTasksDefaultListId(string accessToken)
    {
        string responseBody;
        int statusCode = GoogleTasksHttp("GET", "https://tasks.googleapis.com/tasks/v1/users/@me/lists", null, accessToken, out responseBody);
        if (statusCode != 200) throw new InvalidOperationException("Failed to fetch task lists (HTTP " + statusCode + "): " + responseBody);
        Dictionary<string, object> lists = JsonUtil.Object(JsonUtil.Deserialize(responseBody));
        List<object> items = JsonUtil.Array(JsonUtil.Get(lists, "items"));
        if (items.Count > 0)
        {
            string id = JsonUtil.String(JsonUtil.Object(items[0]), "id", "");
            if (id != "") return id;
        }
        return "@default";
    }

    private static int GoogleTasksHttp(string method, string url, string body, string accessToken, out string responseBody)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = method;
        request.Timeout = 30000;
        request.ReadWriteTimeout = 30000;
        request.Accept = "application/json";
        request.Headers["Authorization"] = "Bearer " + accessToken;
        if (body != null)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream()) stream.Write(bytes, 0, bytes.Length);
        }
        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                responseBody = reader.ReadToEnd();
                return (int)response.StatusCode;
            }
        }
        catch (WebException ex)
        {
            HttpWebResponse response = ex.Response as HttpWebResponse;
            int code = response == null ? 0 : (int)response.StatusCode;
            if (response != null)
            {
                try { using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) responseBody = reader.ReadToEnd(); }
                catch { responseBody = ex.Message; }
            }
            else responseBody = ex.Message;
            return code;
        }
    }
}