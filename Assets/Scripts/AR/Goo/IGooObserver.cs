using UnityEngine;

public interface IGooObserver
{
    void OnGooCreated(GameObject goo);
    void OnGooDestroyed(GameObject goo);
}
