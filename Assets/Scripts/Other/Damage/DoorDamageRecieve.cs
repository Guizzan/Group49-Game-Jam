using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDamageRecieve : MonoBehaviour, IDamageable
{
    public GameObject door;
    public GameObject Fractures;
    public float radius;
    public float force;
    public LayerMask layer;
    public void GetDamage(Vector3 position)
    {
        door.SetActive(false);
        Fractures.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(position, radius,layer);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(force, position, radius, radius);
            }
        }
    }
}
