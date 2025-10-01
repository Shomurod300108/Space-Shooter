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

    void Start()
    {

    }

    public void StartSpawning()
{
     StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
       
}

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 postoSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, postoSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2.0f);
        }
    }
    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
       {
         Vector3 postoSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0, 3);
         Instantiate(_powerups[randomPowerUp], postoSpawn, Quaternion.identity);
         yield return new WaitForSeconds(Random.Range(3, 8));
       }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}

