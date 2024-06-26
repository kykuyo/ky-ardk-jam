using System;
using UnityEngine;
using UnityEngine.UI;

public class ResetGame : MonoBehaviour
{
    [SerializeField]
    private Button _resetButton;

    public static event Action OnGameReset;

    private void Start()
    {
        _resetButton.onClick.AddListener(Reset);
    }

    private void OnDestroy()
    {
        _resetButton.onClick.RemoveListener(Reset);
    }

    private void Reset()
    {
        OnGameReset?.Invoke();
    }
}
