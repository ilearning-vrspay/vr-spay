using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// https://gist.github.com/pferreirafabricio/57eea7a719a2c34ca8b30dacacdd8eb1
public static class Request
{
    public static IEnumerator post(string routeName, string data, Action<Response> handleResponse)
    {
        Debug.Log($"http://localhost:5000/{routeName}");
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:5000/{routeName}", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler?.text);
            Response apiReponse = JsonUtility.FromJson<Response>(request.downloadHandler?.text);

            handleResponse(apiReponse);
        }
    }

    public static IEnumerator get(string routeName, Action<Response> handleResponse)
    {

        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:5000/{handleRouteName(routeName)}", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Response apiReponse = JsonUtility.FromJson<Response>(request.downloadHandler?.text);

            handleResponse(apiReponse);
        }
    }

    public static IEnumerator put(string routeName, string data, Action<Response> handleResponse = null)
    {
        using (UnityWebRequest request = new UnityWebRequest($"http://localhost:5000/{handleRouteName(routeName)}", "PUT"))
        {
            string jsonFields = JsonUtility.ToJson(data);

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonFields));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            Response apiReponse = JsonUtility.FromJson<Response>(request.downloadHandler?.text);

            if (handleResponse != null)
                handleResponse(apiReponse);
        }
    }

    private static string handleRouteName(string routeName)
    {
        if (routeName.StartsWith("/"))
            return routeName;

        return $"/{routeName}";
    }
}

[Serializable]
public class Response
{
    public string status;
    public string message;
}

[Serializable]
public class Data
{
    public string error;
    public string message;
}
