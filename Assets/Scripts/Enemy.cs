using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed = 3.0f;
    protected Player _player;
    protected Animator _anim;
    protected AudioSource _audioSource;
    [SerializeField] protected GameObject _explosionPrefab;
    [SerializeField] private GameObject _laserPrefab;
    protected float _fireRate = 3.0f;
    private float _canFire = 1f;
    private float _canBackFire = -1f;
    private float _backFireRate = 2f;
    private bool _isEnemyLaser = true;
    [SerializeField] private EnemyMovementPattern _movementPattern;
    private float _spawnTime;
    private float _frequency;
    private float _phase;
    private float _distanceY;
    [SerializeField] protected GameObject _shieldVisualizer;
    protected bool _hasShield = false;
    [SerializeField] private GameObject _enemyThruster;
    public float _dist;
    public float _aggroSpeed = 10f;
    [SerializeField] private GameObject _backwardLaserPrefab;
    [SerializeField] private float _horizontalThreshold = 0.5f;
    private bool _hasEnteredScreen = false;

    protected virtual void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_movementPattern == EnemyMovementPattern.ZigZag)
        {
            _spawnTime = Time.time;
            _frequency = (float)(Mathf.PI * UnityEngine.Random.Range(0.16f, 0.64f));
            _phase = UnityEngine.Random.Range(0f, 2f);
        }

        int chance = Random.Range(0, 100);
        if (chance < 30)
        {
            _hasShield = true;
            _shieldVisualizer.SetActive(true);
        }
        else
        {
            _shieldVisualizer.SetActive(false);
        }
    }

    protected virtual void Update()
    {

        if (_player == null) 
        return;

        // Distance check
        _dist = Vector3.Distance(transform.position, _player.transform.position);

        CalculateMovement();
        FireRoutine();
        CheckAndShootPickups();

        if (_player != null)
        {
            CheckPositionAndFire();
        }

        if (transform.position.y < 5.5f)
        {
            _hasEnteredScreen = true;
        }

        if (_hasEnteredScreen && transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void FireRoutine()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(6f, 10f);
            _canFire = Time.time + _fireRate;

            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
            lasers[i].AssignEnemyLaser();
            }
            
        }
    }

    private void CheckPositionAndFire()
    {
        bool isBehindPlayer = transform.position.y < _player.transform.position.y;
        bool isAlignedHorizontally = Mathf.Abs(transform.position.x - _player.transform.position.x) <= _horizontalThreshold;

        if (isBehindPlayer && isAlignedHorizontally)
        {
            Debug.Log("Enemy is below the player, backfire is being prepared");
            if (Time.time > _canBackFire)
            {
                Debug.Log("Enemy fired backward");
                _backFireRate = Random.Range(3f, 5f);
                _canBackFire = Time.time + _backFireRate;

                Instantiate(_backwardLaserPrefab, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);
            }
        }
    }

    protected virtual void CalculateMovement()
{
    if (_dist < 2.5f)
    {
        // Turn on thruster if not already
        if (_enemyThruster != null && !_enemyThruster.activeSelf)
            _enemyThruster.SetActive(true);

        // Accelerate towards player
        _aggroSpeed += 10f * Time.deltaTime; // acceleration value — tweak this number
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        transform.position += direction * _aggroSpeed * Time.deltaTime;
    }
    else
    {
        // Reset thruster
        if (_enemyThruster != null && _enemyThruster.activeSelf)
            _enemyThruster.SetActive(false);

        // Move normally until in range
        if (_movementPattern == EnemyMovementPattern.Down)
            MoveDown();
        else
            MoveZigZagDown();

        // Reset aggro speed for next time
        _aggroSpeed = 5f; // base speed before accelerating again
    }
}


    protected virtual void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            float RandomX = Random.Range(-11f, 11f);
            transform.position = new Vector3(RandomX, 7f, 0);
        }
    }

    protected virtual void MoveZigZagDown()
    {
        if (transform.position.y > -6f)
        {
            _distanceY = _speed * Mathf.Sin(_frequency * Time.time - _spawnTime + _phase) * Time.deltaTime;
            transform.Translate(Vector3.right * _distanceY);
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            SetStartPosition();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            if (_hasShield)
            {
                _hasShield = false;
                _shieldVisualizer.SetActive(false);
                return;
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();

            Destroy(_enemyThruster);
            Destroy(this.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            if (_hasShield)
            {
                _hasShield = false;
                _shieldVisualizer.SetActive(false);
                return;
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();

            Destroy(_enemyThruster);
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void Awake()
    {
        // Randomly assign the new movement pattern
        int count = Enum.GetValues(typeof(EnemyMovementPattern)).Length;
        int movementIndex = UnityEngine.Random.Range(0, count);
        _movementPattern = (EnemyMovementPattern)Enum.GetValues(typeof(EnemyMovementPattern)).GetValue(movementIndex);
    }

    public enum EnemyMovementPattern
    {
        Down,
        ZigZag
    }

    private void SetStartPosition()
    {
        float randomX = Random.Range(-11f, 11f);
        transform.position = new Vector3(randomX, 7f, 0);
    }

    private void CheckAndShootPickups()
    {
        float detentionRangeY = 5f;
        float horizontalRange = 0.5f;

        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Powerup");

        foreach (GameObject pickup in pickups)
        {
            if (pickup == null) continue;

            Vector3 dirToPickup = pickup.transform.position - transform.position;

            bool isAbove = dirToPickup.y > 0 && dirToPickup.y <= detentionRangeY;
            bool isHorizontallyAligned = Mathf.Abs(dirToPickup.x) <= horizontalRange;

            if (isAbove && isHorizontallyAligned)
            {
                if (Time.time > _canFire)
                {
                    _fireRate = Random.Range(1.5f, 3.0f);

                    Instantiate(_laserPrefab, transform.position, Quaternion.identity);

                }
            } 

            
        }
    }
}

