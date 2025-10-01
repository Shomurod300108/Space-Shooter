using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 19f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private Spawn_Manager _spawnManager;
    
    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is null");
        }

        


    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.75f);
            _spawnManager.StartSpawning();
            Destroy(other.gameObject);
        }
        
    }
}
