using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodableItem : MonoBehaviour
{
    [SerializeField] private GameObject _explodeEffect;
    [SerializeField] private Health _health;

    [SerializeField] private float _damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //check health
        if (_health.GetHealth() <= 0)
            Explode();
    }

    void Explode()
    {
        Instantiate(_explodeEffect, transform.position, transform.rotation);
        DestroyMe();
    }

    void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
