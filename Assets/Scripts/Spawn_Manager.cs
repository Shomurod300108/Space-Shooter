using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    private bool _stopSpawning = false;
    private bool _isSpawning = false;
    [SerializeField] private int _startingEnemiesInWave = 5;  //enemies in wave 1 
    [SerializeField] private int _enemiesIncreasePerWave = 2;  //how many added each wave
    [SerializeField] private float _timeBetweenSpawns = 0.75f;   //time between each enemy spawn
    [SerializeField] private float _timeBetweenWaves = 2.5f;   //delay after a wave finishes before next wave
    private int _currentWave = 0;
    private int _enemiesInWave = 0;
    [SerializeField] private GameObject _seekerEnemyPrefab;
    [SerializeField] private GameObject _dodgerEnemyPrefab;
    [SerializeField] private GameObject _projectilePowerupPrefab;
    [SerializeField] private float _projectilePowerupChance = 0.05f;
    [SerializeField] private GameObject _bossEnemyPrefab;
    private bool _bossSpawned = false;

    public int CurrentWave => _currentWave;
  
    public void StartSpawning()
    {
        if (_isSpawning) return;
        _isSpawning = true;

        _currentWave = 0;
        _enemiesInWave = _startingEnemiesInWave;

        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(WaveRoutine());
    }

    private void SpawnEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 6f, 0);

        float randomValue = Random.value; // Generate once between 0 and 1
        GameObject enemyToSpawn;

        if (randomValue < 0.15f)
        {
            // 15% chance → Dodger enemy
            enemyToSpawn = _dodgerEnemyPrefab;
        }
        else if (randomValue < 0.40f)
        {
            // Next 25% chance → Seeker enemy
            enemyToSpawn = _seekerEnemyPrefab;
        }
        else
        { 
            // Remaining 60% → Normal enemy
            enemyToSpawn = _enemyPrefab;
        }

        GameObject newEnemy = Instantiate(enemyToSpawn, posToSpawn, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    IEnumerator SpawnEnemyWaveRoutine(int enemiesCount)
    {
      for (int i = 0; i < enemiesCount; i++)
      {
        if (_stopSpawning) yield break;

        SpawnEnemy();

        yield return new WaitForSeconds(_timeBetweenSpawns);
      }

      yield break; 
    }

    IEnumerator WaveRoutine()
    {
      while (!_stopSpawning)
      {
        _currentWave++;

        _enemiesInWave = _startingEnemiesInWave + (_currentWave - 1) * _enemiesIncreasePerWave;

        Debug.Log("Spawning wave " + _currentWave);

        yield return StartCoroutine(SpawnEnemyWaveRoutine(_enemiesInWave));

        yield return new WaitUntil(() => _enemyContainer == null || _enemyContainer.transform.childCount == 0);

        if (_currentWave >= 3 && !_bossSpawned)
        {
            // Show flicker warning before boss appears
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                yield return StartCoroutine(uiManager.ShowBossWarning());
            }

            Instantiate(_bossEnemyPrefab, new Vector3(0, 7f, 0), Quaternion.identity);

            _bossSpawned = true;
            _stopSpawning = true;
            yield break; 
        }

        yield return new WaitForSeconds(_timeBetweenWaves);
       }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            float randomValue = Random.value;
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0f);

            if (randomValue < _projectilePowerupChance)
            {
                // Spawn your rare projectile powerup
                Instantiate(_projectilePowerupPrefab, posToSpawn, Quaternion.identity);
            }
            else
            {
                // Spawn one of your normal powerups
                GameObject randomPowerup = _powerups[Random.Range(0, _powerups.Length)];
                Instantiate(randomPowerup, posToSpawn, Quaternion.identity);
            }
        }

        yield break;
    }

    public void OnBossDefeated()
    {
        _bossSpawned = false;
        _stopSpawning = false;

        StartCoroutine(WaveRoutine());
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        _isSpawning = false;
    }
}

