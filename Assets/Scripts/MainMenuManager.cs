using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private TextMeshProUGUI highScoreText;

    private const string HighScoreKey = "PebblePlanetHighScore";

    private void Start()
    {
        UpdateHighScoreText();
    }

    public void Update()
    {
        // Delete High Score for Testing
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("Start Game button clicked. Loading scene: " + gameSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            int savedHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            highScoreText.text = $"High Score: {savedHighScore}";
        }
        else
        {
            Debug.LogWarning("MainMenuManager: High Score Text UI element not assigned.");
        }
    }
}