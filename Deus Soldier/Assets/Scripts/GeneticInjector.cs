using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InjectorType
{
    SINGLE,
    CHOICE,
    RANDOM
}

public class GeneticInjector : Item
{
    [Header("Injector Refs")]
    [SerializeField] private InjectionUI injectionUI;

    [Header("Genetic Injector Data")]
    [SerializeField] private InjectorType injectorType;
    [SerializeField] private Trait[] traitPool;

    // Start is called before the first frame update
    void Start()
    {
        _colliderRef = GetComponent<Collider>(); //assign collider
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {          
            _playerRef = collision.gameObject;
            InjectTrait();
            OnCollect();          
        }            
    }

    //handles player acquiring trait for each case
    void InjectTrait()
    {
        switch (injectorType)
        {
            //give player trait
            case InjectorType.SINGLE:
                _playerRef.GetComponent<TraitManager>().ActivateTrait(traitPool[0].name);
                break;
            //offer player choices and give desired trait
            case InjectorType.CHOICE:
                injectionUI.DisplayInjectionUI(traitPool);
                break;
            //randomly select trait from pool and give to player
            case InjectorType.RANDOM:
                _playerRef.GetComponent<TraitManager>().ActivateTrait(traitPool[Random.Range(0, traitPool.Length)].name);
                break;
            default:
                Debug.Log("Missing injectorType case for InjectTrait()");
                break;
        }
    }

    public override void UseItem()
    {
        InjectTrait();
    }
}
