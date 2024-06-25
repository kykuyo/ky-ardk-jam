using UnityEngine;

public class BubbleSpawner : GenericSpawner<Bubble>
{
    [SerializeField]
    private float _staminaCostPerShot = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.OnStaminaChanged += OnStaminaChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStaminaChanged -= OnStaminaChanged;
    }

    private void OnStaminaChanged(float amount)
    {
        IsBlocked = amount <= 0;
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
}
