using System.Collections;
using UnityEngine;

public class Player_Missile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _rotateSpeed = 360f;
    [SerializeField] private float _lifeTime = 6f;
    [SerializeField] private string _targetTag = "Enemy";
    [SerializeField] private float _targetSearchInterval = 0.15f; // how often to search for closest target
    [SerializeField] private float _maxTargetSearchDistance = 12f; // ignore very far targets
    [SerializeField] private int _damage = 1;
    private Rigidbody2D _rb;
    private Transform _target;
    private float _spawnTime;
    [SerializeField] private GameObject _hitVFX;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _spawnTime = Time.time;
        _target = null;
        // start searching a tiny bit delayed so multiple projectiles don't cost too much
        InvokeRepeating(nameof(FindClosestTarget), 0f, _targetSearchInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(FindClosestTarget));
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (Time.time - _spawnTime > _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (_target != null)
        {
            Vector2 direction = (Vector2)(_target.position - transform.position);
            if (direction.sqrMagnitude < 0.01f)
            {
                _rb.velocity = Vector2.zero;
            }
            else
            {
                float desiredAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, desiredAngle, _rotateSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
                _rb.velocity = transform.up * _speed;
            }
        }
        else
        {
            _rb.velocity = transform.up * _speed;
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(_targetTag);
        Transform best = null;
        float bestSqr = float.MaxValue;
        Vector3 pos = transform.position;

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float sqr = (e.transform.position - pos).sqrMagnitude;
            if (sqr < bestSqr && sqr <= _maxTargetSearchDistance * _maxTargetSearchDistance)
            {
                bestSqr = sqr;
                best = e.transform;
            }
        }

        _target = best;
    }

    private void OnTriggerEnter2D(Collider2D other)
 {
    if (other.CompareTag(_targetTag))
    {
        // Try to get the EnemyShield first
        Transform shield = other.transform.Find("Shields");
        if (shield != null)
        {
            Destroy(shield.gameObject); // destroy child shield first
        }
        else
        {
            // Otherwise get the Enemy component
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                    Destroy(enemy.gameObject);
            }
        }

        if (_hitVFX != null)
            Instantiate(_hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
 }
}




















