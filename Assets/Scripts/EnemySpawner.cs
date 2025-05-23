using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform planetTransform;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private float spawnDistance = 15f;
    [SerializeField] private float spawnVariance = 2f;

    private float nextSpawnTime = 0f;

    void Start()
    {
        if (planetTransform == null)
        {
            GameObject planetObj = GameObject.FindGameObjectWithTag("Planet");
            if (planetObj != null)
            {
                planetTransform = planetObj.transform;
            }
            else
            {
                Debug.LogError("EnemySpawner: Planet not found! Make sure your planet has the 'Planet' tag and is in the scene.");
                enabled = false;
                return;
            }
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Enemy Prefab not assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    void SpawnEnemy()
    {
        if (planetTransform == null || enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: PlanetTransform or EnemyPrefab not assigned!");
            enabled = false;
            return;
        }

        // The player orbits around planet.transform.forward.
        // This vector acts as the normal to the plane where enemies should spawn.
        Vector3 planeNormal = planetTransform.forward.normalized;

        // If planeNormal is Vector3.zero (e.g., planet scale is zero or forward isn't defined),
        // we can't reliably determine the plane. Fallback or log error.
        if (planeNormal == Vector3.zero)
        {
            Debug.LogError("EnemySpawner: Planet's forward vector is zero. Cannot determine spawn plane. Defaulting to old method or stopping.");
            return;
        }

        // Create a rotation that describes the orientation of the spawn plane.
        // If Z-axis aligns with planeNormal, then X and Y axes of this rotation lie *in* the plane.
        Quaternion planeOrientation = Quaternion.LookRotation(planeNormal);

        // Get two orthogonal axes that lie within the spawn plane.
        Vector3 axisInPlane1 = planeOrientation * Vector3.right;  // Effectively the "right" vector of the plane
        Vector3 axisInPlane2 = planeOrientation * Vector3.up;     // Effectively the "up" vector of the plane

        // Generate a random angle to pick a direction within the plane
        float angleInRadians = Random.Range(0f, 2f * Mathf.PI);

        // Calculate the direction in the plane using these axes and the random angle
        // (cos, sin) on a unit circle, mapped to our plane's axes
        Vector3 directionInPlane = (axisInPlane1 * Mathf.Cos(angleInRadians) + axisInPlane2 * Mathf.Sin(angleInRadians)).normalized;
        // Ensure normalization, though it should be if axisInPlane1/2 are orthonormal.

        // Calculate a slightly varied spawn distance
        float currentSpawnDistance = spawnDistance + Random.Range(-spawnVariance, spawnVariance);

        // Determine a reasonable minimum spawn distance based on planet's size
        float planetEffectiveRadius = planetTransform.localScale.y / 2f; // Default using Y scale
                                                                         // Try to get a more accurate radius from a SphereCollider if available
        if (planetTransform.TryGetComponent<SphereCollider>(out SphereCollider sc))
        {
            // Consider the largest scale component for safety with non-uniform scaling
            float maxScaleComponent = Mathf.Max(planetTransform.lossyScale.x, planetTransform.lossyScale.y, planetTransform.lossyScale.z);
            planetEffectiveRadius = sc.radius * maxScaleComponent;
        }
        // Ensure enemies spawn outside the planet's surface + a small buffer (e.g., 2 units)
        currentSpawnDistance = Mathf.Max(currentSpawnDistance, planetEffectiveRadius + 2f);

        // Determine the final spawn position
        Vector3 spawnPosition = planetTransform.position + directionInPlane * currentSpawnDistance;

        // Instantiate the enemy
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Set the enemy's target planet (important for EnemyBehavior)
        EnemyBehavior enemyBehavior = newEnemy.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null)
        {
            enemyBehavior.SetTargetPlanet(planetTransform);
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: Spawned enemy '{enemyPrefab.name}' does not have an EnemyBehavior script attached!");
        }
    }
}