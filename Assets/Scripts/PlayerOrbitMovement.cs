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
    [SerializeField] private Transform firePoint;         
    [SerializeField] private float fireRate = 2f;         
    [SerializeField] private float nextFireTime = 0f;

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
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile Prefab or Fire Point not assigned in PlayerOrbitMovement script.");
            return;
        }

        Quaternion projectileRotation = Quaternion.LookRotation(transform.up);

        Instantiate(projectilePrefab, firePoint.position, projectileRotation);
    }
}