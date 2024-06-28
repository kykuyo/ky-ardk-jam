using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField]
    private GooTeamMaterials _teamMaterials;

    [SerializeField]
    private Image _energyIcon;
    private Slider _slider;
    private float _maxStamina;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _maxStamina = GameManager.Instance.MaxStamina;
        int team = GameManager.Instance.Team;
        InitSlider(team);

        GameManager.Instance.OnStaminaChanged += UpdateStaminaBar;
        TeamSelector.OnTeamSelected += InitSlider;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStaminaChanged -= UpdateStaminaBar;
        TeamSelector.OnTeamSelected -= InitSlider;
    }

    private void InitSlider(int team)
    {
        _slider.value = GameManager.Instance.BuddyStamina / _maxStamina;

        Material teamMaterial = _teamMaterials.GetTeamMaterial(team);
        Color teamColor = GetMaterialColor(teamMaterial, "_Color", "_BaseColor");
        _slider.fillRect.GetComponent<Image>().color = teamColor;
        _energyIcon.color = teamColor;
    }

    private Color GetMaterialColor(Material material, params string[] colorPropertyNames)
    {
        foreach (var propName in colorPropertyNames)
        {
            if (material.HasProperty(propName))
            {
                return material.GetColor(propName);
            }
        }
        return Color.white;
    }

    private void UpdateStaminaBar(float newStamina)
    {
        if (_slider == null)
        {
            _slider = GetComponent<Slider>();
        }

        _slider.value = newStamina / _maxStamina;
    }
}
