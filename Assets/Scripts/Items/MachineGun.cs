using Guizzan.Input.GIM.Guns;
using System.Collections;
using UnityEngine;
using Guizzan.Extensions;
using Guizzan.Input.GIM;
using Cinemachine;

public class MachineGun : BaseGun
{
    public float PlayerForce;
    public float MouseRecoil;
    public float Noise = 2;
    public float NoiseLerpSpeed = 1;
    private PlayerController _player;
    private CinemachineBasicMultiChannelPerlin _noise;
    private IEnumerator lerper;
    private void Awake()
    {
        _player = transform.GetTopMostParrent().GetComponent<PlayerController>();
        _noise = _player.AimCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public override void OnGunStateChanged(GunInputs state, InputValue value)
    {
        switch (state)
        {
            case GunInputs.Shoot:
                if (value == InputValue.Down)
                {
                    _player.AddForce(-transform.forward * PlayerForce);
                    _player._appliedMouseDelta = new(_player._appliedMouseDelta.x + UnityEngine.Random.Range(0, MouseRecoil), _player._appliedMouseDelta.y + UnityEngine.Random.Range(0, MouseRecoil));
                    _noise.m_AmplitudeGain = Noise;
                    SoundManager.PlaySound("MachineGun");
                }
                else if (value == InputValue.Up)
                {
                    SetNoise(0);
                }
                break;
            case GunInputs.Reload:
                SoundManager.PlaySound("Reload");
                break;
            case GunInputs.AmmoFinished:
                SoundManager.PlaySound("EmptyGun");
                SetNoise(0);
                break;
        }
    }
    private void OnDisable()
    {
        _noise.m_AmplitudeGain = 0;
    }

    void SetNoise(float value)
    {
        if (lerper != null)
        {
            StopCoroutine(lerper);
        }
        lerper = Lerper(value);
        StartCoroutine(lerper);
    }

    private IEnumerator Lerper(float value)
    {
        while (Mathf.Abs(_noise.m_AmplitudeGain - value) >= 0.1f)
        {
            _noise.m_AmplitudeGain = Mathf.MoveTowards(_noise.m_AmplitudeGain, value, Time.deltaTime * NoiseLerpSpeed);
            yield return null;
        }
        _noise.m_AmplitudeGain = value;
        lerper = null;
    }
}
