using System.Collections.Generic;
using UnityEngine;

public class GooPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _gooPrefab;

    [SerializeField]
    private int _initialPoolSize = 50;

    private readonly Queue<GameObject> _pool = new();

    private void Start()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject goo = Instantiate(_gooPrefab);
            goo.SetActive(false);
            _pool.Enqueue(goo);
        }
    }

    public GameObject GetGoo(Vector3 position, Quaternion rotation)
    {
        if (_pool.Count > 0)
        {
            GameObject goo = _pool.Dequeue();
            goo.transform.position = position;
            goo.transform.rotation = rotation;
            goo.SetActive(true);

            return goo;
        }
        else
        {
            GameObject goo = Instantiate(_gooPrefab, position, rotation);
            return goo;
        }
    }

    public void ReturnGoo(GameObject goo)
    {
        goo.SetActive(false);
        _pool.Enqueue(goo);
    }
}
