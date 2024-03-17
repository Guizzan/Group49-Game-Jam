using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDamageRecieve : MonoBehaviour, IDamageable
{
    public GameObject door;
    public GameObject Fractures;
    public float radius;
    public float force;
    public void GetDamage(Vector3 position)
    {
        door.SetActive(false);
        Fractures.SetActive(true);
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (rb != null)
                    rb.isKinematic = false;
                rb.isKinematic = false;
                rb.AddExplosionForce(force, explosionPos, radius, 3.0F);
            }
        }
    }
}
