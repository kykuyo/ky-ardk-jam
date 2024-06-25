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
    private Animator _animator;

    private void Start()
    {
        Invoke(nameof(SetStartPosition), 0.1f);
    }

    private void SetStartPosition()
    {
        transform.position = _target.position;
        GenericSpawner<Bubble>.OnProjectileSpawned += Attack;
    }

    public void Attack(Vector3 targetPosition)
    {
        Vector3 displacement = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(displacement);
        transform.rotation = targetRotation;
        _animator.SetTrigger("Attack");
    }

    private void LateUpdate()
    {
        Vector3 displacement = _target.position - transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            _speed * Time.deltaTime
        );

        float velocity = displacement.magnitude / Time.deltaTime;

        _animator.SetFloat("Speed", velocity);

        if (displacement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(displacement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * _rotationSpeed
            );
        }
    }

    void OnAnimatorMove()
    {
        Vector3 position = _animator.rootPosition;
        transform.position = position;
    }
}
