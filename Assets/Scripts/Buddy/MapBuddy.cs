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

    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private bool isEating = false;

    public static event Action<int> OnBerryEaten;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(FollowPlayer), 0.0f, _followInterval);
    }

    public void SetTarget(Transform target)
    {
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
            && !isEating
        )
        {
            StartCoroutine(EatBerry());
        }
    }

    private IEnumerator EatBerry()
    {
        isEating = true;
        yield return new WaitForSeconds(_waitTime);
        Berry berry = _target.GetComponent<Berry>();
        berry.obj.Dispose();
        OnBerryEaten?.Invoke(berry.StaminaAmount);
        _target = null;
        isEating = false;
    }
}
