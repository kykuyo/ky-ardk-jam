using System;
using UnityEngine;

public class GooSpawner : GenericSpawner<PlayerGoo>
{
    [SerializeField]
    private GooTeamMaterials _gooTeamMaterials;

    [SerializeField]
    private float _staminaCostPerShot = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        VpsManager.OnGooCapacityStatusChanged += OnGooCapacityStatusChanged;
        GameManager.Instance.OnStaminaChanged += OnStaminaChanged;
    }

    private void OnDestroy()
    {
        VpsManager.OnGooCapacityStatusChanged -= OnGooCapacityStatusChanged;
        GameManager.Instance.OnStaminaChanged -= OnStaminaChanged;
    }

    private void OnGooCapacityStatusChanged(bool isCapacityReached)
    {
        IsBlocked = isCapacityReached;
    }

    private void OnStaminaChanged(float amount)
    {
        IsBlocked = amount <= 0;
    }

    public void SetPoolParent(Transform parentTransform)
    {
        SetParentTransform(parentTransform);
    }

    protected override void SpawnProjectile()
    {
        if (GameManager.Instance.BuddyStamina >= _staminaCostPerShot)
        {
            base.SpawnProjectile();
            GameManager.Instance.DecreaseBuddyStamina(_staminaCostPerShot);
        }
        else
        {
            Debug.Log("Not enough stamina to shoot.");
        }
    }

    protected override void ConfigureProjectile(PlayerGoo goo)
    {
        goo.Team = GameManager.Instance.Team;

        Material teamMaterial = _gooTeamMaterials.GetTeamMaterial(goo.Team);
        if (teamMaterial != null)
        {
            goo.SetMaterial(teamMaterial);
        }
        else
        {
            Debug.LogError($"Material not found for team {goo.Team}");
        }
    }
}
