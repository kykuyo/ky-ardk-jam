using UnityEngine;

public class GameBuddy : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _speed = 1.0f;

    private void Start()
    {
        Invoke(nameof(SetStartPosition), 0.1f);
    }

    private void SetStartPosition()
    {
        transform.position = _target.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            _speed * Time.deltaTime
        );
    }
}
