using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct HealthComponent
{
    [Tooltip("Maximum number of dash charges the entity can have.")]
    [SerializeField] private int _maxHealth;

    [Tooltip("Maximum number of dash charges the entity can have.")]
    [SerializeField] private int _currentHealth;

    private const int MaxHealthLimit = 100;

    public readonly int GetCurrentHealth() => _currentHealth;

    public readonly int GetMaxHealth() => _maxHealth;

    public void SetHealth(int health) => _currentHealth = Mathf.Clamp(health, 0, _maxHealth);

    public void SetMaxHealth(int health) => _maxHealth = Mathf.Clamp(health, 1, MaxHealthLimit);

    public void ResetHealth() => _currentHealth = _maxHealth;

}

public class HealthComponentProvider : ProviderEcs<HealthComponent>
{ }
