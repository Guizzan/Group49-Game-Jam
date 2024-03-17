using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedItem : MonoBehaviour, ICollectable
{
    public string ItemName;
    public string GetName()
    {
        return ItemName;
    }
}
