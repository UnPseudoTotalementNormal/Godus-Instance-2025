using UnityEngine;
public class GameOverCondition : MonoBehaviour
{

    private void Awake()
    {
        GameEvents.onAlienDeath += OnAlienDeath;
    }

    private void OnDestroy()
    {
        GameEvents.onAlienDeath -= OnAlienDeath;
    }

    private void OnAlienDeath(Entity _entity)
    {
        if (UnitManager.instance.alienUnits.Count == 0)
        {
            GameEvents.onGameOver?.Invoke();
        }
    }
}
