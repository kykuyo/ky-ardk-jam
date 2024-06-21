using System;
using Niantic.Lightship.AR.VpsCoverage;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class CurrentVpsData
{
    public string Identifier;
    public string Name;
    public string ImageURL;
}

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
    private Button _button;

    public void ShowVpsContainerUI(AreaTarget areaTarget)
    {
        _panel.gameObject.SetActive(true);

        _text.text = areaTarget.Target.Name;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            CurrentVpsData data =
                new()
                {
                    Identifier = areaTarget.Target.Identifier,
                    Name = areaTarget.Target.Name,
                    ImageURL = areaTarget.Target.ImageURL
                };

            PlayerPrefs.SetString("CurrentVpsData", JsonUtility.ToJson(data));
            PlayerPrefs.Save();

            SceneManager.LoadScene("VPS_Goo");
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
