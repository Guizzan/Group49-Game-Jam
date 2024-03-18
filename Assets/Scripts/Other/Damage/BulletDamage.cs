using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizzan.Extensions;
public class BulletDamage : MonoBehaviour
{
    public float MaxDamage;
    public float MinDamage;
    public LayerMask layer;

    private void OnTriggerEnter(Collider col)
    {
        print(col.gameObject.layer);
        if (layer.ContainsLayer(col.gameObject.layer))
        {
            if (col.gameObject.TryGetComponent(out LivingEntity entity))
            {
                float amount = Random.Range(MinDamage, MaxDamage);
                entity.GetDamage(amount);
            }
            else if (col.transform.GetTopMostParrent().TryGetComponent(out IDamageable damageable))
            {
                damageable.GetDamage(col.transform.position);
            }
        }
    }

}
