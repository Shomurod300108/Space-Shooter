using UnityEngine;

public class Seeker_Missile : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 200f;
    private Rigidbody2D _rb;
    private Transform _target;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindWithTag("Player")?.transform;

        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        if (_target == null) return;

        Vector2 direction = (Vector2)_target.position - _rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        _rb.angularVelocity = -rotateAmount * _rotateSpeed;

        _rb.velocity = transform.up * _speed;
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
            Destroy(this.gameObject);
        }
    }
}
