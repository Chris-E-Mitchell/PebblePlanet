using UnityEngine;

public class PlayerOrbitMovement : MonoBehaviour
{
    [Header("Planet Settings")]
    [SerializeField] private Transform planet;
    [SerializeField] private float planetRadius = 2.5f;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 50f;
    [SerializeField] private float playerOffsetFromSurface = 0.5f;

    [Header("Player Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform firePoint;         
    [SerializeField] private float fireRate = 2f;         
    [SerializeField] private float nextFireTime = 0f;
    [SerializeField] private AudioClip shootSound;

    [Header("Smart Bomb Settings")]
    [SerializeField] private float smartBombRadius = 15f; // Radius from planet center
    [SerializeField] private AudioClip smartBombUseSound;
    [SerializeField] private AudioClip smartBombEarnSound; // Optional
    [SerializeField] private GameObject smartBombVisualEffectPrefab; // Optional visual effect for the bomb

    void Start()
    {
        if (planet == null)
        {
            Debug.LogError("Planet Transform not assigned to PlayerOrbitMovement script!");
            enabled = false;
            return;
        }

        transform.position = planet.position + new Vector3(0, planetRadius + playerOffsetFromSurface, 0);

        Vector3 directionToPlanetCenter = (planet.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, -directionToPlanetCenter) * transform.rotation;

    }

    void Update()
    {
        if (planet == null) return;

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 rotationAxis = planet.transform.forward;
        float angle = -horizontalInput * movementSpeed * Time.deltaTime;
        transform.RotateAround(planet.position, rotationAxis, angle);

        Vector3 vectorFromPlanetToPlayer = (transform.position - planet.position).normalized;
        transform.up = vectorFromPlanetToPlayer;

        transform.position = planet.position + vectorFromPlanetToPlayer * (planetRadius + playerOffsetFromSurface);

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (GameManager.Instance.GetSmartBombsRemaining() > 0)
            {
                UseSmartBomb();
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile Prefab or Fire Point not assigned in PlayerOrbitMovement script.");
            return;
        }

        Quaternion projectileRotation = Quaternion.LookRotation(transform.up);

        if (muzzleFlashPrefab != null) // << ADD THIS BLOCK
        {
            Instantiate(muzzleFlashPrefab, firePoint.position, projectileRotation);
        }

        Instantiate(projectilePrefab, firePoint.position, projectileRotation);

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position);
        }
    }

    private void UseSmartBomb()
    {
        GameManager.Instance.RemoveSmartBomb();
        Debug.Log("SMART BOMB USED! Remaining: " + GameManager.Instance.GetSmartBombsRemaining());

        ActivateSmartBombEffect();

        if (smartBombUseSound != null)
        {
            AudioSource.PlayClipAtPoint(smartBombEarnSound, Camera.main.transform.position);
        }
    }

    private void ActivateSmartBombEffect()
    {
        // Smart bomb visual
        if (smartBombVisualEffectPrefab != null)
        {
            Instantiate(smartBombVisualEffectPrefab, planet.position, Quaternion.identity);
        }

        // Find all objects tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemiesDestroyed = 0;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distanceToPlanetCenter = Vector3.Distance(enemy.transform.position, planet.position);

            if (distanceToPlanetCenter <= smartBombRadius)
            {
                EnemyBehavior eb = enemy.GetComponent<EnemyBehavior>();
                if (eb != null)
                {
                    eb.DestroyEnemy(0);
                    enemiesDestroyed++;
                }
            }
        }
        Debug.Log($"Smart Bomb destroyed {enemiesDestroyed} enemies.");
    }
}