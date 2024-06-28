using System.Collections.Generic;
using UnityEngine;

public class Berry : MapEntity
{
    [SerializeField]
    private int _staminaAmount = 10;

    [SerializeField]
    private GooTeamMaterials _teamMaterials;

    [SerializeField]
    private Vector3 _rotationSpeed = new(0, 50, 0);

    public int StaminaAmount => _staminaAmount;

    public int Team { get; private set; }

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        Team = Random.Range(0, 3);

        SetTeamMaterial();
    }

    private void SetTeamMaterial()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = _teamMaterials.GetTeamMaterial(Team);
    }

    protected override void OnPlayerInteract()
    {
        int team = GameManager.Instance.Team;

        if (team != Team)
        {
            return;
        }

        MapBuddy mapBuddy = FindObjectOfType<MapBuddy>();
        mapBuddy.SetTarget(transform);
    }

    void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}
