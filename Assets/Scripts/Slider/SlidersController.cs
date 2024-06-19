using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class SlidersController : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private SliderInitializizer[] _slidersInitializers;
        [SerializeField] private float _secondsToWait;


        private SliderInitializizer _currentSlider;
        private int _sliderCount = 0;

        private readonly string _slidersKeyOnPlayerPref;
        private bool _isTutorialAlreadyRead;

        private void Awake()
        {
#if !UNITY_EDITOR
            if (AlreadyReadTutorial())
            {
                CloseSliders();
                return;
            }
#endif

            _nextButton.onClick.AddListener(OnClickNextButton);
            _currentSlider = _slidersInitializers[_sliderCount];

            _currentSlider.OnInit();
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
            StartCoroutine(WaitToContinue());
            _currentSlider.OnClose();

            _sliderCount++;

            if(_sliderCount > _slidersInitializers.Length - 1)
            {
                CloseSliders();
#if !UNITY_EDITOR
                PlayerPrefs.SetInt(_slidersKeyOnPlayerPref, 1);
#endif
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