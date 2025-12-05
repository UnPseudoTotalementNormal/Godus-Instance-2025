using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyTypePrefab;
    
    [Header("For Manual Waves")]
    public int numberOfEnemies;

    [Header("For Random Waves")] [Range(0f, 1f)]
    public float chanceToSpawn = 0f;
    public int enemyPowerLevel; //puissance de l'unité
}

[System.Serializable]
public class Waves
{
    public List<EnemyGroup> enemyTypePrefab;
}

public class WavesManager : MonoBehaviour
{
    public event Action OnWaveStarted;
    [SerializeField] private List<EnemyGroup> enemyType;
    [SerializeField] SerializedDictionary<int,Waves> _waves;
    [SerializeField] private float enemySpawnInterval = 0.5f;
    [SerializeField] private int wavePowerLevel = 10;
    [SerializeField] private float wavePowerLevelMultiplier = 1.1f;
    private int actualWavePowerLevel = 0;
    private int enemyIndex = 0;
    
    [Header("Waves Timer")]
    [SerializeField] private float firstTimeBetweenWaves = 180f;
    [SerializeField] private float otherTimeBetweenWaves = 60f;
    private float timeBetweenWaves;
    
    private int currentWave = 0;

    private void Start()
    {
        timeBetweenWaves = firstTimeBetweenWaves;
        StartCoroutine(RandomWavesSpawner()); // A enlever (c pour test)

    }

    public void WavesCanStart() //appeler lorsque le joueur place un alien sur la map
    {
        StartCoroutine(WaveTimerCoroutine(timeBetweenWaves));
    }

    private IEnumerator WaveTimerCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);
        StartWave();
    }

    private void StartWave()
    {
        currentWave++;
        OnWaveStarted?.Invoke();
        if (_waves.ContainsKey(currentWave))
        {
            timeBetweenWaves = otherTimeBetweenWaves;
            StartCoroutine(WavesSpawner());
            return;
        }
        timeBetweenWaves = otherTimeBetweenWaves;
        StartCoroutine(RandomWavesSpawner());
    }

    private IEnumerator WavesSpawner()
    {
        WaitForSeconds _wait = new WaitForSeconds(enemySpawnInterval);

        SpawnEnemy();
        yield return new WaitForSeconds(firstTimeBetweenWaves);
    }
    
    
    private IEnumerator RandomWavesSpawner()
    {
        WaitForSeconds _wait = new WaitForSeconds(enemySpawnInterval);

        while (actualWavePowerLevel != wavePowerLevel)
        {
            yield return _wait;

            if (enemyType[^1].enemyPowerLevel > wavePowerLevel - actualWavePowerLevel)
            {
                for (int _enemyIndex = 0; _enemyIndex < enemyType.Count; _enemyIndex++)
                {
                    if (enemyType[_enemyIndex].enemyPowerLevel > wavePowerLevel - actualWavePowerLevel)
                    {
                        enemyIndex = _enemyIndex;
                        break;
                    }
                }
            }
            else
            {
                enemyIndex = enemyType.Count;
            }
            
            int _enemyToSpawn = WeightedRandom(enemyIndex);
            
            actualWavePowerLevel += enemyType[_enemyToSpawn].enemyPowerLevel;
            SpawnEnemy();
        }
        actualWavePowerLevel = 0;
        wavePowerLevel = Mathf.RoundToInt(wavePowerLevel * wavePowerLevelMultiplier);
        StopAllCoroutines();
        StartCoroutine(WaveTimerCoroutine(timeBetweenWaves));
    }

    private int WeightedRandom(int maxValueEnemyIndex)
    {
        float _totalWeight = 0f;

        for (int _i = 0; _i <= maxValueEnemyIndex; _i++)
        {
            _totalWeight += enemyType[_i].chanceToSpawn;
        }

        float _randomValue = Random.Range(0f, _totalWeight);
        float _cumulative = 0f;

        for (int _i = 0; _i < maxValueEnemyIndex; _i++)
        {
            _cumulative += enemyType[_i].chanceToSpawn;
            if (_randomValue < _cumulative)
            {
                return _i;
            }
        }

        return enemyType.Count - 1; 
    }

    private void SpawnEnemy()
    {
        
    }
    
}
