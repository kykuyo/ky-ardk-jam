using System.Collections.Generic;
using UnityEngine;

public class GooPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _gooPrefab;

    [SerializeField]
    private int _initialPoolSize = 50;

    private readonly Queue<GameObject> _pool = new();

    private List<IGooObserver> _observers = new List<IGooObserver>();

    private void Start()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject goo = Instantiate(_gooPrefab);
            goo.SetActive(false);
            _pool.Enqueue(goo);
        }
    }

    public void RegisterObserver(IGooObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnregisterObserver(IGooObserver observer)
    {
        _observers.Remove(observer);
    }

    public GameObject GetGoo(Vector3 position, Quaternion rotation)
    {
        GameObject goo;
        if (_pool.Count > 0)
        {
            goo = _pool.Dequeue();
            goo.transform.position = position;
            goo.transform.rotation = rotation;
            goo.SetActive(true);
        }
        else
        {
            goo = Instantiate(_gooPrefab, position, rotation);
        }

        foreach (var observer in _observers)
        {
            observer.OnGooCreated(goo);
        }

        return goo;
    }

    public void ReturnGoo(GameObject goo)
    {
        goo.SetActive(false);
        _pool.Enqueue(goo);

        foreach (var observer in _observers)
        {
            observer.OnGooDestroyed(goo);
        }
    }
}
