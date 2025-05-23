using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform targetPlanet;
    [SerializeField] private int enemyDamage = 1;
    [SerializeField] private int enemyPoints = 10;

    void Start()
    {
        // Fallback if targetPlanet is not assigned by spawner
        if (targetPlanet == null)
        {
            GameObject planetObj = GameObject.FindGameObjectWithTag("Planet");
            if (planetObj != null)
            {
                targetPlanet = planetObj.transform;
            }
            else
            {
                Debug.LogError("EnemyBehavior: Planet not found! Make sure your planet has the 'Planet' tag.");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        if (targetPlanet == null) return;

        Vector3 directionToPlanet = (targetPlanet.position - transform.position).normalized;
        transform.position += directionToPlanet * speed * Time.deltaTime;

        transform.LookAt(targetPlanet.position);
    }

    
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Enemy hit by projectile!");

            Destroy(gameObject);
            Destroy(collision.gameObject);

            if (GameManager.Instance != null && !PlanetHealth.isGameOver)
            {
                GameManager.Instance.AddScore(enemyPoints);
                Debug.Log("Adding enemy points:" + enemyPoints);
            }
        }
        else if (collision.gameObject.CompareTag("Planet"))
        {
            if (PlanetHealth.isGameOver) // Don't do anything if game is already over
            {
                Destroy(gameObject); // Just disappear if game over
                return;
            }

            Debug.Log("Enemy reached the planet!");
            PlanetHealth planetHealthComponent = collision.gameObject.GetComponent<PlanetHealth>();
            if (planetHealthComponent != null)
            {
                planetHealthComponent.TakeDamage(enemyDamage);
            }
            else
            {
                Debug.LogWarning("Enemy collided with Planet, but PlanetHealth component was not found!");
            }
            Destroy(gameObject); // Destroy the enemy after it hits the planet
        }
    }

    public void SetTargetPlanet(Transform targetPlanet)
    {
        this.targetPlanet = targetPlanet;
    }
}