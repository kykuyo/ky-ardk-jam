using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    private Image _staminaBar;
    private float _width;
    private float _maxStamina = 100;

    private void Start()
    {
        InitBar();
        GameManager.Instance.OnStaminaChanged += UpdateStaminaBar;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStaminaChanged -= UpdateStaminaBar;
    }

    private void InitBar()
    {
        //TODO: Replace this for an slider.
        _staminaBar = transform.GetChild(0).GetComponent<Image>();
        RectTransform rectTransform = _staminaBar.GetComponent<RectTransform>();
        _width = rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.y);

        float stamina = GameManager.Instance.BuddyStamina;
        UpdateStaminaBar(stamina);
    }

    private void UpdateStaminaBar(float newStamina)
    {
        float newWidth = newStamina / _maxStamina * _width;
        RectTransform rectTransform = _staminaBar.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
    }
}
