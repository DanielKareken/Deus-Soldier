using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    ADDITIVE,
    MULTIPLICATIVE
}

public class StatusBuff : MonoBehaviour
{  
    public ModifierType modType;
    public float totalDuration;
    public float modifierAmount;
    ResourceStat _resourceStatRef;
    PlayerBuffsUI _buffsUIRef;

    float _lifeTime = 0f;
    bool _initialized;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_initialized)
        {
            //timer
            if (_lifeTime < totalDuration)
                _lifeTime += Time.deltaTime;
            else
                OnDurationEnd();

            //UI
            /*if (_buffsUIRef)
            {
                _buffsUIRef.

            }*/              
        }
    }

    //constructor
    public StatusBuff(ResourceStat resource, float duration, float amount, ModifierType modType, PlayerBuffsUI buffUIRef = null, Item item = null)
    {
        _resourceStatRef = resource;
        modType = modType;
        totalDuration = duration;
        modifierAmount = amount;

        if (buffUIRef && item)
        {
            _buffsUIRef = buffUIRef;
            _buffsUIRef.AddBuff(item);
        }           

        _initialized = true;
    }

    void OnDurationEnd()
    {
        _resourceStatRef.RemoveModifier(this.GetComponent<StatusBuff>());
    }
}
