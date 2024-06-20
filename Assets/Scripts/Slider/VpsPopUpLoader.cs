using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    [System.Serializable]
    public class ArMessages
    {
        public string Message { get; set; }
        public float Duration { get; set; }

        public ArMessages(string message, float duration)
        {
            Message = message;
            Duration = duration;
        }
    }


    public class VpsPopUpLoader : MonoBehaviour
    {
        [SerializeField] TMP_Text _text;
        [SerializeField] Image _image;
        [SerializeField] GameObject _vps_popUp;

        public void OnOpenLocation(string locationName,Sprite sprite)
        {
            _text.text = locationName;
            _image.sprite = sprite;
            _vps_popUp.SetActive(true);
        }

        public void OnCloseLocation()
        {
            _vps_popUp.SetActive(false);
        }

    }
}