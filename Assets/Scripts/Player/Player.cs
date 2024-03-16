using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Guizzan.Player;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static bool enableMove = true;
    public static bool enableLook = true;
    public static bool enablePhysics = true;
    public static bool canAttack = false;
    public static bool enableIK = false;
    public SkinnedMeshRenderer[] Mesh;
    public Transform HeadRotator;
    public Transform FpsCamPos;
    public Transform DropPosition;
    public Transform Hand;
    public GameObject FirstPersonCam;
    public GameObject ThirdPersonCam;
    public List<GameObject> Body;
    public GameObject FpsHands;
    public GameObject FpsLegs;
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
        enableIK = true;
    }
    public void SetVisibility(VisibilityModes mode)
    {
        switch (mode)
        {
            case VisibilityModes.FirstPerson:
                foreach (SkinnedMeshRenderer item in Mesh) item.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                FpsHands.SetActive(true);
                FpsLegs.SetActive(true);
                break;
            case VisibilityModes.ThirdPerson:
                foreach (SkinnedMeshRenderer item in Mesh) item.shadowCastingMode = ShadowCastingMode.On;
                FpsHands.SetActive(false);
                FpsLegs.SetActive(false);
                foreach (GameObject item in Body) item.SetActive(true);
                break;
            case VisibilityModes.None:
                foreach (SkinnedMeshRenderer item in Mesh) item.shadowCastingMode = ShadowCastingMode.Off;
                FpsHands.SetActive(false);
                FpsLegs.SetActive(false);
                foreach (GameObject item in Body) item.SetActive(false);
                break;
        }
    }
}
