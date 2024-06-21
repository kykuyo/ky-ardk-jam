using UnityEngine;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using System.Collections;

public class Water : MonoBehaviour
{
    [SerializeField]
    private float _lifetime = 2f;

    [SerializeField]
    private float _splashLifeTime = 0.3f;

    [SerializeField]
    private float _bounceSpeed = 6f;

    [SerializeField]
    private float _randomness = 40f;

    private Vector3 _originalScale;

    private Rigidbody _rigidbody;
    private Action<GameObject> _returnToPool;
    private NativeArray<float3> _bounceDirection;
    private JobHandle _bounceJobHandle;
    private bool _jobScheduled;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _bounceDirection = new NativeArray<float3>(1, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        if (_bounceDirection.IsCreated)
        {
            _bounceDirection.Dispose();
        }
    }

    public void Initialize(Action<GameObject> returnToPool, float waterSize)
    {
        _originalScale = new Vector3(waterSize, waterSize, waterSize);
        _returnToPool = returnToPool;
    }

    public void LaunchWater(Vector3 position, Vector3 direction, float speed)
    {
        transform.position = position;
        transform.localScale = _originalScale;
        gameObject.SetActive(true);
        if (_rigidbody != null)
        {
            _rigidbody.velocity = direction * speed;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        Invoke(nameof(ReturnToPool), _lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.localScale = _originalScale * 2.5f;

        Vector3 collisionNormal = collision.contacts[0].normal;
        float3 randomDirection = (float3)UnityEngine.Random.insideUnitSphere * _randomness;

        if (_jobScheduled)
        {
            _bounceJobHandle.Complete();
            _jobScheduled = false;
        }

        var bounceJob = new CalculateBounceJob
        {
            Forward = transform.forward,
            Normal = (float3)collisionNormal,
            RandomDirection = randomDirection,
            BounceSpeed = _bounceSpeed,
            Result = _bounceDirection
        };

        _bounceJobHandle = bounceJob.Schedule();
        _jobScheduled = true;
        StartCoroutine(CompleteBounceJob());
    }

    private IEnumerator CompleteBounceJob()
    {
        yield return new WaitUntil(() => _bounceJobHandle.IsCompleted);
        _bounceJobHandle.Complete();
        _jobScheduled = false;

        if (_rigidbody != null)
        {
            _rigidbody.velocity = (Vector3)_bounceDirection[0];
        }
        Invoke(nameof(ReturnToPool), _splashLifeTime);
    }

    private void ReturnToPool()
    {
        CancelInvoke();
        _returnToPool?.Invoke(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
        if (_jobScheduled)
        {
            _bounceJobHandle.Complete();
            _jobScheduled = false;
        }
    }

    [BurstCompile]
    private struct CalculateBounceJob : IJob
    {
        public float3 Forward;
        public float3 Normal;
        public float3 RandomDirection;
        public float BounceSpeed;
        public NativeArray<float3> Result;

        public void Execute()
        {
            float3 bounceDirection = math.reflect(Forward, Normal) + RandomDirection;
            bounceDirection = math.normalize(bounceDirection) * BounceSpeed;
            Result[0] = bounceDirection;
        }
    }
}
