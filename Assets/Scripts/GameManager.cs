using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float MaxStamina => MAX_STAMINA;
    private const float MAX_STAMINA = 100;

    public float BuddyStamina
    {
        get { return buddyStamina; }
        private set
        {
            float newValue = value > MAX_STAMINA ? MAX_STAMINA : value;
            newValue = newValue < 0 ? 0 : newValue;
            if (buddyStamina != newValue)
            {
                buddyStamina = newValue;
                OnStaminaChanged?.Invoke(buddyStamina);
            }
        }
    }

    private float buddyStamina = 0;

    public int Team { get; private set; }

    public CurrentVpsData CurrentVpsData { get; private set; }

    public event Action<float> OnStaminaChanged;

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

    private void Start()
    {
        Team = PlayerPrefs.GetInt("SelectedTeam");
        MapBuddy.OnBerryEaten += FeedBuddy;
        TeamManager.OnTeamSelected += SetTeam;
        TeamManager.OnGameReset += ResetGame;
    }

    private void OnDestroy()
    {
        TeamManager.OnTeamSelected -= SetTeam;
        TeamManager.OnGameReset -= ResetGame;
        MapBuddy.OnBerryEaten -= FeedBuddy;
    }

    private void SetTeam(int team)
    {
        Team = team;
    }

    public void SetCurrentVpsData(CurrentVpsData data)
    {
        CurrentVpsData = data;
    }

    private void ResetGame()
    {
        BuddyStamina = 0;
    }

    public void FeedBuddy(float amount)
    {
        BuddyStamina += amount;
    }

    public void DecreaseBuddyStamina(float amount)
    {
        BuddyStamina -= amount;
    }
}
