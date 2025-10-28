using UnityEngine;

public class Projectile_Powerup : MonoBehaviour
{
    [Header("Powerup")]
    [SerializeField] private float durationSeconds = 10f;
    [SerializeField] private AudioClip _pickupSfx;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _bottomY = -6f;

    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _bottomY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(_playerTag)) return;

        Player player = other.GetComponent<Player>(); 
        if (player != null)
        {
            Debug.Log("Projectile Powerup collected");
            player.ActivateHoming();
            if (_pickupSfx) AudioSource.PlayClipAtPoint(_pickupSfx, transform.position);
        }

        Destroy(gameObject);
    }
}

