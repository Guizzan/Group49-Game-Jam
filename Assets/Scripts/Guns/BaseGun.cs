using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizzan.Input.GIM;
using Guizzan.Input.GIM.Guns;
using UnityEngine.UIElements;
using System;
using TreeEditor;

public abstract class BaseGun : MonoBehaviour
{
    public int MaxAmmo = 12;
    public bool isAutomatic = false;
    public float ShootingTime = 1;
    public float ReloadingTime = 1;
    public float ArtifactsDestroyTime = 30;
    public float AmmoForce = 1;
    public float CasingForce = 1;
    public float RecoilCounterForce;  
    public float maxAngleDifference = 30f;
    public float RecoilForce;
    public GameObject AmmoPrefab;
    public GameObject AmmoCasingPrefab;
    public GameObject Magazine;
    public Transform Nozzle;
    public Transform CasingPoint;


    [SerializeField]
    private int _currentAmmo = 12;
    private IEnumerator _curRoutine;
    private Quaternion _initialRot;
    private Vector3 _initialpos;
    [ButtonInvoke(nameof(Shoot), InputValue.Down)] public bool testShoot;
    [ButtonInvoke(nameof(Shoot), InputValue.Up)] public bool testStopShoot;
    [ButtonInvoke(nameof(Reload))] public bool testReload;

    private void Start()
    {
        _initialRot = transform.localRotation;
        _initialpos = transform.localPosition;
    }

    public void SetInput(GunInputs Input, InputValue value)
    {
        switch (Input)
        {
            case GunInputs.Shoot:
                Shoot(value);
                break;
            case GunInputs.Reload:
                Reload();
                break;
        }
    }

    private void Shoot(InputValue value)
    {
        if (value == InputValue.Down)
        {
            if (_curRoutine == null)
            {
                _curRoutine = ShootingRoutine();
                StartCoroutine(_curRoutine);
            }
        }
        else if (value == InputValue.Up)
        {
            if (_curRoutine != null)
            {
                StopCoroutine(_curRoutine);
                _curRoutine = null;
            }
        }
    }

    private IEnumerator ShootingRoutine()
    {
        if (_currentAmmo <= 0)
        {
            _curRoutine = null;
            yield break;
        }
        OnGunStateChanged(GunInputs.Shoot);
        _currentAmmo -= 1;

        GameObject casing = Instantiate(AmmoCasingPrefab);
        casing.transform.position = CasingPoint.position;
        casing.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * CasingForce);
        StartCoroutine(DelayedDestroy(casing, ArtifactsDestroyTime));
        Nozzle.GetComponent<ParticleSystem>().Play();
        Vector3 force = ((Vector3.up * UnityEngine.Random.Range(0.5f, 1)) + (Vector3.right * UnityEngine.Random.Range(-1, 1))) * RecoilForce;
        GetComponent<Rigidbody>().AddForceAtPosition(force, Nozzle.transform.position);
        GameObject ammo = Instantiate(AmmoCasingPrefab);
        ammo.transform.position = Nozzle.position;
        ammo.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * AmmoForce);
        StartCoroutine(DelayedDestroy(ammo, 1));

        yield return new WaitForSeconds(ShootingTime);
        if (isAutomatic)
        {
            _curRoutine = ShootingRoutine();
            StartCoroutine(_curRoutine);
            yield break;
        }
        _curRoutine = null;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _initialRot, Time.deltaTime * RecoilCounterForce);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _initialpos, Time.deltaTime * RecoilCounterForce);
    }


    private void Reload()
    {
        _curRoutine = ReloadingRoutine();
        StartCoroutine(_curRoutine);
    }

    private IEnumerator ReloadingRoutine()
    {
        if (_currentAmmo == MaxAmmo) yield break;
        GameObject instance = Instantiate(Magazine);
        Magazine.SetActive(false);
        instance.transform.position = Magazine.transform.position;
        instance.AddComponent<Rigidbody>();
        StartCoroutine(DelayedDestroy(instance, ArtifactsDestroyTime));
        OnGunStateChanged(GunInputs.Reload);
        yield return new WaitForSeconds(ReloadingTime);
        _currentAmmo = MaxAmmo;
        _curRoutine = null;
        Magazine.SetActive(true);
    }

    private IEnumerator DelayedDestroy(GameObject instance, float time)
    {
        yield return new WaitForSeconds(time);
        if (instance != null)
            Destroy(instance);
    }
    public abstract void OnGunStateChanged(GunInputs state);
}
