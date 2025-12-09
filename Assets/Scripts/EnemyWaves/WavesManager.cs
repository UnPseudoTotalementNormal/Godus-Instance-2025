using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using AI;
using AYellowpaper.SerializedCollections;
using TileSystemSpace;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyGroup
{
    public GameObject enemyTypePrefab;

    [Header("For Manual Waves")]
    public int numberOfEnemies;
    public float firstInterval;
    public float spawnInterval;

    [Header("For Random Waves")] 
    [Range(0f, 1f)] public float chanceToSpawn = 0f;
    public int enemyPowerLevel;
    public Vector2 firstIntervalRange = new(1f, 5f);
    public Vector2 spawnIntervalRange = new(1f, 3f);
}

[Serializable]
public class Waves
{
    public List<EnemyGroup> enemyTypePrefab;
}


[Serializable]
public class WaveInfo
{
    public int maxEnemiesInWave = 0;
    public int currentWave = 0;
}

public class WavesManager : MonoBehaviour
{
    [SerializeField] private WaveInfo waveInfo;
    [SerializeField] private List<EnemyGroup> enemyType;
    [SerializeField] private SerializedDictionary<int, Waves> waves;
    [SerializeField] private TileSystem tileSystem;

    private Tile _tileForSpawn;
    [SerializeField] private int safeSpawnRadius = 50;
    [SerializeField] private Vector3 waveSpawnPoint = Vector3.zero;
    [SerializeField] private List<GameObject> currentEnemyAlive = new();

    [Header("Waves Timer")]
    [SerializeField] private float firstTimeBetweenWaves = 180f;
    [SerializeField] private float otherTimeBetweenWaves = 60f;
    private float timeBetweenWaves;

    [Header("For Endless Waves")]
    [SerializeField] private int wavePowerLevel = 10;
    [SerializeField] private float wavePowerLevelMultiplier = 1.1f;
    private int actualWavePowerLevel = 0;
    
    [Header("Random Spawn Balancing")]
    [SerializeField] private float highPowerChanceMultiplier = 1.2f;
    [SerializeField] private float lowPowerChanceMultiplier = 0.9f;

    private bool waveIsRunning = false;
    
    private void Start()
    {
        timeBetweenWaves = firstTimeBetweenWaves;
        StartWave();
    }

    public void WavesCanStart()
    {
        StartCoroutine(WaveTimerCoroutine(timeBetweenWaves));
    }

    private IEnumerator WaveTimerCoroutine(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        StartWave();
    }

    private void StartWave()
    {
        StopCoroutine(WaveTimerCoroutine(timeBetweenWaves));
        waveInfo.currentWave++;
        SetWaveSpawnPoint();
        GameEvents.onEnabledSlideBarRemainingEnemy?.Invoke(true);
        GameEvents.onWaveStarted?.Invoke();
        GameEvents.onWaveInfo?.Invoke(waveInfo);

        waveInfo.maxEnemiesInWave = 0;

        if (waves.ContainsKey(waveInfo.currentWave))
        {
            timeBetweenWaves = otherTimeBetweenWaves;
            CountEnemiesInManualWave();
            WavesSpawner();
            return;
        }

        timeBetweenWaves = otherTimeBetweenWaves;
        RandomWavesSpawner();
    }

    private void CountEnemiesInManualWave()
    {
        foreach (var _group in waves[waveInfo.currentWave].enemyTypePrefab)
            waveInfo.maxEnemiesInWave += _group.numberOfEnemies;
    }

    private void WavesSpawner()
    {
        waveIsRunning = true;

        foreach (var _group in waves[waveInfo.currentWave].enemyTypePrefab)
            StartCoroutine(SpawnGroupCoroutine(_group));
    }

    private void RandomWavesSpawner()
    {
        waveIsRunning = true;
        actualWavePowerLevel = 0;
        waveInfo.maxEnemiesInWave = 0;

        List<EnemyGroup> _generatedGroups = new List<EnemyGroup>();

        int _maxPower = 1;
        foreach (var _g in enemyType)
            if (_g.enemyPowerLevel > _maxPower) _maxPower = _g.enemyPowerLevel;

        while (actualWavePowerLevel < wavePowerLevel)
        {
            List<EnemyGroup> _possible = new List<EnemyGroup>();

            foreach (var _group in enemyType)
            {
                if (actualWavePowerLevel + _group.enemyPowerLevel > wavePowerLevel)
                    continue;

                float _powerRatio = _group.enemyPowerLevel / (float)_maxPower;
                
                if (_powerRatio >= 0.75f)
                    _group.chanceToSpawn *= highPowerChanceMultiplier;
                else if (_powerRatio <= 0.35f)
                    _group.chanceToSpawn *= lowPowerChanceMultiplier;

                _possible.Add(_group);
            }

            if (_possible.Count == 0)
                break;

            int _chosenIndex = WeightedRandom(_possible);
            EnemyGroup _chosen = _possible[_chosenIndex];

            int _maxUnits = (wavePowerLevel - actualWavePowerLevel) / _chosen.enemyPowerLevel;
            if (_maxUnits <= 0)
                continue;

            EnemyGroup _clone = new EnemyGroup
            {
                enemyTypePrefab = _chosen.enemyTypePrefab,
                enemyPowerLevel = _chosen.enemyPowerLevel,
                chanceToSpawn = _chosen.chanceToSpawn,
                firstInterval = Random.Range(_chosen.firstIntervalRange.x, _chosen.firstIntervalRange.y),
                spawnInterval = Random.Range(_chosen.spawnIntervalRange.x, _chosen.spawnIntervalRange.y),
                numberOfEnemies = _maxUnits
            };

            waveInfo.maxEnemiesInWave += _clone.numberOfEnemies;
            actualWavePowerLevel += _clone.numberOfEnemies * _clone.enemyPowerLevel;

            _generatedGroups.Add(_clone);
        }

        foreach (var _group in _generatedGroups)
            StartCoroutine(SpawnGroupCoroutine(_group));

        wavePowerLevel = Mathf.RoundToInt(wavePowerLevel * wavePowerLevelMultiplier);
    }

    private int WeightedRandom(List<EnemyGroup> _groups)
    {
        float _totalWeight = 0f;
        foreach (var _group in _groups)
            _totalWeight += _group.chanceToSpawn;

        float _pick = Random.Range(0f, _totalWeight);
        float _cumulative = 0f;

        for (int _i = 0; _i < _groups.Count; _i++)
        {
            _cumulative += _groups[_i].chanceToSpawn;
            if (_pick <= _cumulative)
                return _i;
        }

        return _groups.Count - 1;
    }

    private void SetWaveSpawnPoint()
    {
        do
        {
            waveSpawnPoint = new Vector3(
                Mathf.RoundToInt(Random.Range(0, tileSystem.GetSize().x)),
                Mathf.RoundToInt(Random.Range(0, tileSystem.GetSize().y)),
                0
            );

            _tileForSpawn = tileSystem.GetTile(
                Mathf.RoundToInt(waveSpawnPoint.x),
                Mathf.RoundToInt(waveSpawnPoint.y)
            );

        }
        while (_tileForSpawn.tileType == TileType.Water || IsEnemyNextTo());
    }

    private IEnumerator SpawnGroupCoroutine(EnemyGroup _group)
    {
        int _spawned = 0;

        yield return new WaitForSeconds(_group.firstInterval);

        SpawnEnemy(_group.enemyTypePrefab);
        _spawned++;

        while (waveIsRunning && _spawned < _group.numberOfEnemies)
        {
            yield return new WaitForSeconds(_group.spawnInterval);

            SpawnEnemy(_group.enemyTypePrefab);
            _spawned++;
        }
    }

    private void SpawnEnemy(GameObject _enemy)
    {
        GameObject _new = Instantiate(_enemy, waveSpawnPoint, Quaternion.identity);
        currentEnemyAlive.Add(_new);

        TestEnemy _comp = _new.GetComponent<TestEnemy>();
        if (_comp != null)
        {
            _comp.onDeath += () =>
            {
                HandleEnemyDeath(_new);
            };
        }
    }

    private void HandleEnemyDeath(GameObject _enemy)
    {
        currentEnemyAlive.Remove(_enemy);
        GameEvents.onEnemyDeath?.Invoke();

        if (currentEnemyAlive.Count > 0)
            return;

        WaveIsEnded();
    }

    private void WaveIsEnded()
    {
        waveIsRunning = false;
        StopAllCoroutines();
        StartCoroutine(WaveTimerCoroutine(timeBetweenWaves));
        GameEvents.onWaveEnded?.Invoke();
        GameEvents.onEnabledSlideBarRemainingEnemy?.Invoke(false);
    }

    private bool IsEnemyNextTo()
    {
        Collider2D[] _hit = Physics2D.OverlapCircleAll(new Vector2(waveSpawnPoint.x, waveSpawnPoint.y), safeSpawnRadius);

        for (int _i = 0; _i < _hit.Length; _i++)
        {
            if (_hit[_i].TryGetComponent<ITeamComponent>(out var _team))
            {
                if (_team.team == EntityTeam.Alien)
                    return true;
            }
        }
        return false;
    }
}