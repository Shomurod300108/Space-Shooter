using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stopYPosition = 2f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private int _health = 100;
    [SerializeField] private GameObject _explosionPrefab; 
    private Player _player;
    private bool _isAtCenter = false;
    private bool _isAlive = true;
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        StartCoroutine(BossEntry());
    }

    IEnumerator BossEntry()
    {
        while (transform.position.y > _stopYPosition)
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
            yield return null;
        }

        _isAtCenter = true;
        StartCoroutine(AttackPatternLoop());
    }

    IEnumerator AttackPatternLoop()
    {
        while (_isAlive)
        {
            int pattern = Random.Range(0, 2);
            switch (pattern)
            {
                case 0: yield return StartCoroutine(LaserAttack()); break;
                case 1: yield return StartCoroutine(MissileAttack()); break;
            }
            yield return new WaitForSeconds(_fireRate);
        }
    }

    IEnumerator LaserAttack()
    {
        Instantiate(_laserPrefab, transform.position + Vector3.down * 1.2f, Quaternion.identity);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator MissileAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(_missilePrefab, transform.position + new Vector3(-1 + i, -1, 0), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") || other.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
            Damage(5);
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Damage(10);
        }
    }

    public void Damage(int amount)
    {
        _health -= amount;

        if (_health <= 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Spawn_Manager spawnManager = FindObjectOfType<Spawn_Manager>();
            if (spawnManager != null)
            {
                spawnManager.OnBossDefeated();
            }

            Destroy(gameObject);
        }
    }

    void Die()
    {
        _isAlive = false;
        Destroy(gameObject);
    }


    
}
