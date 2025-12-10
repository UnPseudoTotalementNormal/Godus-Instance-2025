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
        
        public void Add(ResourceType _type, int _amount)
        {
            ref int _value = ref GetValueRef(_type);
            int _max = GetMaxValue(_type);

            _value = Mathf.Clamp(_value + _amount, 0, _max);

            GameEvents.onResourceValueRefreshed?.Invoke(_type, _value);
        }
        
        public void Set(ResourceType _type, int _newValue)
        {
            ref int _value = ref GetValueRef(_type);
            int _max = GetMaxValue(_type);

            _value = Mathf.Clamp(_newValue, 0, _max);

            GameEvents.onResourceValueRefreshed?.Invoke(_type, _value);
        }
        
        public void AddMax(ResourceType _type, int _amount)
        {
            ref int _max = ref GetMaxRef(_type);
            _max = Mathf.Max(0, _max + _amount);

            ref int _value = ref GetValueRef(_type);
            _value = Mathf.Clamp(_value, 0, _max);

            GameEvents.onResourceMaxValueRefreshed?.Invoke(_type, _max);
        }

        public void SetMax(ResourceType _type, int _newMax)
        {
            ref int _max = ref GetMaxRef(_type);
            _max = Mathf.Max(0, _newMax);

            ref int _value = ref GetValueRef(_type);
            _value = Mathf.Clamp(_value, 0, _max);

            GameEvents.onResourceMaxValueRefreshed?.Invoke(_type, _max);
        }
        
        private ref int GetValueRef(ResourceType _type)
        {
            switch (_type)
            {
                case ResourceType.Wood:  return ref wood;
                case ResourceType.Stone: return ref stone;
                case ResourceType.Iron:  return ref iron;
                case ResourceType.Glorp: return ref glorp;
                case ResourceType.Meat:  return ref meat;
                default: throw new System.ArgumentOutOfRangeException(nameof(_type), _type, null);
            }
        }

        private ref int GetMaxRef(ResourceType _type)
        {
            switch (_type)
            {
                case ResourceType.Wood:  return ref maxWood;
                case ResourceType.Stone: return ref maxStone;
                case ResourceType.Iron:  return ref maxIron;
                case ResourceType.Glorp: return ref maxGlorp;
                case ResourceType.Meat:  return ref maxMeat;
                default: throw new System.ArgumentOutOfRangeException(nameof(_type), _type, null);
            }
        }

        private int GetMaxValue(ResourceType _type)
        {
            switch (_type)
            {
                case ResourceType.Wood:  return maxWood;
                case ResourceType.Stone: return maxStone;
                case ResourceType.Iron:  return maxIron;
                case ResourceType.Glorp: return maxGlorp;
                case ResourceType.Meat:  return maxMeat;
                default: throw new System.ArgumentOutOfRangeException(nameof(_type), _type, null);
            }
        }
    }
}