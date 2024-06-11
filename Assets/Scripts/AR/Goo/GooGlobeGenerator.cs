using UnityEngine;

public class GooGlobeGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _gooGlobePrefab;

    [SerializeField]
    private int _numberOfGlobes = 4;

    [SerializeField]
    private float _coneRadius = 5f;

    [SerializeField]
    private float _generationInterval = 0.5f;

    private Camera _mainCamera;
    private int _globesGenerated = 0;

    private const int MAX_GENERATION_ATTEMPTS = 10;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void StartGeneration()
    {
        InvokeRepeating(nameof(TryGenerateGlobe), 0, _generationInterval);
    }

    private void TryGenerateGlobe()
    {
        if (_globesGenerated >= _numberOfGlobes)
        {
            CancelInvoke(nameof(TryGenerateGlobe));
            return;
        }

        for (int i = 0; i < MAX_GENERATION_ATTEMPTS; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * _coneRadius;
            Vector3 direction = (_mainCamera.transform.forward + randomDirection).normalized;
            Ray ray = new(_mainCamera.transform.position, direction);

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 spawnPosition = (hit.point + _mainCamera.transform.position) / 2;

                if (Physics.CheckSphere(spawnPosition, _gooGlobePrefab.transform.localScale.x / 2))
                {
                    Debug.Log("GooGlobe detected, trying again...");

                    continue;
                }

                Instantiate(_gooGlobePrefab, spawnPosition, Quaternion.identity);
                _globesGenerated++;
                break;
            }
        }
    }
}
