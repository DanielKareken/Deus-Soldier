using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth;

    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;    
    }

    public void DealDamage(float damage)
    {
        _currentHealth -= damage;
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public void InitializeHealth(float amount)
    {
        _maxHealth = amount;
        _currentHealth = _maxHealth;
    }

    public void SetHealth(float amount)
    {
        _currentHealth = amount;
    }

    public void AddHealth(float amount)
    {
        if (_currentHealth + amount > _maxHealth)
            _currentHealth = _maxHealth;
        else
            _currentHealth += amount;
    }
}
