using UnityEngine;

public class GooSpawner : MonoBehaviour
{
    [SerializeField]
    private GooPool _gooPool;

    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private int _numberOfGoo = 150;

    [SerializeField]
    private float _spreadAngle = 14f;

    public void SpawnGoo(Vector3 spawnPoint, Vector3 endPoint)
    {
        Vector3 direction = (endPoint - spawnPoint).normalized;

        for (int i = 0; i < _numberOfGoo; i++)
        {
            Vector3 variation =
                Quaternion.AngleAxis(Random.Range(-_spreadAngle, _spreadAngle), Vector3.up)
                * Quaternion.AngleAxis(Random.Range(-_spreadAngle, _spreadAngle), Vector3.right)
                * direction;

            GameObject gooGO = _gooPool.GetGoo(spawnPoint, Quaternion.identity);

            Rigidbody rb = gooGO.GetComponent<Rigidbody>();
            rb.velocity = variation * _speed;
        }
    }
}
