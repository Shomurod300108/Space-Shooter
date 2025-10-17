using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    private bool _stopSpawning = false;
    private bool _isSpawning = false;
    [SerializeField]
    private int _startingEnemiesInWave = 5;  //enemies in wave 1 
    [SerializeField]
    private int _enemiesIncreasePerWave = 2;  //how many added each wave
    [SerializeField]
    private float _timeBetweenSpawns = 0.75f;   //time between each enemy spawn
    [SerializeField]
    private float _timeBetweenWaves = 2.5f;   //delay after a wave finishes before next wave
    private int _currentWave = 0;
    private int _enemiesInWave = 0;
    [SerializeField]
    private GameObject _seekerEnemyPrefab;

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
    Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

    // 25% chance to spawn seeker enemy, otherwise normal
    GameObject enemyToSpawn = Random.value < 0.25f ? _seekerEnemyPrefab : _enemyPrefab;

    GameObject newEnemy = Instantiate(enemyToSpawn, posToSpawn, Quaternion.identity);
    newEnemy.transform.parent = _enemyContainer.transform;
}



    IEnumerator SpawnEnemyWaveRoutine(int enemiesCount)
{
    for (int i = 0; i < enemiesCount; i++)
    {
        if (_stopSpawning) yield break;

        // Decide which enemy to spawn: normal or seeker
        GameObject enemyPrefabToSpawn = Random.value < 0.25f ? _seekerEnemyPrefab : _enemyPrefab; 
        // 25% chance for seeker enemies (you can tweak this)

        Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
        GameObject newEnemy = Instantiate(enemyPrefabToSpawn, spawnPos, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;

        yield return new WaitForSeconds(_timeBetweenSpawns);
    }
}


    IEnumerator WaveRoutine()
    {
        while (!_stopSpawning)
        {
            _currentWave++;

            _enemiesInWave = _startingEnemiesInWave + (_currentWave - 1) * _enemiesIncreasePerWave;

            yield return StartCoroutine(SpawnEnemyWaveRoutine(_enemiesInWave));

            yield return new WaitUntil(() => _enemyContainer == null || _enemyContainer.transform.childCount == 0);

            if (_stopSpawning) break;

            yield return new WaitForSeconds(_timeBetweenWaves);
        }
    }

       IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
       {
         Vector3 postoSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0, 6);
         Instantiate(_powerups[randomPowerUp], postoSpawn, Quaternion.identity);
         yield return new WaitForSeconds(Random.Range(3, 8));
       }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        _isSpawning = false;
    }
}

