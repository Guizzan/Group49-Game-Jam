using System.Collections;
using UnityEngine;
using Guizzan.Input.GIM;
using Guizzan.Input.GIM.Guns;


public abstract class BaseGun : PoseOverride, IDropable, IGuizzanInputManager<GunInputs>, ICollectable
{
    public string GunName;
    public int MaxAmmo = 12;
    public bool isAutomatic = false;
    public float ShootingTime = 1;
    public float ReloadingTime = 1;
    public float ArtifactsDestroyTime = 30;
    public float AmmoForce = 1;
    public float CasingForce = 1;
    public float sliderLimit = 1;
    public float sliderSpeed = 1;
    public float MinDamage;
    public float MaxDamage;

    public Vector3 RecoilConstraints;
    public float RecoilCounterForce;
    public float RecoilForce;

    public GameObject AmmoPrefab;
    public GameObject AmmoCasingPrefab;
    public GameObject Magazine;
    public Transform Nozzle;
    public Transform CasingPoint;
    public Transform Slider;


    [SerializeField]
    private int _currentAmmo = 12;
    private IEnumerator _curRoutine;

    private Vector3 _initialSlider;
    public Vector3 _sliderTarget;
    private Quaternion _initialRot;
    public Quaternion angularRecoil;

    [ButtonInvoke(nameof(Shoot), InputValue.Down)] public bool testShoot;
    [ButtonInvoke(nameof(Shoot), InputValue.Up)] public bool testStopShoot;
    [ButtonInvoke(nameof(Reload))] public bool testReload;

    public GameObject dropPrefab;
    public GameObject DropPrefab()
    {
        return dropPrefab;
    }
    private void Start()
    {
        _initialRot = transform.localRotation;
        _initialSlider = Slider.transform.localPosition;
    }
    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _initialRot * angularRecoil, Time.deltaTime * RecoilForce);
        angularRecoil = Quaternion.Lerp(angularRecoil, Quaternion.identity, Time.deltaTime * RecoilCounterForce);
        Slider.localPosition = Vector3.MoveTowards(Slider.localPosition, _sliderTarget, Time.deltaTime * sliderSpeed);
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
                OnGunStateChanged(GunInputs.Shoot, InputValue.Up);
                if (_currentAmmo <= 0)
                {
                    _sliderTarget = _initialSlider + (Vector3.forward * sliderLimit);
                }
                else
                {
                    _sliderTarget = _initialSlider;
                }
                _curRoutine = null;
            }
        }
    }

    private IEnumerator ShootingRoutine()
    {
        if (_currentAmmo <= 0)
        {
            OnGunStateChanged(GunInputs.AmmoFinished, InputValue.Down);
            _curRoutine = null;
            _sliderTarget = _initialSlider + (Vector3.forward * sliderLimit);
            yield break;
        }
        OnGunStateChanged(GunInputs.Shoot, InputValue.Down);
        _currentAmmo -= 1;

        GameObject casing = Instantiate(AmmoCasingPrefab);
        casing.transform.position = CasingPoint.position;
        casing.GetComponent<Rigidbody>().AddForce(Vector3.right * CasingForce);
        StartCoroutine(DelayedDestroy(casing, ArtifactsDestroyTime));
        Nozzle.GetComponent<ParticleSystem>().Play();

        GameObject ammo = Instantiate(AmmoPrefab);
        ammo.transform.position = Nozzle.position;
        ammo.GetComponent<Rigidbody>().AddForce(transform.forward * AmmoForce);
        ammo.GetComponent<BulletDamage>().MaxDamage = MaxDamage;
        ammo.GetComponent<BulletDamage>().MinDamage = MinDamage;
        StartCoroutine(DelayedDestroy(ammo, 5));

        _sliderTarget = _initialSlider + (Vector3.forward * sliderLimit);
        AddRecoil();

        yield return new WaitForSeconds(ShootingTime / 2);
        _sliderTarget = _initialSlider;
        yield return new WaitForSeconds(ShootingTime / 2);
        if (isAutomatic)
        {
            _curRoutine = ShootingRoutine();
            StartCoroutine(_curRoutine);
            yield break;
        }
        _curRoutine = null;
    }
    private void AddRecoil()
    {
        float randomAngleX = UnityEngine.Random.Range(-RecoilConstraints.x, RecoilConstraints.x);
        float randomAngleY = UnityEngine.Random.Range(-RecoilConstraints.y, RecoilConstraints.y);
        float randomAngleZ = UnityEngine.Random.Range(-RecoilConstraints.z, RecoilConstraints.z);
        angularRecoil = Quaternion.Euler(randomAngleX, randomAngleY, randomAngleZ);
    }
    private void Reload()
    {
        _curRoutine = ReloadingRoutine();
        StartCoroutine(_curRoutine);
    }

    private IEnumerator ReloadingRoutine()
    {
        if (_currentAmmo == MaxAmmo) yield break;
        GameObject instance = Instantiate(Magazine, transform);
        Magazine.SetActive(false);
        instance.transform.position = Magazine.transform.position;
        instance.transform.rotation = Magazine.transform.rotation;
        instance.transform.parent = null;
        instance.AddComponent<Rigidbody>();
        StartCoroutine(DelayedDestroy(instance, ArtifactsDestroyTime));
        OnGunStateChanged(GunInputs.Reload, InputValue.Down);
        yield return new WaitForSeconds(ReloadingTime);
        _currentAmmo = MaxAmmo;
        _curRoutine = null;
        _sliderTarget = _initialSlider;
        Magazine.SetActive(true);
    }

    private IEnumerator DelayedDestroy(GameObject instance, float time)
    {
        yield return new WaitForSeconds(time);
        if (instance != null)
            Destroy(instance);
    }
    public abstract void OnGunStateChanged(GunInputs state, InputValue value);

    public string GetName()
    {
        return GunName;
    }
}
