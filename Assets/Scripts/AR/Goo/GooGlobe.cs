using System.Collections;
using UnityEngine;

public class GooGlobe : MonoBehaviour
{
    [Header("Life")]
    [SerializeField]
    private float _maxLife = 100f;

    [SerializeField]
    private float _damagePerBubble = 0.2f;

    [Header("Behaviour")]
    [SerializeField]
    private Vector2 _timeToExplodeRange = new(7f, 10f);

    [Header("Animation")]
    [SerializeField]
    private float _animationScale = 1.1f;

    [SerializeField]
    private float _animationDuration = 0.2f;

    [SerializeField]
    private AnimationCurve _animationCurve;

    [SerializeField]
    private int _raycastCount = 100;

    [SerializeField]
    private float _raycastDistance = 10f;

    //private GooSpawner _gooSpawner;

    private float _currentLife;
    private Vector3 _originalScale;
    private float _timeToExplode;

    private void Awake()
    {
        _currentLife = _maxLife;
        _originalScale = transform.localScale;
        _timeToExplode = Random.Range(_timeToExplodeRange.x, _timeToExplodeRange.y);
    }

    private void Start()
    {
        //_gooSpawner = FindObjectOfType<GooSpawner>();
        StartCoroutine(ExplodeAfterSeconds(_timeToExplode));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            _currentLife -= _damagePerBubble;
            if (_currentLife <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(AnimateScale());
            }
        }
    }

    private IEnumerator ExplodeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Vector3 bestPosition = FindBestGooSpawnPosition();
        // _gooSpawner.SpawnGoo(transform.position, bestPosition);

        Destroy(gameObject);
    }

    private Vector3 FindBestGooSpawnPosition()
    {
        Vector3 bestPosition = transform.position;
        int bestHitCount = 0;

        Vector3 cameraPosition = Camera.main.transform.position;

        for (int i = 0; i < _raycastCount; i++)
        {
            Vector3 direction = Random.onUnitSphere;

            direction = Vector3.Reflect(direction, transform.position - cameraPosition).normalized;

            Ray ray = new(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance))
            {
                int hitCount = 1;

                for (int j = 0; j < _raycastCount; j++)
                {
                    direction = Random.onUnitSphere;
                    ray = new Ray(hit.point, direction);

                    if (Physics.Raycast(ray, _raycastDistance))
                    {
                        hitCount++;
                    }
                }

                if (hitCount > bestHitCount)
                {
                    bestHitCount = hitCount;
                    bestPosition = hit.point;
                }
            }
        }

        return bestPosition;
    }

    private IEnumerator AnimateScale()
    {
        float elapsed = 0f;
        Vector3 targetScale = _originalScale * _animationScale;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _animationDuration;

            t = _animationCurve.Evaluate(t);

            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);

            yield return null;
        }

        transform.localScale = _originalScale;
    }
}
