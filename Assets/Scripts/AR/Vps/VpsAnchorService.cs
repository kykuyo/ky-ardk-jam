using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

[Serializable]
public class AnchorTeamData
{
    public string position;
    public string rotation;
}

[Serializable]
public class AnchorData
{
    public List<AnchorTeamData> team_0 = new();
    public List<AnchorTeamData> team_1 = new();
    public List<AnchorTeamData> team_2 = new();
    public string date;
}

public class VpsAnchorService : MonoBehaviour
{
    [SerializeField]
    private GameObject _gooPrefab;

    [SerializeField]
    private GooTeamMaterials _gooTeamMaterials;

    private CancellationTokenSource _cts;

    private string _vpsId;

    public static event Action OnGooCreated;

    private void Awake()
    {
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _cts.Cancel();
    }

    private void Start()
    {
        VpsTracker.OnVpsTrackingStarted += OnVpsTrackingStarted;
        VpsManager.OnVpsWillClose += SendAnchorData;
    }

    private void OnVpsTrackingStarted(string vpsId)
    {
        _vpsId = vpsId;
        LoadAnchorData(vpsId);
    }

    public async void LoadAnchorData(string vpsId)
    {
        AnchorData anchor = await Firebase.GetDataAsync<AnchorData>(
            $"/vps-anchors/{vpsId}",
            _cts.Token
        );

        if (anchor == null)
        {
            Debug.LogWarning("No anchor data found");
            return;
        }

        LoadTeamGoo(anchor.team_0, 0);
        LoadTeamGoo(anchor.team_1, 1);
        LoadTeamGoo(anchor.team_2, 2);
    }

    private void LoadTeamGoo(List<AnchorTeamData> teamData, int teamNumber)
    {
        foreach (var data in teamData)
        {
            Vector3 position = ParseVector3(data.position);
            Quaternion rotation = ParseQuaternion(data.rotation);

            Goo goo = Instantiate(_gooPrefab, position, rotation).GetComponent<Goo>();
            goo.ApplyDeform();
            goo.Team = teamNumber;

            Material teamMaterial = _gooTeamMaterials.GetTeamMaterial(teamNumber);
            if (teamMaterial != null)
            {
                goo.SetMaterial(teamMaterial);
            }
            else
            {
                Debug.LogError($"Material not found for team {teamNumber}");
            }
        }

        OnGooCreated?.Invoke();
    }

    private Vector3 ParseVector3(string vectorString)
    {
        string[] s = vectorString.Trim('(', ')').Split(',');
        return new Vector3(
            float.Parse(s[0], CultureInfo.InvariantCulture),
            float.Parse(s[1], CultureInfo.InvariantCulture),
            float.Parse(s[2], CultureInfo.InvariantCulture)
        );
    }

    private Quaternion ParseQuaternion(string quaternionString)
    {
        string[] s = quaternionString.Trim('(', ')').Split(',');
        return Quaternion.Euler(
            float.Parse(s[0], CultureInfo.InvariantCulture),
            float.Parse(s[1], CultureInfo.InvariantCulture),
            float.Parse(s[2], CultureInfo.InvariantCulture)
        );
    }

    public AnchorData GetAnchorData()
    {
        List<Goo> _goos = new();
        foreach (var goo in FindObjectsOfType<Goo>())
        {
            if (goo.gameObject.activeInHierarchy)
            {
                _goos.Add(goo);
            }
        }

        AnchorData anchorData = new() { date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") };

        foreach (var goo in _goos)
        {
            AnchorTeamData teamData =
                new()
                {
                    position = goo.transform.position.ToString(),
                    rotation = goo.transform.rotation.eulerAngles.ToString()
                };

            switch (goo.Team)
            {
                case 0:
                    anchorData.team_0.Add(teamData);
                    break;
                case 1:
                    anchorData.team_1.Add(teamData);
                    break;
                case 2:
                    anchorData.team_2.Add(teamData);
                    break;
                default:
                    Debug.LogError($"Invalid team number: {goo.Team}");
                    break;
            }
        }

        return anchorData;
    }

    private async void SendAnchorData()
    {
        if (string.IsNullOrEmpty(_vpsId))
        {
            Debug.LogError("VPS ID not set");
            return;
        }

        AnchorData currentAnchorData = GetAnchorData();

        string vpsAnchorData = JsonUtility.ToJson(currentAnchorData);

        AnchorData anchorData = await Firebase.PatchDataAsync<AnchorData>(
            $"/vps-anchors/{_vpsId}",
            vpsAnchorData
        //_cts.Token
        );

        if (anchorData == null)
        {
            Debug.LogError("Failed to send anchor data");
            return;
        }

        VpsStatus status =
            new()
            {
                team_0_score = currentAnchorData.team_0.Count,
                team_1_score = currentAnchorData.team_1.Count,
                team_2_score = currentAnchorData.team_2.Count,
                date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

        string vpsStatusData = JsonUtility.ToJson(status);

        VpsStatus vpsStatus = await Firebase.PatchDataAsync<VpsStatus>(
            $"/vps-status/{_vpsId}",
            vpsStatusData
        //_cts.Token
        );

        if (vpsStatus == null)
        {
            Debug.LogError("Failed to update VPS status");
            return;
        }

        Debug.Log("Anchor data sent successfully");
    }
}
