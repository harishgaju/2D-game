using System.Collections;
using UnityEngine;

public class PendulumPlatform : MonoBehaviour
{

    public Transform pivotPoint;
    public float swingSpeed = 2f;
    public float swingAngle = 30f;
    public float resetTime = 2f;

    public GameObject touchParticlePrefab;

    private Rigidbody2D rb;
    private bool isResetting = false;
    private bool isPlayerOnPlatform = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        SwingPlatform();
    }

    void Update()
    {
        if (!isResetting && !isPlayerOnPlatform)
        {
            SwingPlatform();
        }
    }

    private void SwingPlatform()
    {
        float swingDirection = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = Quaternion.Euler(0, 0, swingDirection);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            other.transform.SetParent(transform);
            other.transform.rotation = Quaternion.identity;
            if (touchParticlePrefab != null)
            {
                GameObject particle = Instantiate(touchParticlePrefab, other.transform.position, Quaternion.identity);
                Destroy(particle, 2f); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            other.transform.SetParent(null);
            ResetPlayerRotation(other);
            StartCoroutine(ResetPlatformRotation());
        }
    }

    private void ResetPlayerRotation(Collider2D player)
    {
        player.transform.rotation = Quaternion.identity;
    }

    private IEnumerator ResetPlatformRotation()
    {
        isResetting = true;

        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTime < resetTime)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / resetTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isResetting = false;
    }
}
