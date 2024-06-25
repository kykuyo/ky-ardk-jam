using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class ArInstruction : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;

        [SerializeField] private Color _color;

        private float _fadeDuration;

        private Coroutine _myCorrutine;

        private List<ArMessages> _nextMessages = new();

        public void InitMessage(string message,float fadeDuration = 1)
        {
            if(_myCorrutine == null)
            {
                _text.text = message;
                _fadeDuration = fadeDuration;
                _myCorrutine =StartCoroutine(FadeInCoroutine());
                return;
            }

            ArMessages next = new(message, fadeDuration);

            _nextMessages.Add(next);
        }

        public void FadeOut()
        {
            _myCorrutine = StartCoroutine(FadeOutCoroutine());

            if (_nextMessages.Count <= 0)
            {
                _myCorrutine = null;
                return;
            }

            ArMessages currentMessage = _nextMessages[0];

            _nextMessages.RemoveAt(0);

            _text.text = currentMessage.Message;
            _fadeDuration = currentMessage.Duration;

            _myCorrutine = StartCoroutine(FadeInCoroutine());


        }

        private IEnumerator FadeInCoroutine()
        {
            float counter = 0f;
            Color textColor = _text.color;
            Color imageColor = _image.color;

            while (counter < _fadeDuration)
            {
                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, counter / _fadeDuration);
                textColor.a = alpha;
                imageColor.a = alpha;
                _text.color = textColor;
                _image.color = _color;
                yield return null;
            }
            textColor.a = 1;
            imageColor.a = 1;
            _text.color = textColor;
            _image.color = imageColor;

            yield return new WaitForSeconds(1);
        }

        private IEnumerator FadeOutCoroutine()
        {
            float counter = 0f;
            Color textColor = _text.color;
            Color imageColor = _image.color;

            while (counter < _fadeDuration)
            {
                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, counter / _fadeDuration);
                textColor.a = alpha;
                imageColor.a = alpha;
                _text.color = textColor;
                _image.color = imageColor;
                yield return null;
            }
            textColor.a = 0;
            imageColor.a = 0;
            _text.color = textColor;
            _image.color = imageColor;
        }
    }
}