using Niantic.Lightship.Maps.ObjectPools;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MapEntity : MonoBehaviour
{
    public PooledObject<GameObject> obj;

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            OnPlayerInteract();
        }
    }

    protected abstract void OnPlayerInteract();
}