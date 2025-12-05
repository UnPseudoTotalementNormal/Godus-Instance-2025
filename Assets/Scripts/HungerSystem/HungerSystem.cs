using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HungerSystem : MonoBehaviour
{

    float hunger = 100;
    int starve = 1;
    int FoodGain = 1;


    float EatingFood(float hunger)
    {
        if (hunger< 50)
        hunger += FoodGain;
        return hunger;
    }
    
    void Update()
    {
        if (hunger >= 1)
        {
            hunger -= starve * Time.deltaTime;
            Mathf.Round(hunger);
            Debug.Log(Mathf.Round(hunger));
        }
        hunger = EatingFood(hunger);
    }
}
