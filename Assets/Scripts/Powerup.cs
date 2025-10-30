using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID;
    [SerializeField] private AudioClip _clip;
    private Transform _player;
    private float _moveSpeed = 6.0f;
    private bool _isAttracted = false;
    
    void Start()
    {
        _player = GameObject.Find("Player").transform;
    }
    
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -6f)
        {
            Destroy(this.gameObject);
        }

        if (_isAttracted && _player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, _moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                        case 3:
                        player.AmmoRefillBoostActive();
                        break;
                    case 4:
                        player.AddLivesActive();
                        break;
                    case 5:
                        player.SlowDownActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }

            }
            Destroy(this.gameObject);
        }
            
    }

    public void StartAttraction(float speed)
    {
      _isAttracted = true;
      _moveSpeed = speed;
    }
}
