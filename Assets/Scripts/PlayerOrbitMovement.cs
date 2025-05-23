using UnityEngine;

public class PlayerOrbitMovement : MonoBehaviour
{
    [Header("Planet Settings")]
    [SerializeField] private Transform planet;
    [SerializeField] private float planetRadius = 2.5f;

    [Header("Player Movement")]
    [SerializeField] private float movementSpeed = 50f;
    [SerializeField] private float playerOffsetFromSurface = 0.5f;

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

        if (Mathf.Approximately(horizontalInput, 0f))
        {
            return;
        }

        Vector3 rotationAxis = planet.transform.forward;
        float angle = -horizontalInput * movementSpeed * Time.deltaTime;
        transform.RotateAround(planet.position, rotationAxis, angle);

        Vector3 vectorFromPlanetToPlayer = (transform.position - planet.position).normalized;
        transform.up = vectorFromPlanetToPlayer;

        transform.position = planet.position + vectorFromPlanetToPlayer * (planetRadius + playerOffsetFromSurface);
    }
}