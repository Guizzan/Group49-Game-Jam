using UnityEditor.Rendering;
using UnityEngine;
using Guizzan.Input.GIM;

public class PickUpItem : MonoBehaviour
{
    public GameObject GunsParrent;
    public Transform DropPos;
    public float DropForce;
    public static GameObject _currentItem;
    public float ThrowOffset;
    GameObject GetItem(string name)
    {
        foreach (Transform item in GunsParrent.transform)
        {
            if (item.GetComponent<ICollectable>().GetName() == name)
            {
                return item.gameObject;
            }
        }
        return null;
    }

    public void GetItem()
    {
        if (PlayerRaycaster.Selection == null) return;

        if (_currentItem != null)
        {
            DropItem();
        }

        GameObject Item = GetItem(PlayerRaycaster.Selection.GetComponent<ICollectable>().GetName());
        if (Item == null) return;

        foreach (Transform item in GunsParrent.transform)
        {
            item.gameObject.SetActive(false);
        }
        Destroy(PlayerRaycaster.Selection);
        Item.GetComponent<PoseOverride>().SetIK(true);
        Item.SetActive(true);
        _currentItem = Item;
    }

    public void DropItem(float throwMultiplier = 1)
    {
        if (_currentItem == null) return;
        GameObject Instance = Instantiate(_currentItem.GetComponent<IDropable>().DropPrefab());
        Instance.transform.position = DropPos.position;
        Instance.transform.rotation = DropPos.rotation;
        Vector3 force = (PlayerRaycaster.Ray.direction + transform.right * ThrowOffset) * DropForce * throwMultiplier;
        Instance.GetComponent<Rigidbody>().AddForce(force);
        Instance.GetComponent<Rigidbody>().velocity += GetComponent<CharacterController>().velocity;
        foreach (Transform item in GunsParrent.transform)
        {
            item.gameObject.SetActive(false);
        }
        _currentItem.GetComponent<PoseOverride>().SetIK(false);
        SoundManager.PlaySound("Throw");
        SoundManager.PlaySoundOnCollision(Instance, "BrickHit", "Enemy");
        SoundManager.PlaySoundOnCollision(Instance, "Drop");
        _currentItem = null;
    }

}

public interface IDropable
{
    public GameObject DropPrefab();
}
