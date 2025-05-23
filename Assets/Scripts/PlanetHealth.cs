using UnityEngine;
using TMPro;

public class PlanetHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private AudioClip planetHitSound;
    private int currentHealth;

    [Header("UI (Optional)")]
    [SerializeField] private TextMeshProUGUI healthText;

    public static bool isGameOver = false;

    void Start()
    {
        currentHealth = maxHealth;
        isGameOver = false;
        Time.timeScale = 1f;
        UpdateHealthUI();
        Debug.Log($"Planet health initialized: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

        if (planetHitSound != null && currentHealth < maxHealth)
        {
            AudioSource.PlayClipAtPoint(planetHitSound, Camera.main.transform.position);
        }

        UpdateHealthUI();
        Debug.Log($"Planet took {amount} damage. Current health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Planet Health: {currentHealth} / {maxHealth}";
        }
        // if (healthSlider != null)
        // {
        //     healthSlider.maxValue = maxHealth;
        //     healthSlider.value = currentHealth;
        // }
    }

    void Die()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Planet Destroyed! GAME OVER.");

        // Basic game over effect: Pause the game
        Time.timeScale = 0f;

        // Here you could also:
        // - Show a Game Over screen/UI panel
        // - Stop enemy spawners
        // - Disable player input
    }

    // Optional: A way to reset health if you implement a restart mechanism without reloading the scene
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isGameOver = false;
        Time.timeScale = 1f;
        UpdateHealthUI();
    }
}