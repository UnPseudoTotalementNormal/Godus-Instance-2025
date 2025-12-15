using System;
using UnityEngine;
public class GameOverCondition : MonoBehaviour
{

    private void Awake()
    {
        GameEvents.onTownHallCreated += GameOver;
    }

    private void OnDestroy()
    {
        GameEvents.onTownHallCreated -= GameOver;
    }

    private void GameOver()
    {
        GameEvents.onGameOver?.Invoke();
        Debug.Log("Game Over");
    }
}
