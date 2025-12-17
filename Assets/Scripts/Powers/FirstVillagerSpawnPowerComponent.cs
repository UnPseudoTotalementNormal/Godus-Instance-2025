using UnityEngine;

[RequireComponent(typeof(ResourcesDrop))]
public class FirstVillagerSpawnPowerComponent : MonoBehaviour
{
    private ResourcesDrop resourcesDropPower;
    
    [SerializeField] private GameObject villagerPrefab;
    [SerializeField] private int numberOfVillagersToSpawn = 3;
    
    private bool hasSpawnedFirstVillagers = false;
    
    private void Awake()
    {
        resourcesDropPower = GetComponent<ResourcesDrop>();
        resourcesDropPower.onResourcesDropped += HandleResourcesDropped;
    }

    private void HandleResourcesDropped(Vector2Int _resourcePos)
    {
        if (hasSpawnedFirstVillagers)
        {
            return;
        }
        
        hasSpawnedFirstVillagers = true;
        for (int _i = 0; _i < numberOfVillagersToSpawn; _i++)
        {
            Vector2 _spawnPosition = new Vector2(_resourcePos.x, _resourcePos.y);
            Instantiate(villagerPrefab, _spawnPosition, Quaternion.identity);
        }
    }
}
