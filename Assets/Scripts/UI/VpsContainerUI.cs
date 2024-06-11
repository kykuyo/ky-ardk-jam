using Niantic.Lightship.AR.VpsCoverage;
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
    private Image _image;

    [SerializeField]
    private Button _button;

    public void ShowVpsContainerUI(AreaTarget areaTarget)
    {
        _panel.gameObject.SetActive(true);

        _text.text = areaTarget.Target.Name;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("VPS_Goo");
        });

        _image.sprite = null;

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
                _image.sprite = Sprite.Create(
                    (Texture2D)downLoadedImage,
                    new Rect(0, 0, downLoadedImage.width, downLoadedImage.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        );
    }
}
