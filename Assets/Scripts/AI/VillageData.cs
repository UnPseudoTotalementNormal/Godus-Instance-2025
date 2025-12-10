using UnityEngine;

namespace AI
{
    public class VillageData
    {
        //Resources
        public int wood = 0;
        public int maxWood = 50;
        public int stone = 0;
        public int maxStone = 50;
        public int iron = 0;
        public int maxIron = 50;
        public int glorp = 0;
        public int maxGlorp = 50;
        public int meat = 0;
        public int maxMeat = 50;
        
        public void Add(ResourceType type, int amount)
        {
            ref int value = ref GetValueRef(type);
            int max = GetMaxValue(type);

            value = Mathf.Clamp(value + amount, 0, max);

            GameEvents.onResourceValueRefreshed?.Invoke(type, value);
        }
        
        public void Set(ResourceType type, int newValue)
        {
            ref int value = ref GetValueRef(type);
            int max = GetMaxValue(type);

            value = Mathf.Clamp(newValue, 0, max);

            GameEvents.onResourceValueRefreshed?.Invoke(type, value);
        }
        
        public void AddMax(ResourceType type, int amount)
        {
            ref int max = ref GetMaxRef(type);
            max = Mathf.Max(0, max + amount);

            ref int value = ref GetValueRef(type);
            value = Mathf.Clamp(value, 0, max);

            GameEvents.onResourceMaxValueRefreshed?.Invoke(type, max);
        }

        public void SetMax(ResourceType type, int newMax)
        {
            ref int max = ref GetMaxRef(type);
            max = Mathf.Max(0, newMax);

            ref int value = ref GetValueRef(type);
            value = Mathf.Clamp(value, 0, max);

            GameEvents.onResourceMaxValueRefreshed?.Invoke(type, max);
        }
        
        private ref int GetValueRef(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:  return ref wood;
                case ResourceType.Stone: return ref stone;
                case ResourceType.Iron:  return ref iron;
                case ResourceType.Glorp: return ref glorp;
                case ResourceType.Meat:  return ref meat;
                default: throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private ref int GetMaxRef(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:  return ref maxWood;
                case ResourceType.Stone: return ref maxStone;
                case ResourceType.Iron:  return ref maxIron;
                case ResourceType.Glorp: return ref maxGlorp;
                case ResourceType.Meat:  return ref maxMeat;
                default: throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private int GetMaxValue(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:  return maxWood;
                case ResourceType.Stone: return maxStone;
                case ResourceType.Iron:  return maxIron;
                case ResourceType.Glorp: return maxGlorp;
                case ResourceType.Meat:  return maxMeat;
                default: throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}