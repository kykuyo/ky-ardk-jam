using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class ColorGooSelector : MonoBehaviour
    {
        [SerializeField] private Button[] _buttons;
        [SerializeField] private Button _nextButton;

        private void Awake()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                int index = i;

                _buttons[i].onClick.AddListener(() => OnButtonSelected(index));
            }

            if (null == _nextButton)
                return;

            _nextButton.onClick.AddListener(SetDefault);
        }


        private void OnDestroy()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].onClick.RemoveAllListeners();
            }

            if (null == _nextButton)
                return;
            _nextButton.onClick.RemoveAllListeners();
        }

        public void SetDefault()
        {
            OnButtonSelected(0);
        }

        private void OnButtonSelected(int index)
        {
            GooColorIndexController.Instance.SetIndex(index);
            //AGREGAR PARA QUE MUESTRE EL GOO DE FORMA CORRECTA
        }
    }
}