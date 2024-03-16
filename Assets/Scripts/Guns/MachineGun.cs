using Guizzan.Input.GIM.Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : BaseGun
{
    public override void OnGunStateChanged(GunInputs state)
    {
        switch (state)
        {
            case GunInputs.Shoot:
                break;
            case GunInputs.Reload:
                break;
        }
    }
}
