using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//affects how the weapon shoots
public enum WeaponProjectileType
{
    RAYCAST, PROJECTILE
}

//determines fire type of weapon
public enum WeaponFireType
{
    MELEE, SEMI, AUTOMATIC
}

//affects ammo reserve interactions such as reload, overheat
//also affects ProjectileMaterial
public enum AmmunitionType
{
    BULLET,
    ENERGY
}

[CreateAssetMenu(menuName = "new WeaponData")]
public class WeaponData : ScriptableObject 
{
    [Header("Weapon Stats")]
    [SerializeField] public int maxMagSize;
    [SerializeField] public float range;
    [SerializeField] public float firerate;
    [SerializeField] public float revSpeed;
    [Range(0, 180)]
    [SerializeField] public float hipfireAcc;
    [SerializeField] public bool isShotgun; //fires multiple projectiles per shot
    [Tooltip("Always 1 if isShotgun is not true")]
    [SerializeField] public int projectileCountPerShot = 1; //how amny porjectiles per shot
    [SerializeField] public float recoilForce;
    [SerializeField] public bool allowADS;
    [SerializeField] public float adsSpeed;
    [SerializeField] public LayerMask aimLayers;
    [Space]
    [SerializeField] public WeaponProjectileType projectileType;
    [SerializeField] public WeaponFireType fireType;

    [Header("Projectile Stats")]
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float upwardForce;
    [SerializeField] public LayerMask hitMask; //what projectiles can hit
    [SerializeField] public float projectileDamage;
    [SerializeField] public int maxCollisions = 1; //how many times projectile can collide before destroying (NOTE: overrides collision restraints such as FMJ)
    [SerializeField] public bool isDamageOnHit; //false if collisions dont do damage ever

    [Header("Projectile Impact Types")]
    [SerializeField] public GameObject[] impactVFX;
    [Tooltip("FMJ allows projectiles to penetrate multiple surfaces (depending on maxCollisions var)")]
    [SerializeField] public bool isFMJ; //mutually exclusive with isBouncy
    [Tooltip("Incendiary projectiles ignite flammable objects on hit")]
    [SerializeField] public bool isIncendiary;

    [Header("Split Shot Stats")]
    [Tooltip("If true, projectile splits into mutliple after first collision")]
    [SerializeField] public bool isSplitShot;
    [Tooltip("Number of projectiles spawned after splitting")]
    [SerializeField] public int splitShotCount;
    [Range(0, 180)]
    [SerializeField] public float splitShotSpread;
    [Tooltip("Max time to pass before automatically splitting")]
    [SerializeField] public float maxSplitTime;

    [Header("Projectile Explosive Stats")]
    [SerializeField] public bool isExplosive;
    [SerializeField] public GameObject explosiveEffect;
    [Tooltip("If false, projectiles will not detonate on collision, instead using fuse timer")]
    [SerializeField] public bool explodeOnTouch = true;
    [Tooltip("If true, spawns more projectiles on explosion")]
    [SerializeField] public bool isClusterBomb;
    [Range(1, 10)]
    [Tooltip("Number of projectiles to spawn on explosion (if cluster bomb)")]
    [SerializeField] public int clusterProjectileCount;
    [Tooltip("Number of times projectile will cluster")]
    [SerializeField] public int maxClusterTimes;
    [Tooltip("Time until projectile automatically detonates")]
    [SerializeField] public float maxFuseTime; //time until automatic explosion
    [SerializeField] public float explosionRadius;
    [SerializeField] public float explosionDamage;
    [SerializeField] public float explosionForceMulti;

    [Header("Projectile Bouncy Stats")]
    [Tooltip("Projectile bounces off objects (number of times based on max collisions var)")]
    [SerializeField] public bool isBouncy; //mutually exclusive with isFMJ
    [Range(0, 1)]
    [SerializeField] public float bounciness; 

    [Header("Projectile Other")]
    [SerializeField] public ProjectileMaterial projectileMaterial;
    [SerializeField] public bool useGravity; //true if projectile lobs like a grenade, false if it follows a straight path
}
