using UnityEngine;

public class GenericSpawner<T> : MonoBehaviour
    where T : MonoBehaviour, IProjectile
{
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private float _projectileSize = 0.1f;

    [SerializeField]
    private float _jetSize = 0.01f;

    [SerializeField]
    private float _jetSpeed = 20f;

    [SerializeField]
    private int _poolSize = 150;

    [SerializeField]
    private int _burstCount = 5;

    [SerializeField]
    private LayerMask layerMask;

    protected Vector3 _targetPosition;
    protected ProjectilePool<T> _projectilePool;

    private Transform _parentTransform;

    public bool IsBlocked { get; set; } = false;

    private bool _isMouseButtonDown = false;

    public static event System.Action<Vector3> OnProjectileSpawned;

    public static event System.Action OnSpawnerReleased;

    public void SetParentTransform(Transform parentTransform)
    {
        _parentTransform = parentTransform;
    }

    protected virtual void Awake()
    {
        _projectilePool = new ProjectilePool<T>(
            _prefab,
            _poolSize,
            _projectileSize,
            _parentTransform
        );
    }

    private void Update()
    {
        if (IsBlocked)
        {
            return;
        }

        if (
            Input.GetMouseButton(0)
            && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
        )
        {
            _targetPosition = GetMouseWorldPosition();
            SpawnBurst();
            _isMouseButtonDown = true;
        }
        else if (_isMouseButtonDown)
        {
            _isMouseButtonDown = false;
            OnSpawnerReleased?.Invoke();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void SpawnBurst()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            SpawnProjectile();
        }
    }

    protected virtual void SpawnProjectile()
    {
        Vector3 spawnPosition = transform.position;

        GameObject instance = _projectilePool.GetProjectile();
        if (instance == null)
        {
            return;
        }

        instance.transform.position = spawnPosition;
        instance.transform.localScale = new Vector3(
            _projectileSize,
            _projectileSize,
            _projectileSize
        );
        instance.SetActive(true);

        if (instance.TryGetComponent(out T projectileScript))
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
            projectileScript.Launch(spawnPosition, direction, _jetSpeed);
            ConfigureProjectile(projectileScript);

            OnProjectileSpawned?.Invoke(direction);
        }
    }

    protected virtual void ConfigureProjectile(T projectileScript) { }
}
