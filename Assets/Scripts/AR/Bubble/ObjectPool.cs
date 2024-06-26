using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool<T>
    where T : MonoBehaviour, IProjectile
{
    private GameObject _prefab;
    private int _poolSize;
    float _projectileSize;
    private Queue<GameObject> _pool;
    private Transform _parentTransform;

    public ProjectilePool(
        GameObject prefab,
        int poolSize,
        float projectileSize,
        Transform parentTransform
    )
    {
        if (prefab == null)
            throw new System.ArgumentNullException(nameof(prefab));
        if (poolSize <= 0)
            throw new System.ArgumentOutOfRangeException(
                nameof(poolSize),
                "Pool size must be greater than 0."
            );
        if (projectileSize < 0)
            throw new System.ArgumentOutOfRangeException(
                nameof(projectileSize),
                "Projectile size must be non-negative."
            );

        _prefab = prefab;
        _poolSize = poolSize;
        _projectileSize = projectileSize;
        _parentTransform = parentTransform;

        InitializePool();
    }

    private void InitializePool()
    {
        _pool = new Queue<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject instance = InstantiateProjectile();
            if (instance != null)
            {
                instance.SetActive(false);
                _pool.Enqueue(instance);
            }
        }
    }

    private GameObject InstantiateProjectile()
    {
        GameObject instance = Object.Instantiate(_prefab, _parentTransform);
        if (instance != null && instance.TryGetComponent(out T projectileScript))
        {
            projectileScript.Initialize(ReturnToPool, _projectileSize);
            instance.SetActive(false);
            return instance;
        }
        return null;
    }

    public GameObject GetProjectile()
    {
        while (_pool.Count > 0)
        {
            GameObject instance = _pool.Dequeue();
            if (instance != null)
            {
                return instance;
            }
        }
        return InstantiateProjectile();
    }

    public void ReturnToPool(GameObject instance)
    {
        if (instance == null)
            return;
        if (_pool.Count < _poolSize && !_pool.Contains(instance))
        {
            instance.SetActive(false);
            _pool.Enqueue(instance);
        }
        else
        {
            Object.Destroy(instance);
        }
    }
}
