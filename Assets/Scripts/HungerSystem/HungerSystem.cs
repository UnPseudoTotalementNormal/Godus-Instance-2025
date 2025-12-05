using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HungerSystem : MonoBehaviour
{

    [SerializeField] private float maxHunger = 100;
    private float hunger = 100;
    [SerializeField] private float starvePerSecond = 1;

    public float EatingFood(float _regainHunger)
    {
        hunger += _regainHunger;
        if (hunger > maxHunger)
        {
            hunger = maxHunger;
        }
        return hunger;
    }
    
    void Update()
    {
        if (hunger > 0)
        {
            hunger -= starvePerSecond * Time.deltaTime;
        }
    }
}
