using UnityEngine;
using System;

public interface IProjectile
{
    void Initialize(Action<GameObject> returnToPool, float size);
    void Launch(Vector3 position, Vector3 direction, float speed);
}
