using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MapBuddy : MonoBehaviour
{
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private float _followInterval = 1.0f;

    [SerializeField]
    private float _waitTime = 2.0f;

    [SerializeField]
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    private Transform _target;
    private bool _isEating = false;

    public static event Action<float> OnBerryEaten;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(FollowPlayer), 0.0f, _followInterval);
    }

    public void SetTarget(Transform target)
    {
        if (GameManager.Instance.BuddyStamina >= GameManager.Instance.MaxStamina)
        {
            return;
        }

        if (_isEating)
        {
            return;
        }

        _target = target;
        _navMeshAgent.SetDestination(target.position);
    }

    private void FollowPlayer()
    {
        if (_player == null)
        {
            return;
        }

        if (_target != null)
        {
            return;
        }

        _navMeshAgent.SetDestination(_player.position);
    }

    private void Update()
    {
        if (
            _target != null
            && !_navMeshAgent.pathPending
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance
            && !_isEating
        )
        {
            StartCoroutine(EatBerry());
        }

        _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }

    private IEnumerator EatBerry()
    {
        _isEating = true;
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(_waitTime);
        Berry berry = _target.GetComponent<Berry>();
        berry.obj.Dispose();
        OnBerryEaten?.Invoke(berry.StaminaAmount);
        _target = null;
        _isEating = false;
    }

    void OnAnimatorMove()
    {
        Vector3 position = _animator.rootPosition;
        position.y = _navMeshAgent.nextPosition.y;
        transform.position = position;
        _navMeshAgent.nextPosition = transform.position;
    }
}
