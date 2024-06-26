using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niantic.Lightship.AR.VpsCoverage;
using Sliders;
using UnityEngine;

public class AreaRanking : MonoBehaviour
{
    [SerializeField]
    private int _initialDelay = 3;

    [SerializeField]
    private int _updateInterval = 60;

    [SerializeField]
    private PieChart _pieChart;

    private int[] _teamScores = new int[3];

    private void Start()
    {
        InvokeRepeating(nameof(UpdateVpsRanking), _initialDelay, _updateInterval);
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateVpsRanking));
    }

    private Dictionary<string, MapVps> GetVpsMapComponents()
    {
        MapVps[] vpsComponents = FindObjectsOfType<MapVps>();
        Dictionary<string, MapVps> vpsComponentMap = new();

        foreach (var component in vpsComponents)
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

    [ContextMenu("Update VPS Ranking")]
    public async void UpdateVpsRanking()
    {
        _teamScores = new int[3];
        Dictionary<string, MapVps> vpsComponentMap = GetVpsMapComponents();
        string[] vpsIds = vpsComponentMap.Keys.ToArray();

        List<Task<(string vpsId, VpsStatus vpsStatus)>> tasks = vpsIds
            .Select(vpsId => GetVpsData(vpsId))
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
    }

    private async Task<(string vpsId, VpsStatus vpsStatus)> GetVpsData(string vpsId)
    {
        VpsStatus vpsStatus = await Firebase.GetDataAsync<VpsStatus>($"/vps-status/{vpsId}");
        return (vpsId, vpsStatus);
    }

    public void UpdateChart()
    {
        _pieChart.SetValues(_teamScores.Select(score => (float)score).ToArray());
    }
}
