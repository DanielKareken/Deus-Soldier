using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEffect : MonoBehaviour
{
    [SerializeField] private GameObject _vfx;

    WeaponData _weaponData;
    float _damageMulti;
    float _rangeMulti;

    //called by Projectile, passing its data to me
    public void SetupExplosion(WeaponData weaponData, float damageMulti = 1f, float rangeMulti = 1f)
    {
        _weaponData = weaponData;
        _damageMulti = damageMulti;
        _rangeMulti = rangeMulti;

        Explode();
    }

    void Explode()
    {
        //explosion VFX
        if(_vfx)
            Instantiate(_vfx, transform, false);

        var hits = Physics.OverlapSphere(transform.position, _weaponData.explosionRadius * _rangeMulti);

        if(hits != null)
        {
            Rigidbody rb;
            foreach (Collider hit in hits)
            {
                if (hit.GetComponent<Rigidbody>())
                {
                    //add force
                    rb = hit.GetComponent<Rigidbody>();
                    //Debug.Log("Hit: " + rb.name);
                    rb.AddExplosionForce(_weaponData.explosionForceMulti, transform.position, _weaponData.explosionRadius * _rangeMulti, 2f, ForceMode.Impulse);     
                }
                //deal damage
                if (hit.GetComponent<Health>())
                    hit.GetComponent<Health>().DealDamage(_weaponData.explosionDamage * _damageMulti);
            }
        }

        Invoke("DestroyMe", 2f);
    }

    private void DestroyMe()
    {
       Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _weaponData.explosionRadius);
    }
}

