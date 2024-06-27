using System;
using System.Collections;
using UnityEngine;

public class Goo : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _deformScale = new(0.35f, 0.1f, 0.35f);

    [SerializeField]
    protected AnimationCurve _scaleReductionCurve = AnimationCurve.Linear(0, 1, 1, 0);

    protected Vector3 _initialScale;

    protected Action<GameObject> _returnToPool;

    public int Team { get; set; }

    [HideInInspector]
    public bool HasCollided { get; protected set; } = false;

    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    public void Initialize(Action<GameObject> returnToPool, float size)
    {
        _initialScale = new Vector3(size, size, size);
        _returnToPool = returnToPool;
    }

    public void Clean()
    {
        StartCoroutine(ReduceScaleAndClean());
    }

    public void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
    }

    public void ApplyDeform()
    {
        transform.localScale = _deformScale;
    }

    private IEnumerator ReduceScaleAndClean()
    {
        float startTime = Time.time;
        while (transform.localScale.magnitude > 0.01f)
        {
            float t =
                (Time.time - startTime)
                / _scaleReductionCurve.keys[_scaleReductionCurve.length - 1].time;
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.zero,
                _scaleReductionCurve.Evaluate(t)
            );
            yield return null;
        }

        transform.localScale = Vector3.zero;

        if (_returnToPool != null)
        {
            _returnToPool?.Invoke(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
