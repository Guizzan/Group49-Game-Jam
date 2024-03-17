using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
public abstract class LivingEntity : MonoBehaviour
{
    public float Health = 100;
    public Image healthSlider;

    public void GetDamage(float amount)
    {
        Health -= amount;
        healthSlider.fillAmount = Health;
        GotHit();
        if (Health <= 0)
        {
            Death();
        }
    }

    public abstract void GotHit();
    public abstract void Death();

}
