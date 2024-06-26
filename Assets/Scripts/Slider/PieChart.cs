using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class PieChart : MonoBehaviour
    {
        [SerializeField]
        Image[] _pieChartsLevels;

        [SerializeField]
        private float[] _values;

        [SerializeField]
        private TMP_Text[] _texts;

        public void SetValues(float[] valueToSet)
        {
            bool allValuesAreZero = valueToSet.All(value => value == 0);
            SetImagesActive(!allValuesAreZero);

            if (allValuesAreZero || _pieChartsLevels.Length != valueToSet.Length)
            {
                if (_pieChartsLevels.Length != valueToSet.Length)
                {
                    Debug.LogError(
                        "The number of pie charts and the number of values must be the same."
                    );
                }
                return;
            }

            bool hasTexts = _texts.Length > 0;
            float totalValue = 0;

            for (int i = 0; i < _pieChartsLevels.Length; i++)
            {
                float percentage = FindPercentage(valueToSet, i);
                totalValue += percentage;
                _pieChartsLevels[i].fillAmount = totalValue;

                if (hasTexts && _texts[i] != null)
                {
                    _texts[i].text = $"{percentage * 100:F2}%";
                }
            }
        }

        private void SetImagesActive(bool isActive)
        {
            for (int i = 0; i < _pieChartsLevels.Length; i++)
            {
                _pieChartsLevels[i].gameObject.SetActive(isActive);
            }
        }

        private float FindPercentage(float[] valueToSet, int index)
        {
            float totalAmount = 0;

            for (int i = 0; i < valueToSet.Length; i++)
            {
                totalAmount += valueToSet[i];
            }

            return valueToSet[index] / totalAmount;
        }
    }
}
