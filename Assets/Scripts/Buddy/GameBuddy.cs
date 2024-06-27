using System;
using UnityEngine;

public class GameBuddy : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _speed = 1.0f;

    [SerializeField]
    private float _rotationSpeed = 3.0f;

    [SerializeField]
    private float _minDistance = 5.0f;

    [SerializeField]
    private Animator _animator;

    private bool _startMoving = false;

    private Quaternion? _targetRotation = null;

    private void Start()
    {
        GenericSpawner<Bubble>.OnProjectileSpawned += Attack;
        Invoke(nameof(SetStartPosition), 1);
    }

    private void OnDestroy()
    {
        GenericSpawner<Bubble>.OnProjectileSpawned -= Attack;
    }

    private void SetStartPosition()
    {
        transform.position = _target.position;
        _startMoving = true;
    }

    public void Attack(Vector3 targetPosition)
    {
        Vector3 displacement = targetPosition - transform.position;
        _targetRotation = Quaternion.LookRotation(displacement);
    }

    private void FixedUpdate()
    {
        if (!_startMoving)
        {
            return;
        }

        Vector3 displacement = _target.position - transform.position;
        float distance = displacement.magnitude;

        if (distance > _minDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _target.position,
                _speed * Time.fixedDeltaTime
            );
        }

        if (_targetRotation.HasValue)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation.Value,
                Time.fixedDeltaTime * _rotationSpeed
            );

            // Opcional: Resetear _targetRotation si se desea detener la interpolación una vez alcanzada la rotación objetivo.
            // if (Quaternion.Angle(transform.rotation, _targetRotation.Value) < 1.0f) { _targetRotation = null; }
        }

        if (displacement != Vector3.zero && !_targetRotation.HasValue)
        {
            Quaternion targetRotation = Quaternion.LookRotation(displacement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.fixedDeltaTime * _rotationSpeed
            );
        }
    }
}
