using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelector : MonoBehaviour
{
    [SerializeField]
    private List<Image> _images;

    public static event Action<int> OnTeamSelected;

    private void Start()
    {
        UpdateButtonStates(0);
    }

    public void SelectTeam(int teamIndex)
    {
        UpdateButtonStates(teamIndex);
        OnTeamSelected?.Invoke(teamIndex);
    }

    private void UpdateButtonStates(int teamIndex)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].color = new Color(
                _images[i].color.r,
                _images[i].color.g,
                _images[i].color.b,
                i == teamIndex ? 1f : 0.5f
            );
        }
    }
}
