using UnityEngine;

namespace ColorSelector
{
    [RequireComponent(typeof(PointerTrackerArea))]
    public class RectSelector : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;

        public delegate void OnValueChanged(RectSelector sender, Vector2 position);
        public OnValueChanged ValueChanged;

        public Vector2 Value
        {
            get { return _rectTransform.transform.localPosition; }
            set { _rectTransform.transform.localPosition = value; }
        }
        public Vector2 NormalizedValue
        {
            get { return _pointerTrackArea.Normalize(Value); }
            set { Value = _pointerTrackArea ? _pointerTrackArea.DeNormalize(value) : Vector2.zero; }
        }

        private PointerTrackerArea _pointerTrackArea;

        private void Awake()
        {
            _pointerTrackArea = GetComponent<PointerTrackerArea>();
            _pointerTrackArea.Drag = PointerTrackerArea_Drag;
        }

        private void PointerTrackerArea_Drag(PointerTrackerArea sender, Vector2 position)
        {
            Debug.Log(this.gameObject.name);
            _rectTransform.transform.localPosition = position;

            ValueChanged?.Invoke(this, position);
        }
    }
}