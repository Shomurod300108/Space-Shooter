using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _bossIncomingText;
    [SerializeField] private Text _bossHealthText;
    private GameManager _gameManager;

    void Start()
    {
        _scoreText.text = "Score:" + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_bossIncomingText != null)
            _bossIncomingText.gameObject.SetActive(false);

        if (_bossHealthText != null)
            _bossHealthText.gameObject.SetActive(false);    
        
    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score:" + playerScore;
    }
    public void UpdateLives(int currentLives)
    {
    if (currentLives >= 0 && currentLives < _livesSprites.Length)
    {
        _livesImage.sprite = _livesSprites[currentLives];
    }
    else
    {
        Debug.LogWarning("Invalid currentLives value: " + currentLives);
    }

    if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _restartText.gameObject.SetActive(true);
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator ShowBossWarning()
    {
        if (_bossIncomingText == null)
            yield break;

        _bossIncomingText.gameObject.SetActive(true);

        float flickerDuration = 3.5f;
        float flickerInterval = 0.3f;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < flickerDuration)
        {
            visible = !visible;
            _bossIncomingText.enabled = visible;

            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        _bossIncomingText.enabled = true;
        yield return new WaitForSeconds(1f);

        _bossIncomingText.gameObject.SetActive(false);
    }
        

}
