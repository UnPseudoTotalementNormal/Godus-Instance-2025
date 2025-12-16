using AI;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class UpgradeStatsComponent : MonoBehaviour
{
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private AttackComponent attackComponent;
    
    [HideInInspector] public ResourceType[] upgradeOrder = 
    {
        ResourceType.None,
        ResourceType.Wood,
        ResourceType.Stone,
        ResourceType.Iron,
        ResourceType.Glorp,
    };
    
    public ResourceType currentUpgrade { get; private set; } = ResourceType.None;
    
    [SerializeField] private SerializedDictionary<ResourceType, float> healthUpgradeValues;
    [SerializeField] private SerializedDictionary<ResourceType, float> attackDamageUpgradeValues;
    [SerializeField] private SerializedDictionary<ResourceType, float> attackSpeedUpgradeValues;
    
    public ResourceType GetNextUpgrade()
    {
        int _currentIndex = System.Array.IndexOf(upgradeOrder, currentUpgrade);
        if (_currentIndex < upgradeOrder.Length - 1)
        {
            return upgradeOrder[_currentIndex + 1];
        }
        return ResourceType.None;
    }

    public void ApplyUpgrade()
    {
        ResourceType _nextUpgrade = GetNextUpgrade();
        if (_nextUpgrade == ResourceType.None)
        {
            Debug.LogWarning("No more upgrades available.");
            return;
        }

        if (healthComponent != null && healthUpgradeValues.ContainsKey(_nextUpgrade))
        {
            float _healthIncrease = healthUpgradeValues[_nextUpgrade];
            float _newMaxHealth = healthComponent.GetMaxHealth() + _healthIncrease;
            healthComponent.Heal(_newMaxHealth);
        }

        if (attackComponent != null)
        {
            if (attackDamageUpgradeValues.ContainsKey(_nextUpgrade))
            {
                float _damageIncrease = attackDamageUpgradeValues[_nextUpgrade];
                attackComponent.IncreaseAttackDamage(_damageIncrease);
            }

            if (attackSpeedUpgradeValues.ContainsKey(_nextUpgrade))
            {
                float _speedIncrease = attackSpeedUpgradeValues[_nextUpgrade];
                attackComponent.IncreaseAttackSpeed(_speedIncrease);
            }
        }

        currentUpgrade = _nextUpgrade;
    }
}
