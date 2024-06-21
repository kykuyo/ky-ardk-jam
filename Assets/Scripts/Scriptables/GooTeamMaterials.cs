using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamMaterial
{
    public int teamId;
    public Material material;
}

[CreateAssetMenu(
    fileName = "GooTeamMaterials",
    menuName = "Ky-ARDK-Jam/GooTeamMaterials",
    order = 1
)]
public class GooTeamMaterials : ScriptableObject
{
    [SerializeField]
    private List<TeamMaterial> teamMaterialsList = new();

    private Dictionary<int, Material> teamMaterials = new();

    void OnEnable()
    {
        foreach (var teamMaterial in teamMaterialsList)
        {
            teamMaterials[teamMaterial.teamId] = teamMaterial.material;
        }
    }

    public Material GetTeamMaterial(int teamId)
    {
        if (teamMaterials.TryGetValue(teamId, out Material material))
        {
            return material;
        }
        else
        {
            Debug.LogWarning($"Material for team {teamId} not found.");
            return null;
        }
    }
}
