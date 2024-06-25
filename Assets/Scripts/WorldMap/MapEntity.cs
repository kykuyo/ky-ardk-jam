using Niantic.Lightship.Maps.ObjectPools;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MapEntity : MonoBehaviour
{
    public PooledObject<GameObject> obj;
    public float maxInteractionDistance = 5f;

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(
                    player.transform.position,
                    transform.position
                );
                if (distanceToPlayer <= maxInteractionDistance)
                {
                    OnPlayerInteract();
                }
            }
        }
    }

    protected abstract void OnPlayerInteract();
}
