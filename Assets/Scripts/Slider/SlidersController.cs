using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public static class PrefsKeysPointer
    {
        public static string tutorial = "tutorial";
    }

    public class SlidersController : MonoBehaviour
    {
        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private SliderInitializizer[] _slidersInitializers;

        [SerializeField]
        private float _secondsToWait;

        private SliderInitializizer _currentSlider;
        private int _sliderCount = 0;

        private readonly string _slidersKeyOnPlayerPref = PrefsKeysPointer.tutorial;
        private bool _isTutorialAlreadyRead;

        private void Awake()
        {
            _nextButton.onClick.AddListener(OnClickNextButton);

            if (AlreadyReadTutorial())
            {
                CloseSliders();
                return;
            }

            _currentSlider = _slidersInitializers[_sliderCount];

            _currentSlider.OnInit();
        }

        public void OnReset()
        {
            PlayerPrefs.SetInt(_slidersKeyOnPlayerPref, 0);

            _sliderCount = 0;
            _currentSlider = _slidersInitializers[_sliderCount];
            _currentSlider.OnInit();
            _nextButton.gameObject.SetActive(true);
        }

        private bool AlreadyReadTutorial()
        {
            if (!PlayerPrefs.HasKey(_slidersKeyOnPlayerPref))
            {
                PlayerPrefs.SetInt(_slidersKeyOnPlayerPref, 0);
                return false;
            }

            _isTutorialAlreadyRead = PlayerPrefs.GetInt(_slidersKeyOnPlayerPref) == 1;

            return _isTutorialAlreadyRead;
        }

        public void OnDestroy()
        {
            _nextButton.onClick.RemoveAllListeners();
        }

        private void OnClickNextButton()
        {
            Debug.Log("Next Button Clicked");
            StartCoroutine(WaitToContinue());
            _currentSlider.OnClose();

            _sliderCount++;

            if (_sliderCount > _slidersInitializers.Length - 1)
            {
                CloseSliders();

                PlayerPrefs.SetInt(_slidersKeyOnPlayerPref, 1);

                return;
            }
            _currentSlider = _slidersInitializers[_sliderCount];

            _currentSlider.OnInit();
        }

        private IEnumerator WaitToContinue()
        {
            _nextButton.enabled = false;
            yield return new WaitForSeconds(_secondsToWait);
            _nextButton.enabled = true;
        }

        private void CloseSliders()
        {
            _nextButton.gameObject.SetActive(false);

            for (int i = 0; i < _slidersInitializers.Length; i++)
            {
                _slidersInitializers[i].OnClose();
            }
        }
    }

    public abstract class SliderInitializizer : MonoBehaviour
    {
        public abstract void OnInit();

        public abstract void OnClose();
    }
}
