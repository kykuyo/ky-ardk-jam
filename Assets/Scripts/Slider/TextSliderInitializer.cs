using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class TextSliderInitializer : SliderInitializizer
    {
        [SerializeField]
        private TMP_Text[] _texts;

        [SerializeField]
        private Image[] _backgrounds;

        [SerializeField]
        private float _fadeDuration;

        [SerializeField]
        private GameObject _container;

        private void Awake()
        {
            foreach (var text in _texts)
            {
                Color color = text.color;
                color.a = 0;
                text.color = color;
            }

            foreach (var background in _backgrounds)
            {
                Color color = background.color;
                color.a = 0;
                background.color = color;
                //background.gameObject.SetActive(false);
            }
            _container.SetActive(false);
        }

        public override void OnClose()
        {
            StartCoroutine(FadeTextsAndBackgroundsToZeroAlpha());
        }

        public override void OnInit()
        {
            _container.SetActive(true);
            StartCoroutine(FadeTextsAndBackgroundsToFullAlpha());
        }

        private IEnumerator FadeTextsAndBackgroundsToFullAlpha()
        {
            float elapsedTime = 0;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / _fadeDuration);

                foreach (var text in _texts)
                {
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                }

                foreach (var background in _backgrounds)
                {
                    Color color = background.color;
                    color.a = alpha;
                    background.color = color;
                }

                yield return null;
            }

            foreach (var text in _texts)
            {
                Color color = text.color;
                color.a = 1;
                text.color = color;
            }

            foreach (var background in _backgrounds)
            {
                Color color = background.color;
                color.a = 1;
                background.color = color;
            }
        }

        private IEnumerator FadeTextsAndBackgroundsToZeroAlpha()
        {
            float elapsedTime = 0;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1 - (elapsedTime / _fadeDuration));

                foreach (var text in _texts)
                {
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                }

                foreach (var background in _backgrounds)
                {
                    Color color = background.color;
                    color.a = alpha;
                    background.color = color;
                }

                yield return null;
            }

            foreach (var text in _texts)
            {
                Color color = text.color;
                color.a = 0;
                text.color = color;
            }

            foreach (var background in _backgrounds)
            {
                Color color = background.color;
                color.a = 0;
                background.color = color;
            }
            _container.SetActive(false);
        }
    }
}
