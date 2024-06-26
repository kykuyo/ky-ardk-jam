using System;
using Niantic.Lightship.AR.VpsCoverage;
using UnityEngine;

public class MapVps : MapEntity
{
    [SerializeField]
    private GooTeamMaterials _teamMaterial;

    public AreaTarget areaTarget;

    public VpsStatus vpsStatus;

    private MeshRenderer _pinMeshRenderer;

    private void Start()
    {
        _pinMeshRenderer = GetComponentInChildren<MeshRenderer>();
        VpsService.Instance.OnVpsStatusReceived += OnVpsStatusReceived;
    }

    private void OnDestroy()
    {
        VpsService.Instance.OnVpsStatusReceived -= OnVpsStatusReceived;
    }

    protected override void OnPlayerInteract()
    {
        VpsContainerUI vpsContainerUI = FindObjectOfType<VpsContainerUI>();

        vpsContainerUI.ShowVpsContainerUI(areaTarget);
        string vpsId = areaTarget.Target.Identifier;
        VpsService.Instance.GetVpsStatus(vpsId);
    }

    private void OnVpsStatusReceived(string vpsId, VpsStatus status)
    {
        if (vpsId != areaTarget.Target.Identifier)
        {
            return;
        }

        vpsStatus = status;

        SetTeamMaterial();
    }

    public void UpdateStatus(VpsStatus status)
    {
        vpsStatus = status;

        SetTeamMaterial();
    }

    private void SetTeamMaterial()
    {
        if (vpsStatus == null)
        {
            Debug.Log("VPS status is null");
            return;
        }

        int team_0_score = vpsStatus.team_0_score;
        int team_1_score = vpsStatus.team_1_score;
        int team_2_score = vpsStatus.team_2_score;

        int maxScore = Mathf.Max(team_0_score, team_1_score, team_2_score);

        // Verificar si maxScore es mayor que 0 antes de asignar el material
        if (maxScore > 0)
        {
            if (maxScore == team_0_score)
            {
                _pinMeshRenderer.material = _teamMaterial.GetTeamMaterial(0);
            }
            else if (maxScore == team_1_score)
            {
                _pinMeshRenderer.material = _teamMaterial.GetTeamMaterial(1);
            }
            else if (maxScore == team_2_score)
            {
                _pinMeshRenderer.material = _teamMaterial.GetTeamMaterial(2);
            }
        }
        else
        {
            Debug.Log("No team has a score greater than 0");
        }
    }
}
