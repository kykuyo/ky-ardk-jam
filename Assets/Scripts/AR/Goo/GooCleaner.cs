using UnityEngine;

public class GooCleaner : MonoBehaviour
{
    [SerializeField]
    private float _sphereRadius = 10;

    [SerializeField]
    private float _gooLifeDecreaseSpeed = 10f;

    [SerializeField]
    private LayerMask _layerMask;

    private Camera _mainCamera;

    private readonly RaycastHit[] hits = new RaycastHit[10];

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            int hitCount = Physics.SphereCastNonAlloc(
                ray,
                _sphereRadius,
                hits,
                Mathf.Infinity,
                _layerMask.value
            );

            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].transform.TryGetComponent<Goo>(out var goo))
                {
                    goo.DecreaseLife(_gooLifeDecreaseSpeed * Time.deltaTime);
                }
            }
        }
    }
}
