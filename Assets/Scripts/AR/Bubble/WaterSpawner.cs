using UnityEngine;
using System.Collections.Generic;

public class WaterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _waterPrefab;

    [SerializeField]
    private float _jetSize = 0.01f;

    [SerializeField]
    private float _jetSpeed = 20f;

    [SerializeField]
    private int _poolSize = 150;

    [SerializeField]
    private int _burstCount = 5;

    [SerializeField]
    private float _waterSize = 0.1f;

    private Vector3 _targetPosition;
    private Queue<GameObject> _waterPool;

    private void Awake()
    {
        InitializePool();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (GameManager.Instance.BuddyStamina <= 0)
            {
                return;
            }

            _targetPosition = GetMouseWorldPosition();
            SpawnWaterBurst();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void InitializePool()
    {
        _waterPool = new Queue<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject waterInstance = InstantiateWater();
            waterInstance.SetActive(false);
            _waterPool.Enqueue(waterInstance);
        }
    }

    private GameObject GetWaterFromPool()
    {
        if (_waterPool.Count > 0)
        {
            GameObject waterInstance = _waterPool.Dequeue();
            return waterInstance;
        }
        else
        {
            GameObject waterInstance = InstantiateWater();
            return waterInstance;
        }
    }

    private GameObject InstantiateWater()
    {
        GameObject waterInstance = Instantiate(
            _waterPrefab,
            transform.position,
            transform.rotation
        );
        waterInstance.transform.localScale = new Vector3(_waterSize, _waterSize, _waterSize);
        if (waterInstance.TryGetComponent(out Water waterScript))
        {
            waterScript.Initialize(ReturnWaterToPool, _waterSize);
        }
        waterInstance.SetActive(false);
        return waterInstance;
    }

    private void ReturnWaterToPool(GameObject waterInstance)
    {
        if (_waterPool.Count < _poolSize)
        {
            waterInstance.SetActive(false);
            _waterPool.Enqueue(waterInstance);
        }
        else
        {
            Destroy(waterInstance);
        }
    }

    private void SpawnWaterBurst()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            SpawnWater();
        }
    }

    private void SpawnWater()
    {
        Vector3 spawnPosition = transform.position;

        GameObject waterInstance = GetWaterFromPool();

        if (waterInstance.TryGetComponent(out Water waterScript))
        {
            Vector3 direction = (_targetPosition - spawnPosition).normalized;
            Vector3 randomDirectionOffset =
                new(
                    Random.Range(-_jetSize, _jetSize),
                    Random.Range(-_jetSize, _jetSize),
                    Random.Range(-_jetSize, _jetSize)
                );
            direction += randomDirectionOffset;
            direction.Normalize();
            waterScript.LaunchWater(spawnPosition, direction, _jetSpeed);
        }
    }
}
