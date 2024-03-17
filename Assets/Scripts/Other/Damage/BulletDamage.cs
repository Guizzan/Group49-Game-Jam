using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float radius;
    private bool damageDealt = false;

    private void Update()
    {
        if (damageDealt)
            return;

        Vector3 damagePos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(damagePos, radius);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (damageable != null)
                    damageable.GetDamage();
                damageDealt = true;
            }
        }
    }
}
