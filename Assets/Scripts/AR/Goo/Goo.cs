using System.Collections;
using UnityEngine;

public class Goo : MonoBehaviour
{
    [SerializeField]
    private float _maxLife = 100f;

    [SerializeField]
    private float _deformationAmount = 0.5f;

    private float _currentLife;
    private Vector3 _initialScale;
    private bool _hasCollided = false;
    private GooPool _gooPool;

    private void Awake()
    {
        _initialScale = transform.localScale;
        _gooPool = FindObjectOfType<GooPool>();
    }

    private void OnEnable()
    {
        InitGoo();
    }

    public void InitGoo()
    {
        _currentLife = _maxLife;
        transform.localScale = _initialScale;
        _hasCollided = false;
        GetComponent<Rigidbody>().isKinematic = false;

        if (TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = true;
        }

        if (TryGetComponent(out BoxCollider boxCollider))
        {
            boxCollider.enabled = false;
        }

        StartCoroutine(DisposeAfterSeconds(3));
    }

    private IEnumerator DisposeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!_hasCollided)
        {
            _gooPool.ReturnGoo(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out GooGlobe _))
        {
            return;
        }
        _hasCollided = true;
        Deform(collision);
    }

    private void Deform(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        Vector3 newScale = transform.localScale;
        newScale.y *= 1 - _deformationAmount;

        transform.localScale = newScale;
        transform.position = collision.contacts[0].point;

        Vector3 surfaceNormal = collision.contacts[0].normal;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        transform.rotation = rotation;

        if (TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        if (TryGetComponent(out BoxCollider boxCollider))
        {
            boxCollider.enabled = true;
            boxCollider.size = new Vector3(1, 1 - _deformationAmount, 1);
        }
    }

    public void DecreaseLife(float amount)
    {
        _currentLife -= amount;

        if (_currentLife <= 0 || transform.localScale.x <= _initialScale.x / 3)
        {
            _gooPool.ReturnGoo(gameObject);
        }
        else
        {
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        float scaleFactor = _currentLife / _maxLife;
        transform.localScale = new Vector3(
            transform.localScale.x * scaleFactor,
            transform.localScale.y * scaleFactor,
            transform.localScale.z * scaleFactor
        );
    }
}
