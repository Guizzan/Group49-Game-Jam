using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHealth : LivingEntity
{
    public GameObject DeathCam;
    public override void Death()
    {
        PlayerController player = GetComponent<PlayerController>();
        player.AimCamera.SetActive(false);
        player.Camera.SetActive(false);
        player.OrbitCam.SetActive(false);
        Destroy(player);
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<PlayerSoundManager>());
        Destroy(GetComponent<PlayerRaycaster>());
        Destroy(GetComponent<PickUpItem>());
        Destroy(GetComponent<CharacterController>());
        Destroy(GetComponent<MovingPlatformHandler>());
        SoundManager.PlaySoundOnCollision(gameObject, "Drop");
        DeathCam.SetActive(true);
        Destroy(this);
        SoundManager.PlaySound("FailSound");
    }

    public override void GotHit()
    {
        SoundManager.PlaySound("HamsterHit", transform.position);
    }
}
