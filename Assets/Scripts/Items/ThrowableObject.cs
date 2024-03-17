using Guizzan.Input.GIM;
using Guizzan.Input.GIM.Guns;
using UnityEngine;
using Guizzan.Extensions;

public class ThrowableObject : PoseOverride, IGuizzanInputManager<GunInputs>, IDropable, ICollectable
{
    public string ItemName;
    public float ThrowMultiplier = 2;
    public GameObject dropPrefab;
    public GameObject DropPrefab()
    {
        return dropPrefab;
    }

    public string GetName()
    {
        return ItemName;
    }

    public void SetInput(GunInputs Input, InputValue value)
    {
        switch (Input)
        {
            case GunInputs.Shoot:
                transform.GetTopMostParrent().GetComponent<PickUpItem>().DropItem(ThrowMultiplier);
                break;
        }
    }
}
