using UnityEngine;
public class GameOverCondition : MonoBehaviour
{

    private void Awake()
    {
        GameEvents.onAlienDeath += GameOver;
    }

    private void OnDestroy()
    {
        GameEvents.onAlienDeath -= GameOver;
    }

    private void GameOver()
    {
        if (UnitManager.instance.alienUnit.Count == 0)
        {
            GameEvents.onGameOver?.Invoke();
        }
    }
}
