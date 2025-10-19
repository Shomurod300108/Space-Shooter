using UnityEngine;

public class Enemy_Seeker : Enemy
{
    [SerializeField]
    private GameObject _seekerMissilePrefab;
    [SerializeField]
    private float _fireRateSeeker = 4f;
    private float _nextFire = 0f;
    private float _canFireSeeker = -1f;
    [SerializeField]
    private float _zigzagFrequency = 2f;
    [SerializeField]
    private float _zigzagAmplitude = 1.5f;
    private float _startX;

    protected override void Start()
    {
        base.Start();
        _startX = transform.position.x;

    }

    protected override void Update()
    {
        base.Update();
        if (Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            FireSeekerMissile();
        }
    }

    private void FireSeekerMissile()
    {
        if (Time.time > _canFireSeeker)
        {
            _canFireSeeker = Time.time + _fireRateSeeker;
            Instantiate(_seekerMissilePrefab, transform.position, transform.rotation);
        }
    }

    protected override void CalculateMovement()
    {
        // Move down smoothly
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Add horizontal zig-zag motion
        float newX = Mathf.Sin(Time.time * _zigzagFrequency) * _zigzagAmplitude;
        transform.position = new Vector3(newX + _startX, transform.position.y, 0);

        // Respawn when off screen
        if (transform.position.y < -6f)
        {
            float randomX = Random.Range(-11f, 11f);
            _startX = randomX; // reset starting point
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Player player = other.GetComponent<Player>();
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

        Destroy(gameObject); 
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }
    else if (other.CompareTag("Laser"))
    {
        Player player = other.GetComponent<Player>();
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

        Destroy(gameObject); 
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }
}


}

