using UnityEngine;
using UnityEngine.UI;

namespace ColorSelector
{
    public class HsbSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ColorSlider _colorSlider;

        [Header("Inner")]
        [SerializeField] private Image _imageColor;

        public delegate void OnValueChanged(HsbSelector sender, float hue, float saturation, float brightness);
        public OnValueChanged ValueChanged;

        public float Hue { get { return _colorSlider.Value; } }
        public float Saturation { get { return _picker.NormalizedValue.x; } }
        public float Brightness { get { return _picker.NormalizedValue.y; } }

        private RectSelector _picker;

        private void Awake()
        {
            _picker = GetComponent<RectSelector>();
        }
        private void Start()
        {
            _colorSlider.ValueChanged = Slider_ValueChanged;
            _picker.ValueChanged = RectPicker_ValueChanged;
        }

        public void SetColor(Color color)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out float brightness);

            _colorSlider.Value = hue;
            UpdateImageColor(hue);

            _picker.NormalizedValue = new Vector2(saturation, brightness);
        }

        private void Slider_ValueChanged(ColorSlider sender, float value)
        {
            UpdateImageColor(value);

            InvokeValueChanged();
        }
        private void RectPicker_ValueChanged(RectSelector sender, Vector2 position)
        {
            InvokeValueChanged();
        }

        private void UpdateImageColor(float hue)
        {
            _imageColor.color = Color.HSVToRGB(hue, 1, 1);
        }
        private void InvokeValueChanged()
        {
            ValueChanged?.Invoke(this, Hue, Saturation, Brightness);
        }
    }
}