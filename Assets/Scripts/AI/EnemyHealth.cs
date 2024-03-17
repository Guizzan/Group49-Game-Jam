using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : LivingEntity
{
    private void Start()
    {
        SoundManager.PlaySound("ZombieIdle", transform);
    }
    public override void Death()
    {
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<PathFinder>());
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<EnemySoundManager>());
        SoundManager.PlaySoundOnCollision(gameObject, "Drop");
        Destroy(this);
    }

    public override void GotHit()
    {
        SoundManager.PlaySound("ZombieHit", transform.position);
    }
}
