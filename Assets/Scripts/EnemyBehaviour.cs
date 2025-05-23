using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform targetPlanet; 

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
        }
        else if (collision.gameObject.CompareTag("Planet"))
        {
            Debug.Log("Enemy reached the planet!");
            Destroy(gameObject); 
            
            // Add damage to planet logic here
        }
    }

    public void SetTargetPlanet(Transform targetPlanet)
    {
        this.targetPlanet = targetPlanet;
    }
}