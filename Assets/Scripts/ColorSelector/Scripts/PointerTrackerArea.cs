using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorSelector
{
    [RequireComponent(typeof(RectTransform))]
    public class PointerTrackerArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        private const string TRACK_COROUTINE = "TrackCoroutine";

        public delegate void OnDrag(PointerTrackerArea sender, Vector2 position);
        public OnDrag Drag;

        private RectTransform _rectTransform;
        private Canvas _parentCanvas;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        public Vector2 Normalize(Vector2 position)
        {
            return new Vector2(position.x / _rectTransform.rect.width, position.y / _rectTransform.rect.height);
        }
        public Vector2 DeNormalize(Vector2 position)
        {
            return new Vector2(position.x * _rectTransform.rect.width, position.y * _rectTransform.rect.height);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartCoroutine(TRACK_COROUTINE);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            StopCoroutine(TRACK_COROUTINE);
        }

        private IEnumerator TrackCoroutine()
        {
            while (true)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, _parentCanvas.worldCamera, out Vector2 position);

                var rect = _rectTransform.rect;
                position.x = Mathf.Clamp(position.x, rect.min.x, rect.max.x);
                position.y = Mathf.Clamp(position.y, rect.min.y, rect.max.y);

                Drag?.Invoke(this, position);

                yield return 0;
            }
        }
    }
}