using Niantic.Lightship.AR.VpsCoverage;
using UnityEngine;

public class MapVps : MapEntity
{
    public AreaTarget areaTarget;

    public VpsStatus vpsStatus;

    public GooTeamMaterials teamMaterial;

    private MeshRenderer pinMeshRenderer;

    private void Start()
    {
        pinMeshRenderer = GetComponentInChildren<MeshRenderer>();
        //GetVpsStatus();
    }

    protected override void OnPlayerInteract()
    {
        VpsContainerUI vpsContainerUI = FindObjectOfType<VpsContainerUI>();
        vpsContainerUI.ShowVpsContainerUI(areaTarget);
        GetVpsStatus();
    }

    public async void GetVpsStatus()
    {
        string vpsId = areaTarget.Target.Identifier;
        VpsStatus vpsStatus = await Firebase.GetDataAsync<VpsStatus>($"/vps-status/{vpsId}");

        if (vpsStatus == null)
        {
            Debug.Log("VPS status is null");
            return;
        }

        this.vpsStatus = vpsStatus;

        SetTeamMaterial();

        Debug.Log($"VPS status: {JsonUtility.ToJson(vpsStatus)}");
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

        // Determinar el equipo con la mayor puntuación
        int maxScore = Mathf.Max(team_0_score, team_1_score, team_2_score);

        // Asignar el material correspondiente al equipo con la mayor puntuación
        if (maxScore == team_0_score)
        {
            pinMeshRenderer.material = teamMaterial.GetTeamMaterial(0);
        }
        else if (maxScore == team_1_score)
        {
            pinMeshRenderer.material = teamMaterial.GetTeamMaterial(1);
        }
        else if (maxScore == team_2_score)
        {
            pinMeshRenderer.material = teamMaterial.GetTeamMaterial(2);
        }
    }
}
