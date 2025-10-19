using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 3.0f;
    protected Player _player;
    protected Animator _anim;
    protected AudioSource _audioSource;
    [SerializeField]
    protected GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    protected float _fireRate = 3.0f;
    private float _canFire = -1f;
    private bool _isEnemyLaser = true;
    [SerializeField]
    private EnemyMovementPattern _movementPattern;
    private float _spawnTime;
    private float _frequency;
    private float _phase;
    private float _distanceY;
    [SerializeField]
    protected GameObject _shieldVisualizer;
    protected bool _hasShield = false;

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
        CalculateMovement();
        FireRoutine();
    }

    protected virtual void FireRoutine()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    protected virtual void CalculateMovement()
    {
        if (_movementPattern == EnemyMovementPattern.Down)
        {
            MoveDown();
        }
        else
        {
            MoveZigZagDown();
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

    private void OnTriggerEnter2D(Collider2D other)
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
}

