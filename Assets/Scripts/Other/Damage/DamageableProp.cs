using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableProp : MonoBehaviour, IDamageable
{
    public float radius = 0.5F;
    public float power = 2.0F;

    public void GetDamage()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        { 
            if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (rb != null)
                    rb.isKinematic = false;
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            }
        }
    }
}
