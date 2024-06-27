using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Niantic.Lightship.AR.VpsCoverage;
using Sliders;
using UnityEngine;

public class AreaRanking : MonoBehaviour
{
    [SerializeField]
    private CoverageClientManager _coverageClientManager;

    [SerializeField]
    private int _initialDelay = 3;

    [SerializeField]
    private int _updateInterval = 60;

    [Header("Batch Processing")]
    [SerializeField]
    private int _batchSize = 2;

    [SerializeField]
    private int _getDataDelayInterval = 500;

    [SerializeField]
    private PieChart _pieChart;

    private int[] _teamScores = new int[3];

    private CancellationTokenSource _cts;

    private async void Start()
    {
        _cts = new CancellationTokenSource();
        try
        {
            await Task.Delay(_initialDelay * 1000, _cts.Token);
            LoadAndApplyCachedVpsData();
            while (!_cts.Token.IsCancellationRequested)
            {
                await UpdateVpsRanking();
                await Task.Delay(_updateInterval * 1000, _cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Task was canceled.");
        }
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateVpsRanking));
        _cts.Cancel();
    }

    private void LoadAndApplyCachedVpsData()
    {
        Dictionary<string, MapVps> vpsComponentMap = GetVpsMapComponents();
        _teamScores = new int[3];

        foreach (var vpsId in vpsComponentMap.Keys)
        {
            VpsStatus cachedVpsStatus = LoadVpsDataFromPrefs(vpsId);
            if (cachedVpsStatus != null)
            {
                if (vpsComponentMap.TryGetValue(vpsId, out MapVps mapVps))
                {
                    _teamScores[0] += cachedVpsStatus.team_0_score;
                    _teamScores[1] += cachedVpsStatus.team_1_score;
                    _teamScores[2] += cachedVpsStatus.team_2_score;
                }
            }
        }

        UpdateChart();
    }

    private Dictionary<string, MapVps> GetVpsMapComponents()
    {
        MapVps[] vpsComponents = FindObjectsOfType<MapVps>();
        Dictionary<string, MapVps> vpsComponentMap = new();

        LatLng queryLocation =
            new(_coverageClientManager.QueryLatitude, _coverageClientManager.QueryLongitude);

        List<MapVps> sortedVpsComponents = vpsComponents.ToList();
        sortedVpsComponents.Sort(
            (a, b) =>
                a.areaTarget.Area.Centroid
                    .Distance(queryLocation)
                    .CompareTo(b.areaTarget.Area.Centroid.Distance(queryLocation))
        );

        foreach (var component in sortedVpsComponents)
        {
            AreaTarget data = component.areaTarget;
            string vpsId = !string.IsNullOrEmpty(data.Target.Identifier)
                ? data.Target.Identifier
                : Regex.Replace(data.Target.Name, "[^a-zA-Z0-9]", "");

            if (!vpsComponentMap.ContainsKey(vpsId))
            {
                vpsComponentMap.Add(vpsId, component);
            }
        }

        return vpsComponentMap;
    }

    public async Task UpdateVpsRanking()
    {
        _teamScores = new int[3];
        Dictionary<string, MapVps> vpsComponentMap = GetVpsMapComponents();
        string[] vpsIds = vpsComponentMap.Keys.ToArray();

        try
        {
            for (int i = 0; i < vpsIds.Length; i += _batchSize)
            {
                var batch = vpsIds.Skip(i).Take(_batchSize);

                List<Task<(string vpsId, VpsStatus vpsStatus)>> tasks = batch
                    .Select(vpsId => GetVpsData(vpsId, _cts.Token))
                    .ToList();

                (string vpsId, VpsStatus vpsStatus)[] results = await Task.WhenAll(tasks);

                foreach ((string vpsId, VpsStatus vpsStatus) in results)
                {
                    if (vpsStatus == null)
                    {
                        continue;
                    }

                    if (vpsComponentMap.TryGetValue(vpsId, out MapVps mapVps))
                    {
                        mapVps.UpdateStatus(vpsStatus);
                    }

                    _teamScores[0] += vpsStatus.team_0_score;
                    _teamScores[1] += vpsStatus.team_1_score;
                    _teamScores[2] += vpsStatus.team_2_score;

                    string data = JsonUtility.ToJson(vpsStatus);
                    Debug.Log($"Data for {vpsId}: {data}");
                }

                if (i + _batchSize < vpsIds.Length)
                {
                    await Task.Delay(_getDataDelayInterval, _cts.Token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("UpdateVpsRanking was canceled.");
        }
    }

    private async Task<(string vpsId, VpsStatus vpsStatus)> GetVpsData(
        string vpsId,
        CancellationToken ct
    )
    {
        VpsStatus vpsStatus = await Firebase.GetDataAsync<VpsStatus>($"/vps-status/{vpsId}", ct);

        if (vpsStatus != null)
        {
            SaveVpsDataToPrefs(vpsId, vpsStatus);
        }

        return (vpsId, vpsStatus);
    }

    private void SaveVpsDataToPrefs(string vpsId, VpsStatus vpsStatus)
    {
        string data = JsonUtility.ToJson(vpsStatus);
        PlayerPrefs.SetString($"VpsStatus_{vpsId}", data);
        PlayerPrefs.Save();
    }

    private VpsStatus LoadVpsDataFromPrefs(string vpsId)
    {
        string data = PlayerPrefs.GetString($"VpsStatus_{vpsId}", string.Empty);
        if (!string.IsNullOrEmpty(data))
        {
            return JsonUtility.FromJson<VpsStatus>(data);
        }
        return null;
    }

    public void UpdateChart()
    {
        _pieChart.SetValues(_teamScores.Select(score => (float)score).ToArray());
    }
}
