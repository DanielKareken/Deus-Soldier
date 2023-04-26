using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    NORMAL,
    REGENERATE
}

public class ResourceStat : MonoBehaviour
{
    [Header("Resource Refs")]
    [SerializeField] private float _baseMaxValue;
    [SerializeField] private SliderBarUI _resourceBarRef;
    [SerializeField] private TraitManager _traitManager;
    [SerializeField] private ResourceType _resourceType;

    [Space]
    [SerializeField] private string _resourceName;
    [SerializeField] private float _regenBufferTime; //time after depletion before regen starts
    [SerializeField] private float _baseRegenAmount; //regen amount (PER SEC)

    float _maxValue;
    float _curValue;
    Trait _traitRef;
    bool _regening;
    float _regenTickTimer = 0f;
    float _regenBufferTimer;
    bool _isToggleTickReady; //used for ToggleChangeCurAmount
    float _toggledTickTimer;
    List<StatusBuff> _regenModifiers = new List<StatusBuff>();

    // Start is called before the first frame update
    void Start()
    {
        _curValue = _baseMaxValue;
        _maxValue = _baseMaxValue;
        if(_traitManager)
            _traitRef = _traitManager.GetTrait(_resourceName);
        _regenBufferTimer = _regenBufferTime;

        //initialize Health UI
        OnAmountChange();
    }

    // Update is called once per frame
    void Update()
    {
        //calculate max health based on Health trait
        if (_traitRef)
        {
            if (_traitRef.isAquired)
            {
                _maxValue = _baseMaxValue + _traitRef.traitLevel * _traitRef.modifierValue;
                OnAmountChange();
            }
        }    
        
        //Timers
        if(_resourceType == ResourceType.REGENERATE)
        {
            if (_regenBufferTimer > 0)
            {
                _regenBufferTimer -= Time.deltaTime;
                _regening = false;
            }               
            else
            {
                Regenerate();
            }
        }

        if (_toggledTickTimer > 0 && !_isToggleTickReady)
            _toggledTickTimer -= Time.deltaTime;
        else if(_toggledTickTimer <= 0)
        {
            _isToggleTickReady = true;
            _toggledTickTimer = 0.1f;
        }
    }

    //gets current resource amount
    public float GetCurAmount()
    {
        return _curValue;
    }

    //handles healing or reducing current resource (cant go higher than max resource)
    public void ChangeCurAmount(float amount)
    {
        //Debug.Log("Change current amount: " + amount);
        float lastCurValue = _curValue;

        if (_curValue + amount > _maxValue)
            _curValue = _maxValue;
        else if(_curValue + amount <= 0)
        {
            _curValue = 0;
        }
        else
            _curValue += amount;

        //reset regen buffer timer
        if (lastCurValue > _curValue)
            _regenBufferTimer = _regenBufferTime;

        //change Health UI
        OnAmountChange();           
    }

    //set max health to given amount
    public void SetMaxAmount(float amount)
    {
        _maxValue = amount;

        //change Resource UI
        OnAmountChange();
    }

    //similar to ChangeCurAmount, but is repeately called and is limited by a tick rate
    public void ToggleChangeCurAmount(float amount)
    {
        if(_isToggleTickReady)
        {
            _isToggleTickReady = false;
            ChangeCurAmount(amount);
        }    
    }

    public void AddModifier(StatusBuff buff)
    {
        _regenModifiers.Add(buff);
    }

    public void RemoveModifier(StatusBuff buff)
    {
        _regenModifiers.Remove(buff);
    }

    //regenerate resource
    void Regenerate()
    {
        _regening = true;

        if (_regenTickTimer > 0)
            _regenTickTimer -= Time.deltaTime;
        else
        {
            ChangeCurAmount(CalculateRegenRate());

            _regenTickTimer = 1f;
        }
    }
    //update Resource UI
    void OnAmountChange()
    {
        if (_resourceBarRef)
        {
            _resourceBarRef.SetValue(_curValue);
            _resourceBarRef.SetMaxValue(_maxValue);
        }            
    }

    float CalculateRegenRate()
    {
        float finalRegenRate = _baseRegenAmount;
        float flatAddition = 0f;

        foreach (StatusBuff modifier in _regenModifiers)
        {
            if (modifier.modType == ModifierType.ADDITIVE)
                flatAddition += modifier.modifierAmount;
            else
                finalRegenRate *= modifier.modifierAmount;
        }

        return finalRegenRate + flatAddition;
    }
}
