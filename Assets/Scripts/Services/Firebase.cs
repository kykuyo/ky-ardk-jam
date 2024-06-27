using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Firebase : MonoBehaviour
{
    private readonly static string baseUrl = "https://ky-jam-default-rtdb.firebaseio.com";

    public static async Task<T> GetDataAsync<T>(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            string jsonResponse = await SendRequestAsync(
                HttpMethod.Get,
                $"{baseUrl + path}.json",
                null,
                cancellationToken
            );
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (TaskCanceledException)
        {
            return default(T);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting data: {ex.Message}");
            throw;
        }
    }

    public static async Task<T> PatchDataAsync<T>(
        string path,
        string json,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            string jsonResponse = await SendRequestAsync(
                new HttpMethod("PATCH"),
                $"{baseUrl + path}.json",
                json,
                cancellationToken
            );
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (TaskCanceledException)
        {
            return default(T);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating data: {ex.Message}");
            throw;
        }
    }

    private static async Task<string> SendRequestAsync(
        HttpMethod method,
        string url,
        string json = null,
        CancellationToken cancellationToken = default
    )
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(method, url);

        if (json != null)
        {
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
