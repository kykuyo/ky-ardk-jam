using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Sliders
{
    public class TextSliderInitializer : SliderInitializizer
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private GameObject _background;

        [SerializeField]
        private float _fadeDuration;

        private void Awake()
        {
            Color color = _text.color;
            color.a = 0;
            _text.color = color;
            _background.SetActive(false);
        }

        public override void OnClose()
        {
            StartCoroutine(FadeTextToZeroAlpha());
        }

        public override void OnInit()
        {
            _background.SetActive(true);
            StartCoroutine(FadeTextToFullAlpha());
        }

        IEnumerator FadeTextToFullAlpha()
        {
            float elapsedTime = 0;
            Color color = _text.color;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / _fadeDuration);
                _text.color = color;
                yield return null;
            }

            color.a = 1;
            _text.color = color;
        }

        IEnumerator FadeTextToZeroAlpha()
        {
            float elapsedTime = 0;
            Color color = _text.color;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(1 - (elapsedTime / _fadeDuration));
                _text.color = color;
                yield return null;
            }

            color.a = 0;
            _text.color = color;
            _background.SetActive(false);
        }
    }
}
