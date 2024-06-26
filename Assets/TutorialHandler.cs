using Sliders;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField]
    private SlidersController _slidersController;

    private void Start()
    {
        GameManager.Instance.OnStartTutorial += StartTutorial;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartTutorial -= StartTutorial;
    }

    private void StartTutorial()
    {
        _slidersController.OnReset();
    }
}
