using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizzan.Extensions;
public class BulletDamage : MonoBehaviour
{
    public float MaxDamage;
    public float MinDamage;
    public LayerMask layer;

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (layer.ContainsLayer(other.gameObject.layer))
        {
            if (other.TryGetComponent(out LivingEntity entity))
            {
                float amount = Random.Range(MinDamage, MaxDamage);
                entity.GetDamage(amount);
            }
            else if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.GetDamage();
            }
        }
    }

}
