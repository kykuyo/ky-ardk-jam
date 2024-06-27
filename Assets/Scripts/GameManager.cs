using System;
using Sliders;
using UnityEngine;
using UnityEngine.SceneManagement;
using Niantic.Lightship.AR.VpsCoverage;

public class GameManager : MonoBehaviour
{
    private const string PLAYER_TEAM_KEY = "PlayerTeam";
    private const float MAX_STAMINA = 100;
    public float MaxStamina => MAX_STAMINA;

    public static GameManager Instance { get; private set; }

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

    public AreaTarget AreaTarget { get; private set; }

    public event Action<float> OnStaminaChanged;

    public event Action OnStartTutorial;

    private void Awake()
    {
        LoadTeam();
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
        MapBuddy.OnBerryEaten += FeedBuddy;
        TeamSelector.OnTeamSelected += SetTeam;
        ResetGame.OnGameReset += Reset;
    }

    private void OnDestroy()
    {
        MapBuddy.OnBerryEaten -= FeedBuddy;
        TeamSelector.OnTeamSelected -= SetTeam;
        ResetGame.OnGameReset -= Reset;
    }

    private void LoadTeam()
    {
        if (!PlayerPrefs.HasKey(PLAYER_TEAM_KEY))
        {
            OnStartTutorial?.Invoke();
            return;
        }

        Team = PlayerPrefs.GetInt(PLAYER_TEAM_KEY);
    }

    private void SetTeam(int team)
    {
        Team = team;
        PlayerPrefs.SetInt(PLAYER_TEAM_KEY, team);
        PlayerPrefs.Save();
    }

    public void SetAreaTarget(AreaTarget data)
    {
        AreaTarget = data;
    }

    public void FeedBuddy(float amount)
    {
        BuddyStamina += amount;
    }

    public void DecreaseBuddyStamina(float amount)
    {
        BuddyStamina -= amount;
    }

    private void Reset()
    {
        PlayerPrefs.DeleteKey(PLAYER_TEAM_KEY);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
