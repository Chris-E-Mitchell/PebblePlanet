using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const string HighScoreKey = "PebblePlanetHighScore";

    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    [Header("Score Settings")]
    [SerializeField] private int score = 0;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject newHighScoreText;
    [SerializeField] private TextMeshProUGUI smartBombText;

    [Header("Smart Bomb Settings")]
    [SerializeField] private int smartBombs = 3;
    [SerializeField] private int scoreForNewBomb = 200;
    [SerializeField] private AudioClip smartBombEarnSound;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        UpdateScoreUI();
        UpdateSmartBombUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            if(newHighScoreText != null)
            {
                newHighScoreText.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (PlanetHealth.isGameOver)
        {
            if (gameOverPanel != null && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
                CheckAndSaveHighScore();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                GoToMainMenu();
            }
        }
    }

    public void AddScore(int amount)
    {
        if (PlanetHealth.isGameOver) return;

        score += amount;
        UpdateScoreUI();

        // Terrible way of deteriming if new bomb should be added - only works if exact score multiple is achieved
        if ((score % scoreForNewBomb) == 0)
        {
            AddSmartBomb();
            if (smartBombEarnSound != null)
            {
                AudioSource.PlayClipAtPoint(smartBombEarnSound, Camera.main.transform.position);
            }
            Debug.Log("New smart bomb awarded!");
        }
    }

    public void RemoveSmartBomb()
    {
        if (smartBombs > 0)
        {
            smartBombs--;
            UpdateSmartBombUI();
        }
    }

    public void AddSmartBomb()
    {
        smartBombs++;
        UpdateSmartBombUI();
    }

    public int GetSmartBombsRemaining()
    {
        return smartBombs;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    void UpdateSmartBombUI()
    {
        if (smartBombText != null)
        {
            smartBombText.text = $"Smart Bombs: {smartBombs}";
        }
    }

    public void HandleGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void CheckAndSaveHighScore()
    {
        int currentSavedHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (score > currentSavedHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
            if (newHighScoreText != null)
            {
                newHighScoreText.SetActive(true);
            }
            Debug.Log($"New High Score Saved: {score} (Previous: {currentSavedHighScore})");
        }
        else
        {
            Debug.Log($"Current score {score} did not beat the High Score {currentSavedHighScore}");
        }
    }
}