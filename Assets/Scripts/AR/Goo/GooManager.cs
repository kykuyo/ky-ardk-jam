using System;
using UnityEngine;
using UnityEngine.Events;

public class GooManager : MonoBehaviour, IGooObserver
{
    [SerializeField]
    private GooGlobeGenerator _gooGlobeGenerator;

    [SerializeField]
    private GooPool _gooPool;

    [SerializeField]
    private float _scanningTime = 7f;

    private int _totalGooGenerated = 0;
    private int _totalGooCleaned = 0;

    public event Action OnGameWonAction;
    public event Action OnGameStartedAction;

    public UnityEvent OnGameStartedUnityEvent;
    public UnityEvent OnGameWonUnityEvent;

    private void Start()
    {
        _gooPool.RegisterObserver(this);
        Invoke(nameof(StartGame), _scanningTime);
    }

    private void StartGame()
    {
        _gooGlobeGenerator.StartGeneration();
        OnGameStartedAction?.Invoke();
        OnGameStartedUnityEvent?.Invoke();
    }

    public void OnGooCreated(GameObject goo)
    {
        _totalGooGenerated++;
        Debug.Log("Goo created. Total generated: " + _totalGooGenerated);
    }

    public void OnGooDestroyed(GameObject goo)
    {
        _totalGooCleaned++;
        Debug.Log("Goo destroyed. Total cleaned: " + _totalGooCleaned);

        if (_totalGooCleaned >= _totalGooGenerated * 0.75)
        {
            OnGameWonAction?.Invoke();
            OnGameWonUnityEvent?.Invoke();
        }
    }
}
