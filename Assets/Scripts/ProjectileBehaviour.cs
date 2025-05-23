using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("ProjectileBehavior requires a Rigidbody component!");
            Destroy(gameObject);
            return;
        }

        rb.linearVelocity = transform.forward * speed;

        // Destroy the projectile after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Projectile hit: " + collision.gameObject.name);
    //    Destroy(gameObject);
    //}
}