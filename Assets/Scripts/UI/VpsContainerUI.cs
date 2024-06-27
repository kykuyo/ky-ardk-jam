using System;
using Niantic.Lightship.AR.VpsCoverage;
using Sliders;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VpsContainerUI : MonoBehaviour
{
    [SerializeField]
    private CoverageClientManager _coverageClientManager;

    [SerializeField]
    private RectTransform _panel;

    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private RawImage _image;

    [SerializeField]
    private PieChart _pieChart;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private Button _testSceneButton;

    private AreaTarget _areaTarget;

    private void Start()
    {
        VpsService.Instance.OnVpsStatusReceived += OnVpsStatusReceived;
    }

    private void OnDestroy()
    {
        VpsService.Instance.OnVpsStatusReceived -= OnVpsStatusReceived;
    }

    private void OnVpsStatusReceived(string vpsId, VpsStatus status)
    {
        if (vpsId != _areaTarget.Target.Identifier)
        {
            return;
        }

        _pieChart.SetValues(
            new float[] { status.team_0_score, status.team_1_score, status.team_2_score }
        );
    }

    public void ShowVpsContainerUI(AreaTarget areaTarget)
    {
        _areaTarget = areaTarget;
        _pieChart.SetValues(new float[] { 0, 0, 0 });
        _panel.gameObject.SetActive(true);

        _text.text = areaTarget.Target.Name;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            GameManager.Instance.SetAreaTarget(areaTarget);
            SceneManager.LoadScene("VPS_Goo");
        });

        _testSceneButton.onClick.RemoveAllListeners();
        _testSceneButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SetAreaTarget(areaTarget);
            SceneManager.LoadScene("VPS_TEST");
        });

        _image.texture = null;

        if (areaTarget.Target.ImageURL == null)
        {
            return;
        }

        _coverageClientManager.TryGetImageFromUrl(
            areaTarget.Target.ImageURL,
            downLoadedImage =>
            {
                if (downLoadedImage == null)
                {
                    return;
                }
                _image.texture = downLoadedImage;
            }
        );
    }
}
