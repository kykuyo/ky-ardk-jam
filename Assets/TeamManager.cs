using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _teamSelectionPanel;

    [SerializeField]
    private List<Image> _images;

    private int _selectedTeam = -1;

    public static event Action<int> OnTeamSelected;

    public static event Action OnGameReset;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SelectedTeam"))
        {
            _selectedTeam = PlayerPrefs.GetInt("SelectedTeam");
            _teamSelectionPanel.gameObject.SetActive(false);
        }
        else
        {
            _teamSelectionPanel.gameObject.SetActive(true);
        }

        UpdateButtonStates();
    }

    public void SelectTeam(int teamIndex)
    {
        _selectedTeam = teamIndex;
        UpdateButtonStates();
    }

    public void Confirm()
    {
        if (_selectedTeam != -1)
        {
            PlayerPrefs.SetInt("SelectedTeam", _selectedTeam);
            PlayerPrefs.Save();
            _teamSelectionPanel.gameObject.SetActive(false);
        }

        OnTeamSelected?.Invoke(_selectedTeam);
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].color = new Color(
                _images[i].color.r,
                _images[i].color.g,
                _images[i].color.b,
                i == _selectedTeam ? 1f : 0.5f
            );
        }
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteKey("SelectedTeam");
        _teamSelectionPanel.gameObject.SetActive(true);
        _selectedTeam = -1;
        UpdateButtonStates();
        OnGameReset?.Invoke();
    }
}
