using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int score = 0;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI smartBombText;

    [Header("Smart Bomb Settings")]
    [SerializeField] private int smartBombs = 3;
    [SerializeField] private int scoreForNewBomb = 200;

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
        }
    }

    void Update()
    {
        if (PlanetHealth.isGameOver)
        {
            if (gameOverPanel != null && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    public void AddScore(int amount)
    {
        if (PlanetHealth.isGameOver) return;

        score += amount;
        UpdateScoreUI();

        if ((score % scoreForNewBomb) == 0)
        {
            AddSmartBomb();
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

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}