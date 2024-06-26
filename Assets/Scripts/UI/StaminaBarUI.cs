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
        _slider.fillRect.GetComponent<Image>().color = _teamMaterials.GetTeamMaterial(team).color;
        _energyIcon.color = _teamMaterials.GetTeamMaterial(team).color;
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
