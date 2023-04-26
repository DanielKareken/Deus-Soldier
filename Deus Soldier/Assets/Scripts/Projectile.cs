using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileMaterial
{
    BULLET,
    ENERGY
}

public class Projectile : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject _visual;
    [SerializeField] private GameObject _selfPrefab;

    WeaponData _weaponData;
    Rigidbody _rb;
    Vector3 _projectileVec;
    PhysicMaterial _physicsMat;
    bool _hasHitTarget = false;
    float _lifetime = 10f; //max amount of time this object can live
    float _fuseTimer;
    int _collisionCount; //how many times this projectile has made a collision
    int _clustersLeft; //how many more times this projectile will cluster on hit
    float _splitTimer; //time until auto split shot
    bool _hasSplit; //true after splitting
    float _damageMulti;

    void Update()
    {
         _lifetime -= Time.deltaTime;
        if (_lifetime <= 0)
            Invoke("DestroyMe", 0.05f);

        //fuse timer
        if (_fuseTimer > 0)
            _fuseTimer -= Time.deltaTime;
        else if(_weaponData.isExplosive)
        {
            //Debug.Log("Fuse time up. Exploding...");
            _fuseTimer = _weaponData.maxFuseTime;
            Explode();
            Invoke("DestroyMe", 0.05f);
        }

        if (_splitTimer > 0)
            _splitTimer -= Time.deltaTime;
        else if(!_hasSplit && _weaponData.isSplitShot)
        {
            _hasSplit = true;
            SplitShot();
        }           
    }

    //recieve necessary data on a new instaniation
    public void SetupProjectile(WeaponData weaponData, int numTimesToCluster = 0, float damageMulti = 1f, bool hasSplit = false)
    {
        _weaponData = weaponData;
        _fuseTimer = _weaponData.maxFuseTime;
        _clustersLeft = numTimesToCluster;
        _damageMulti = damageMulti;
        _hasSplit = hasSplit;

        //rigidbody
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _weaponData.useGravity;
        
        //create physics material
        _physicsMat = new PhysicMaterial();
        _physicsMat.bounciness = _weaponData.bounciness;
        _physicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
        _physicsMat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<CapsuleCollider>().material = _physicsMat;
    }

    //apllies force(s) to projectile
    public void LaunchProjectile(Transform origin, Vector3 direction)
    {     
        _rb.AddForce(direction * _weaponData.projectileSpeed, ForceMode.Impulse); //forward force
        _rb.AddForce(origin.up * _weaponData.upwardForce, ForceMode.Impulse); //upward force
    }

    //on collision with object
    void OnHit(GameObject target)
    {
        if (target != null)
        {
            //deal damage
            if (target.GetComponent<Health>() && _weaponData.isDamageOnHit)
                target.GetComponent<Health>().DealDamage(_weaponData.projectileDamage * _damageMulti);
        }

        //if explode on touch
        if(_weaponData.explodeOnTouch)
            Explode();
        
        //impact VFX
        SpawnVFX();
    }
    
    //handles explosion
    void Explode()
    {
        //check if effect is explosive
        if (_weaponData.isExplosive)
        {
            //ensure an explosion effect exists and has the appropriate script
            if (_weaponData.explosiveEffect && _weaponData.explosiveEffect.GetComponent<ExplosiveEffect>())
            {
                //spawn explosion
                GameObject effect = Instantiate(_weaponData.explosiveEffect, transform.position, Quaternion.identity);

                //initialize explosion             
                effect.GetComponent<ExplosiveEffect>().SetupExplosion(_weaponData, _damageMulti, _damageMulti);

                //check if explosion is cluster
                if (_weaponData.isClusterBomb && _clustersLeft > 0)
                    Clustersplode();
            }
            else
                Debug.Log("ERROR: No explosion effect found");
        }       
    }

    //spawn more projectiles on explosion
    void Clustersplode()
    {
        _clustersLeft--;
        //Debug.Log("I am clustering. " + _clustersLeft + " clusters left.");       

        //spawn new projectiles
        for (int i = 0; i < _weaponData.clusterProjectileCount; i++)
        {
            Vector3 clusterOffeset = new Vector3();
            GameObject clusterNade = Instantiate(_selfPrefab, transform.position + clusterOffeset, Quaternion.identity);
            clusterNade.GetComponent<Projectile>().SetupProjectile(_weaponData, _clustersLeft, _damageMulti * 0.5f, _hasSplit);
        }
    }

    void SplitShot()
    {
        //create one projectile per count
        for (int i = 0; i < _weaponData.splitShotCount; i++)
        {
            //Ray ray = _fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit aimHit;
            Vector3 targetPoint;

            // for spread
            Physics.Raycast(transform.position, transform.position, out aimHit, _weaponData.aimLayers);
            targetPoint = aimHit.point;
            //targetPoint = ray.GetPoint(_weaponData.range);


            //calculate direction of projectile normally
            Vector3 dirBeforeSpread = targetPoint - transform.position;

            //calculate direction with spread
            Vector3 dirWithSpread = dirBeforeSpread;

            //apply hipfire spread
            float spread = 180 - _weaponData.hipfireAcc;
            float xSpread = Random.Range(-spread, spread);
            float ySpread = Random.Range(-spread, spread);
            float zSpread = Random.Range(-spread, spread);
            dirWithSpread += new Vector3(xSpread, ySpread, zSpread);
        }
    }

    //creates visual effects cause by projectile
    void SpawnVFX()
    {
        //check if porjectile has any effects
        if (_weaponData.impactVFX.Length != 0)
        {
            foreach (GameObject impactEffect in _weaponData.impactVFX)
            {                
                if (impactEffect == null)
                    return;

                GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
        }
    }

    //called before destroying this game object
    void DisableProjectile()
    {
        //Debug.Log("Projectile disabled");
        _visual.SetActive(false);
        _hasHitTarget = true;
        _rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;

        Invoke("DestroyMe", 1f);
    }

    void DestroyMe()
    {
        Destroy(this.gameObject);
    }

    //Collision detection
    private void OnCollisionEnter(Collision collision)
    {
        if((_weaponData.hitMask.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer) {
            //Debug.Log("Projectile hit: " + collision.gameObject.name);
            _collisionCount++;

            //check if collision is a "soft target", in which case bypass certain traits like bouncing
            if (collision.gameObject.layer != 10) //player layer ignore for now         
                OnHit(collision.gameObject);
            
            //if projectile has meet a condition to be destroyed, destroy projectile (explode as well if explodable)
            if(_collisionCount == _weaponData.maxCollisions || (!_weaponData.isFMJ && !_weaponData.isBouncy))
            {
                //Debug.Log("Collision limit reached");               
                OnHit(null);
                DisableProjectile();
            }              
        }  
    }
}
