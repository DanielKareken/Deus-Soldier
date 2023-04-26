using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SentryState
{
    PATROL,
    ATTACK,
    UPGRADE
}

public class SentryGun : Weapon
{
    [Header("Sentry Refs")]
    [SerializeField] private Health _healthRef;
    [SerializeField] private GameObject _turretHead; //this moves based on physics, not animation

    [Header("Sentry Stats")]
    [SerializeField] private float _maxHealth; //will be passed to health script
    [SerializeField] private float _lockOnSpeed;
    [SerializeField] private LayerMask _targetMask;

    SentryState _sentryState;
    float _sentryLevel; //1 to 5
    float _rotationSpeed = 3f;

    GameObject _target; //current target of sentry, null if no target
    GameObject _playerRef; //player who is "owner" of sentry
    
    // Start is called before the first frame update
    void Start()
    {
        _sentryState = SentryState.PATROL;
        _ammoInMag = _weaponData.maxMagSize;
        _healthRef.InitializeHealth(_maxHealth);
        _sentryLevel = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Timers(); //super

        switch (_sentryState)
        {
            case SentryState.PATROL:
                Patrol();
                break;
            case SentryState.ATTACK:
                Attack();
                break;
            case SentryState.UPGRADE:
                Upgrade();
                break;
        }
    }
       
    //search for targets inside radius
    void Patrol()
    {
        if(FindClosestTarget())           
            _sentryState = SentryState.ATTACK; //switch to shoot phase
    }

    //shoot at target until target dies or leaves range  
    void Attack()
    {
        if(_target == null)
        {
            _sentryState = SentryState.PATROL;
            return;
        }
            
        //ensure target is still within range, if not, go back to patrol
        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        if (distanceToTarget > _weaponData.range)
            _sentryState = SentryState.PATROL;
        else
        {
            LookAtTarget();
            if (_allowFire)
                Shoot();
        }
    }

    //activity of sentry during upgrade phase
    //sentry can not act, but can take damage
    void Upgrade()
    {
        _sentryLevel++;
        _sentryState = SentryState.PATROL;
    }

    //return the closest valid target in range, returns true if found, false if no target found
    bool FindClosestTarget()
    {
        //search for targets in range
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, _weaponData.range, _targetMask, QueryTriggerInteraction.Collide);

        if (targetsInRange != null)
        {
            //make sure sentry can see target (not behind a wall)

            if (targetsInRange.Length == 1)
                _target = targetsInRange[0].gameObject;
            //get closest target to sentry and set as its target
            else
            {
                GameObject closestTarget = null;
                float closestTargetMinDistance = _weaponData.range + 1;

                foreach (Collider possibleTarget in targetsInRange)
                {
                    //calucate distance from new target to sentry
                    float distance = Vector3.Distance(transform.position, possibleTarget.transform.position);

                    //compare distance to current closest distance, if smaller, replace old with new target
                    if (distance < closestTargetMinDistance)
                    {
                        closestTarget = possibleTarget.gameObject;
                        closestTargetMinDistance = distance;
                    }
                }

                _target = closestTarget;
            }
            return true;
        }
        return false;
    }

    //offset y + 90, z - 90
    void LookAtTarget()
    {
        if (_target == null)
            return;

        Vector3 targtetDir = (_target.transform.position - _turretHead.transform.position).normalized;       
        Quaternion lookRotation = Quaternion.LookRotation(targtetDir);
        //Quaternion offset = new Quaternion(0f, 90f, -90f, 0);
        _turretHead.transform.rotation = Quaternion.Slerp(_turretHead.transform.rotation, lookRotation * Quaternion.Euler(0, 90f, -90f), Time.deltaTime * _rotationSpeed);
    }
}
