using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class VpsManager : MonoBehaviour
{
    [Serializable]
    public class GameMode
    {
        public GameObject spawner;
        public Image buttonImage;
    }

    [SerializeField]
    private GameMode gooMode;

    [SerializeField]
    private GameMode bubblesMode;

    [SerializeField]
    private int _maxGooCapacity = 300;

    [SerializeField]
    private RectTransform _closingPanel;

    [SerializeField]
    private RectTransform _statusPanel;

    [SerializeField]
    private TMP_Text _statusText;

    [SerializeField]
    private TMP_Text _totalGoosText;

    public static event Action<bool> OnGooCapacityStatusChanged;

    public static event Action OnVpsWillClose;

    public int TotalGoos { get; private set; }

    private void Awake()
    {
        Goo.OnGooCreated += UpdateGooCapacityStatus;
        Goo.OnGooDestroyed += UpdateGooCapacityStatus;
        VpsAnchorService.OnGooCreated += UpdateGooCapacityStatus;
    }

    private void OnDestroy()
    {
        Goo.OnGooCreated -= UpdateGooCapacityStatus;
        Goo.OnGooDestroyed -= UpdateGooCapacityStatus;
        VpsAnchorService.OnGooCreated -= UpdateGooCapacityStatus;
    }

    // TODO - Test if this is necessary
    private void OnApplicationQuit()
    {
        //OnVpsWillClose?.Invoke();
    }

    private void Start()
    {
        SetActiveMode(gooMode, bubblesMode);
    }

    private void UpdateGooCapacityStatus()
    {
        StartCoroutine(UpdateGooCapacityStatusCoroutine());
    }

    private IEnumerator UpdateGooCapacityStatusCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        TotalGoos = GetActiveGooCount();
        _totalGoosText.text = $"{TotalGoos}/{_maxGooCapacity}";
        OnGooCapacityStatusChanged?.Invoke(TotalGoos >= _maxGooCapacity);
        if (TotalGoos >= _maxGooCapacity)
        {
            _statusPanel.gameObject.SetActive(true);
            _statusText.text = "Goo capacity reached!";
        }
        else
        {
            _statusPanel.gameObject.SetActive(false);
        }
    }

    private int GetActiveGooCount()
    {
        var goos = FindObjectsOfType<Goo>();
        int activeGooCount = 0;
        for (int i = 0; i < goos.Length; i++)
        {
            if (goos[i].gameObject.activeSelf)
            {
                activeGooCount++;
            }
        }
        return activeGooCount;
    }

    public void SwitchMode()
    {
        SetActiveMode(
            gooMode.spawner.activeSelf ? bubblesMode : gooMode,
            gooMode.spawner.activeSelf ? gooMode : bubblesMode
        );
    }

    private void SetActiveMode(GameMode activeMode, GameMode inactiveMode)
    {
        activeMode.spawner.SetActive(true);
        inactiveMode.spawner.SetActive(false);

        SetButtonImageAlpha(activeMode.buttonImage, 1);
        SetButtonImageAlpha(inactiveMode.buttonImage, 0.5f);
    }

    private void SetButtonImageAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public void CloseVps()
    {
        OnVpsWillClose?.Invoke();
        _closingPanel.gameObject.SetActive(true);
        Invoke(nameof(LoadWorldMap), 2);
    }

    public void LoadWorldMap()
    {
        SceneManager.LoadScene("WorldMap");
    }
}
