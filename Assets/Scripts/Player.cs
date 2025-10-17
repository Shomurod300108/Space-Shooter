using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 7.0f;
    private float _speedMultiplier = 2.0f;
    public float _sprintMultiplier = 1.5f;
    [SerializeField]
    private GameObject _laserPreFab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private Spawn_Manager _spawnManager;
    private bool _isTripleShotActive = false;
    [SerializeField]
    private int _shieldHealth = 3;
    private bool _isShieldActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isAmmoRefillBoostActive = false;
    private bool _isAddLivesActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    private SpriteRenderer _shieldRenderer;
    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private Slider _thrusterSlider;
    [SerializeField]
    private float _maxThruster = 100f;
    [SerializeField]
    private float _currentThruster = 100f;
    [SerializeField]
    private float _thrusterUseRate = 10f;
    [SerializeField]
    private float _thrusterRechargeRate = 8f;
    [SerializeField]
    private int _ammoCount = 15;
    private Text _ammoCountText;
    [SerializeField]
    private bool _isAmmoEmpty = false;
    [SerializeField]
    private Animator _cameraAnim;
    private bool _isSlowDownActive = false;
    private float _slowdownRate = 0.5f;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _ammoCountText = GameObject.Find("Ammo_Count_Text").GetComponent<Text>();
        _cameraAnim = GameObject.Find("Main Camera").GetComponent<Animator>();

        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI manager is null");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        if (_thrusterSlider != null)
        {
            _thrusterSlider.maxValue = _maxThruster;
            _thrusterSlider.value = _currentThruster;
        }
        else 
        {
            Debug.LogError("The thruster slider is null");
        }

        if (_shieldVisualizer != null)
        {
            _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
            Debug.Log("Shield Renderer assigned: " + _shieldRenderer);
        }

        if (_ammoCountText == null)
        {
            Debug.LogError("Ammo count text is null");
        }
    }

    void Update()
    {
        CalculateMovement();
        CheckFireInput();
    }

    void CheckFireInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (Input.GetKey(KeyCode.LeftShift))
        {
        transform.Translate(Vector3.up * verticalInput * _speed * _sprintMultiplier * Time.deltaTime);
        transform.Translate(Vector3.right * horizontalInput * _sprintMultiplier * _speed * Time.deltaTime);

        _currentThruster -= _thrusterUseRate * Time.deltaTime;
        }
        else
        {
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);

        _currentThruster += _thrusterRechargeRate * Time.deltaTime;
        }

        

        if (_thrusterSlider != null)
        {
            _thrusterSlider.value = _currentThruster;
        }

        if (transform.position.y >= 6)
        {
            transform.position = new Vector3(transform.position.x, 6, 0);
        }
        else if (transform.position.y <= -3.97f)
        {
            transform.position = new Vector3(transform.position.x, -3.97f, 0);
        }

        if (transform.position.x > 11f)
        {
            transform.position = new Vector3(-11f, transform.position.y, 0);
        }
        else if (transform.position.x < -11f)
        {
            transform.position = new Vector3(11f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isAmmoEmpty == false && _isTripleShotActive == false)
        {
            Instantiate(_laserPreFab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _ammoCount--;
            _ammoCountText.text = "Ammo: " + _ammoCount;

            if (_ammoCount <= 0)
            {
                _isAmmoEmpty = true;
                _ammoCountText.text = "Ammo: EMPTY";
            }
            else
            {
                _isAmmoEmpty = false;
            }
        }
        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldHealth--;
            UpdateShieldColor();
            
            if (_shieldHealth < 1)
            {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            }
        
            return;
        }

        _lives = _lives - 1;
        _cameraAnim.SetTrigger("OnPlayerDamage");

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }

        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldHealth = 3;
        _shieldVisualizer.SetActive(true);
        UpdateShieldColor();
    }

    public void AmmoRefillBoostActive()
    {
        _isAmmoRefillBoostActive = true;
        _ammoCount = 15;
        _isAmmoEmpty = false;
        _ammoCountText.text = "Ammo: " + _ammoCount;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AddLivesActive()
    {
        _isAddLivesActive = true;
        _lives++;
        _lives = Mathf.Clamp(_lives, 0, 3);

        if (_lives == 2)
        {
            _rightEngine.SetActive(false);
            _uiManager.UpdateLives(_lives);
        }

        if (_lives == 3)
        {
            _leftEngine.SetActive(false);
            _uiManager.UpdateLives(_lives);
        }
    }
    
    public void SlowDownActive()
    {
        _isSlowDownActive = true;
        _speed *= _slowdownRate;
        StartCoroutine(SlowDownPowerDownRoutine());
    }

    IEnumerator SlowDownPowerDownRoutine()
    {
         yield return new WaitForSeconds(5.0f);
        _isSlowDownActive = false;
        _speed /= _slowdownRate;
    }
        

    private void UpdateShieldColor()
    {
        if (_shieldRenderer == null)
        {
            Debug.LogError("Shield Renderer is not assigned.");
            return;
        }

        switch (_shieldHealth)
        {
            case 3:
                _shieldRenderer.color = Color.cyan;
                break;
            case 2:
                _shieldRenderer.color = Color.yellow;
                break;
            case 1:
                _shieldRenderer.color = Color.red;
                break;

            default:
                _shieldRenderer.color = Color.clear;
                break;
        }

    }

   
}
