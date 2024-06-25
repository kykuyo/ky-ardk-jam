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

        private void Start()
        {
            SetValues(_values);
        }
        
        public void SetValues(float[] valueToSet)
        {
            float totalValue = 0;

            for (int i = 0; i < _pieChartsLevels.Length; i++)
            {
                totalValue += FindPercentage(valueToSet, i);
                _pieChartsLevels[i].fillAmount = totalValue;
                _texts[i].text = valueToSet.ToString();
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
