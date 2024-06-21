using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum Endpoint
{
    ANCHORS,
    VPS
}

[Serializable]
public class VpsData
{
    public string vps_id;
    public string data;
    public string date;
}

public class GoogleSheetsManager : MonoBehaviour
{
    private readonly string baseUrl =
        "https://script.google.com/macros/s/AKfycbyJzex_Thv1o6qjwtY7kWOqYFgawPx0fHZQQfmUjWbiGr18gpZmFQ0CaQ6D36jG7I10KA/exec";

    [HideInInspector]
    public List<AnchorData> anchors = new();

    [HideInInspector]
    public List<VpsData> vpsList = new();

    public IEnumerator PostData<T>(Endpoint endpoint, T data)
    {
        var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
            JsonConvert.SerializeObject(data)
        );

        string endpointStr = endpoint.ToString().ToLower();

        dataDict.Add("endpoint", endpointStr);

        string jsonData = JsonConvert.SerializeObject(dataDict);
        Debug.Log(jsonData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest www =
            new(baseUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            Debug.Log("Response: " + www.downloadHandler.text);
        }
    }

    public IEnumerator GetDataByVpsId(Endpoint endpoint, string vpsId)
    {
        string endpointStr = endpoint.ToString().ToLower();

        string getUrl = $"{baseUrl}?endpoint={endpointStr}&vps_id={vpsId}";
        UnityWebRequest www = UnityWebRequest.Get(getUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
            AddDataFromJson(endpoint, www.downloadHandler.text);
        }
    }

    public void AddDataFromJson(Endpoint endpoint, string json)
    {
        switch (endpoint)
        {
            case Endpoint.ANCHORS:
                List<AnchorData> newAnchors = JsonConvert.DeserializeObject<List<AnchorData>>(json);
                anchors.AddRange(newAnchors);
                break;

            case Endpoint.VPS:
                List<VpsData> newVpsList = JsonConvert.DeserializeObject<List<VpsData>>(json);
                vpsList.AddRange(newVpsList);
                break;

            default:
                Debug.LogError($"Unknown endpoint: {endpoint}");
                break;
        }
    }
}
