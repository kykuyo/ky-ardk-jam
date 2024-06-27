using System.Collections;

using UnityEngine;

public class PlayerGoo : Goo, IProjectile
{
    private void OnEnable()
    {
        InitGoo();
    }

    public void Launch(Vector3 position, Vector3 direction, float speed)
    {
        transform.position = position;
        transform.localScale = _initialScale;
        gameObject.SetActive(true);
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = direction * speed;
            rb.angularVelocity = Vector3.zero;
        }
        StartCoroutine(DisposeAfterSeconds(3));
    }

    private void InitGoo()
    {
        transform.localScale = _initialScale;
        HasCollided = false;
        GetComponent<Rigidbody>().isKinematic = false;

        if (TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = true;
        }

        BoxCollider[] boxColliders = GetComponents<BoxCollider>();
        foreach (var boxCollider in boxColliders)
        {
            boxCollider.enabled = false;
        }

        gameObject.layer = LayerMask.NameToLayer("Goo");
    }

    private IEnumerator DisposeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!HasCollided)
        {
            _returnToPool?.Invoke(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerGoo otherGoo))
        {
            if (otherGoo.HasCollided)
            {
                return;
            }
            otherGoo._returnToPool?.Invoke(collision.gameObject);

            return;
        }

        if (collision.gameObject.TryGetComponent(out Bubble _))
        {
            return;
        }

        Deform(collision);
    }

    private void Deform(Collision collision)
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        transform.localScale = _deformScale;

        transform.position = collision.contacts[0].point;

        Vector3 surfaceNormal = collision.contacts[0].normal;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        transform.rotation = rotation;

        if (TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        BoxCollider[] boxColliders = GetComponents<BoxCollider>();
        foreach (var boxCollider in boxColliders)
        {
            boxCollider.enabled = true;
            boxCollider.size = new Vector3(
                boxCollider.size.x,
                boxCollider.size.y,
                boxCollider.size.z
            );
        }

        gameObject.layer = LayerMask.NameToLayer("Metaballs");

        HasCollided = true;
    }
}
