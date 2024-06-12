using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int buddyStamina = 0;

    public int BuddyStamina
    {
        get { return buddyStamina; }
        private set
        {
            if (buddyStamina != value)
            {
                buddyStamina = value;
                OnStaminaChanged?.Invoke(buddyStamina);
            }
        }
    }

    public static event Action<int> OnStaminaChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        MapBuddy.OnBerryEaten += FeedBuddy;
    }

    private void OnDisable()
    {
        MapBuddy.OnBerryEaten -= FeedBuddy;
    }

    public void FeedBuddy(int amount)
    {
        BuddyStamina += amount;
        Debug.Log($"Buddy stamina: {BuddyStamina}");
    }
}
