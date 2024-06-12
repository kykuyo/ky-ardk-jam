using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Image _staminaBar;
    private float _width;
    private int _maxStamina = 100;

    private void Start()
    {
        InitBar();
        GameManager.OnStaminaChanged += UpdateStaminaBar;
    }

    private void OnDestroy()
    {
        GameManager.OnStaminaChanged -= UpdateStaminaBar;
    }

    private void InitBar()
    {
        //TODO: Replace this for an slider.
        _staminaBar = transform.GetChild(0).GetComponent<Image>();
        RectTransform rectTransform = _staminaBar.GetComponent<RectTransform>();
        _width = rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.y);

        int stamina = GameManager.Instance.BuddyStamina;
        UpdateStaminaBar(stamina);
    }

    private void UpdateStaminaBar(int newStamina)
    {
        float newWidth = newStamina / (float)_maxStamina * _width;
        RectTransform rectTransform = _staminaBar.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
    }
}
