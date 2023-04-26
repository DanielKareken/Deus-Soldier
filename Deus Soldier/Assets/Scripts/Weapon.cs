using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Refs")]
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] protected Animator _animRef;
    [SerializeField] protected GameObject _weaponVisual;
    //[SerializeField] protected GameObject _muzzleFlash;
    //[SerializeField] protected AudioSource _shootSound, _reloadSound;
    //[SerializeField] protected GameObject _impactEffect;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _barrelTipTransform;
    [SerializeField] private Transform _aimDownSightTransform;
    [SerializeField] private Transform _hipfireTransform;
    [SerializeField] protected Transform _hipfireAimTransform;

    //[Header("Weapon Stats")]
    //[SerializeField] protected int _maxMagSize;
    //[SerializeField] protected float _damage;
    //[SerializeField] protected float _range;
    //[SerializeField] protected float _firerate;   
    //[SerializeField] protected float _revSpeed;
    //[SerializeField] protected float _hipfireAcc; //hipfire accuracy: 100 is precise, 0 is inaccurate
    //[SerializeField] protected int _projectileCountPerShot = 1; //for shotguns
    //[SerializeField] protected float _recoilForce;
    //[SerializeField] protected bool _allowADS;   
    //[SerializeField] protected LayerMask _aimLayers;

    //[Header("Customizeable")]
    //[SerializeField] protected WeaponProjectileType _projectileType;
    //[SerializeField] protected WeaponFireType _fireType;

    //other refs
    Camera _fpsCam;
    Rigidbody _playerRB;

    protected int _ammoInMag; //ammo currently in loaded magazine
    protected float _timeToFire; //time left until weapon can fire again
    protected bool _allowFire = true; //used to prevent firing other than firerate
    protected float _revTime = 0f;
    protected bool _firing = false;
    bool _aiming;
    float _aimStartTime;

    void Start()
    {
        if (GetComponent<Animator>())
            _animRef = GetComponent<Animator>();

        _ammoInMag = _weaponData.maxMagSize;
        _aiming = false;
    }

    void Update()
    {
        Timers();

        //update aiming
        float t = (Time.time - _aimStartTime) / _weaponData.adsSpeed;

        if (_weaponData.fireType != WeaponFireType.MELEE) //melee weapons cant aim
        {
            if (_aiming)
                _weaponVisual.transform.position = Vector3.Slerp(transform.position, _aimDownSightTransform.position, t);
            if (!_aiming)
                _weaponVisual.transform.position = Vector3.Slerp(transform.position, _hipfireTransform.position, t);
        }
    }

    //handles input related to equipped weapon
    public void PlayerInputWeaponAction()
    {
        //shooting
        if(_allowFire) {
            if ((_weaponData.fireType == WeaponFireType.SEMI || _weaponData.fireType == WeaponFireType.MELEE) && Input.GetMouseButtonDown(0))
                Shoot();
            else if (_weaponData.fireType == WeaponFireType.AUTOMATIC && Input.GetMouseButton(0))
                Shoot();
            else
                _firing = false;
        }

        //aiming
        if (Input.GetMouseButton(1))
        {
            if (!_aiming)
            {
                _aiming = true;
                _aimStartTime = Time.time;
            }
        }            
        else
        {
            if(_aiming)
            {
                _aiming = false;
                _aimStartTime = Time.time;
            }
        }          

        //reloading
        if (Input.GetKeyDown(KeyCode.R))
            ReloadWeapon();
    }

    protected void Timers()
    {
        //fire rate
        if (_timeToFire > 0)
            _timeToFire -= Time.deltaTime;

        if (!_firing && _revTime > 0)
            _revTime -= Time.deltaTime;
    }

    protected void Shoot()
    {
        //check if there is ammo left
        if (_ammoInMag <= 0)
            return;

        _firing = true;

        //rev up
        if (_revTime < _weaponData.revSpeed)
            _revTime += Time.deltaTime;
        else if(_timeToFire <= 0)
        {
            _timeToFire = _weaponData.firerate; //reset fire rate

            //FIRE TYPE: MELEE
            if (_weaponData.fireType == WeaponFireType.MELEE)
            {
                if (_animRef)
                    _animRef.SetTrigger("Attack");
            }
            //FIRE TYPE: SEMI and AUTO
            else
            {
                if (_animRef)
                    _animRef.SetTrigger("Shoot");

                //raycast fire type
                //if (_projectileType == WeaponProjectileType.RAYCAST)
                //{
                //RaycastHit _aimHit;

                //if (Physics.Raycast(transform.position, transform.forward, out _aimHit, 99, _aimLayers))
                //{
                //    Debug.Log("Raycast hit: " + _aimHit.collider.name);

                //    //deal damage
                //    if (_aimHit.collider.GetComponent<Health>())
                //        _aimHit.collider.GetComponent<Health>().DealDamage(_damage);

                //    //Debug.Log("hit Object: " + _aimRaycastDown.transform.name);
                //    if (_impactEffect)
                //        Instantiate(_impactEffect, _aimHit.point, transform.rotation);
                //}
                //}
                //projectile fire type
                //else if(_projectileType == WeaponProjectileType.PROJECTILE)
                //{
                //    GameObject projectile = Instantiate(_bulletPrefab, _barrelTipTransform.position, _barrelTipTransform.rotation * Quaternion.Euler(0f,0,0));
                //    projectile.GetComponent<Projectile>().InitializeProjectile(_damage); //pass relevant vars to projectile
                //    pro.GetComponent<Projectile>().LaunchProjectile(transform.up);
                //}

                //float totalSpread = _weaponData.hipfireAcc / _weaponData.projectileCountPerShot;
                //Vector3 angle = _fpsCam.transform.eulerAngles;
                //Quaternion spreadRotation = Quaternion.identity;

                //create one projectile per count
                for (int i = 0; i < _weaponData.projectileCountPerShot; i++)
                {
                    Ray ray = _fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    
                    //calc direction with spread 
                    Vector3 fwdTrans = _fpsCam.transform.up;
                    float spread = 180 - _weaponData.hipfireAcc;
                    fwdTrans += _fpsCam.transform.TransformDirection(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread)));

                    RaycastHit aimHit;
                    Vector3 targetPoint;

                    //raycast
                    //if (Physics.Raycast(_fpsCam.transform.position, _hipfireAimTransform.position, out aimHit, _weaponData.aimLayers))
                    bool hasHitTarget = //Physics.Raycast(ray, out aimHit, _weaponData.aimLayers);
                    Physics.Raycast(_fpsCam.transform.position, fwdTrans, out aimHit, _weaponData.range, _weaponData.aimLayers);

                    //if hit
                    if (hasHitTarget)
                    {
                        targetPoint = aimHit.point;
                        Debug.Log("Hit: " + aimHit.transform.gameObject);
                    }
                    //if no hit
                    else
                    {
                        targetPoint = ray.GetPoint(_weaponData.range);
                        Debug.Log("No hit");
                    }

                    //calculate direction of projectile normally
                    Vector3 dirBeforeSpread = targetPoint - _barrelTipTransform.position;

                    //calculate direction with spread
                    Vector3 dirWithSpread = dirBeforeSpread;

                    //apply hipfire spread
                    if (!_aiming)
                    {
                        //float spread = 180 - _weaponData.hipfireAcc;
                        //float xSpread = Random.Range(-spread, spread);
                        //float ySpread = Random.Range(-spread, spread);
                        //float zSpread = Random.Range(-spread, spread);
                        //dirWithSpread += new Vector3(xSpread, ySpread, zSpread);

                        // Calculate angle of this bullet
                        //float spreadA = totalSpread * (i + 1);
                        //float spreadB = _weaponData.hipfireAcc / 2.0f;
                        //float spread = spreadB - spreadA + totalSpread / 2;

                        //spreadRotation = Quaternion.Euler(angle * spread);
                        //Debug.Log("Spread: " + spreadRotation);

                        //Vector3 targetPos = _fpsCam.transform.position + _fpsCam.transform.forward * _weaponData.range;
                        //targetPos = new Vector3(
                        //    targetPos.x + Random.Range(-spread, spread),
                        //    targetPos.y + Random.Range(-spread, spread),
                        //    targetPos.z + Random.Range(-spread, spread)
                        //);

                        //dirWithSpread = targetPos - _fpsCam.transform.position;
                    }                      

                    //spawn projectile and adjust necessary details
                    GameObject projectile = Instantiate(_bulletPrefab, _barrelTipTransform.position, Quaternion.identity);
                    projectile.transform.forward = dirWithSpread.normalized; //rotate to proper orientation
                    projectile.GetComponent<Projectile>().SetupProjectile(_weaponData, _weaponData.maxClusterTimes); //pass relevant vars to projectile
                    projectile.GetComponent<Projectile>().LaunchProjectile(_fpsCam.transform, dirWithSpread.normalized); //launch
                }

                _ammoInMag--;
            }     
        }
    }

    public void ReloadAnimation()
    {
        //anim.SetTrigger(AnimationTags.RELOAD_TRIGGER);
    }

    void ToggleReloading()
    {
        /*if (PlayerShoot.reloading == true)
        {
            currentClipSize = maxClipSize;
            PlayerShoot.reloading = false;
        }

        else
        {
            PlayerShoot.reloading = true;
        }*/
    }

    void AimWeapon()
    {
        if (_weaponData.allowADS)
        {
            if (!_aiming)
                _aiming = true;
            else
                _aiming = false;

            if (_animRef)
                _animRef.SetBool("Aim", _aiming);
        }
    }

    void ReloadWeapon()
    {
        _ammoInMag = _weaponData.maxMagSize;
    }

    /*void Turn_On_MuzzleFlash()
    {
        _muzzleFlash.SetActive(true);
    }

    void Turn_Off_MuzzleFlash()
    {
        _muzzleFlash.SetActive(false);
    }

    void Play_ShootSound()
    {
        _shootSound.Play();
    }

    void Play_ReloadSound()
    {
        _reloadSound.Play();
    }*/

    public void InitializeRefs(Rigidbody rb, Camera cam)
    {
        _playerRB = rb;
        _fpsCam = cam;
    }
}
