using UnityEngine;

namespace Sliders
{
    public class ResetDataToTutorial : MonoBehaviour
    {
        [SerializeField] private SlidersController _slidersController;
        [SerializeField] private GameObject _popUp;
        public void OnConfirm()
        {
            PlayerPrefs.SetInt(PrefsKeysPointer.tutorial, 0);
            _slidersController.OnReset();
            _popUp.SetActive(false);
        }
    }
}