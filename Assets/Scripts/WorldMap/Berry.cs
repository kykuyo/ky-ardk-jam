using UnityEngine;

public class Berry : MapEntity
{
    [SerializeField]
    private int _staminaAmount = 10;

    public int StaminaAmount => _staminaAmount;

    protected override void OnPlayerInteract()
    {
        MapBuddy mapBuddy = FindObjectOfType<MapBuddy>();
        mapBuddy.SetTarget(transform);
    }
}
