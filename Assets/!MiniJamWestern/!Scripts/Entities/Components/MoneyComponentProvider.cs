using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct MoneyComponent
{
    [SerializeField] private int _maxMoney;
    [SerializeField] private int _currentMoney;

    private const int MaxMoneyLimit = 9999;

    public readonly int GetCurrentMoney() => _currentMoney;
    public readonly int GetMaxMoney() => _maxMoney;

    public void SetMoney(int amount) => _currentMoney = Mathf.Clamp(amount, 0, _maxMoney);
    public void SetMaxMoney(int amount) => _maxMoney = Mathf.Clamp(amount, 1, MaxMoneyLimit);
    public void ResetMoney() => _currentMoney = _maxMoney;
}

public class MoneyComponentProvider : ProviderEcs<MoneyComponent>
{ }
