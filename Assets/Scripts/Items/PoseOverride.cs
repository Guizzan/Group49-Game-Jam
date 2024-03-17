using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Guizzan.Extensions;
public class PoseOverride : MonoBehaviour
{
    public Transform RightHandPos;
    public Transform LeftHandPos;
    public void SetIK(bool enable)
    {
        PlayerController _player = transform.GetTopMostParrent().GetComponent<PlayerController>();
        if (enable)
        {
            _player.rightHandTarget = RightHandPos;
            _player.leftHandTarget = LeftHandPos;
        }
        _player.enableIK = enable;
    }
}
