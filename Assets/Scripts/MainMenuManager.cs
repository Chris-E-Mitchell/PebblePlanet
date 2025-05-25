using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private TextMeshProUGUI highScoreText;

    private const string HighScoreKey = "PebblePlanetHighScore";

    private void Start()
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

    public void StartGame()
    {
        Debug.Log("Start Game button clicked. Loading scene: " + gameSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}