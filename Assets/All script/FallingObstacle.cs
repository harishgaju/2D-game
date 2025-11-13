using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    [Header("Fall Settings")]
    public float distance = 5f;
    public float gravityAfterFall = 5f;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    [Header("FX")]
    public ParticleSystem blastParticle;
    public AudioClip blastSoundClip;

    private Rigidbody2D rb;
    private bool isFalling = false;
    private Vector3 originalPosition;
    private AudioSource audioSource;

    // Static list to track all obstacles
    private static List<FallingObstacle> allObstacles = new List<FallingObstacle>();

    private void OnEnable()
    {
        allObstacles.Add(this);
    }

    private void OnDisable()
    {
        allObstacles.Remove(this);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Missing Rigidbody2D on " + gameObject.name);
            return;
        }

        rb.gravityScale = 0f;
        originalPosition = transform.position;

        if (blastParticle != null)
            blastParticle.Stop();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = blastSoundClip;
    }

    private void Update()
    {
        Physics2D.queriesStartInColliders = false;

        if (isFalling) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance);
        Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            StartCoroutine(ShakeAndFall());
            isFalling = true;
        }
    }

    private IEnumerator ShakeAndFall()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = originalPosition + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;

        if (blastParticle != null)
            blastParticle.Play();

        if (audioSource != null && blastSoundClip != null)
            audioSource.Play();

        rb.gravityScale = gravityAfterFall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling) return;

        if (collision.collider.CompareTag("Player"))
        {
            if (blastParticle != null)
                blastParticle.Play();

            if (audioSource != null && blastSoundClip != null)
                audioSource.Play();

            var playerDissolve = collision.collider.GetComponent<dissolve>();
            if (playerDissolve != null && respawncontroller.instance != null)
            {
                playerDissolve.TriggerRespawn(respawncontroller.instance.respawnpoint.position);
            }
            else if (respawncontroller.instance != null)
            {
                collision.transform.position = respawncontroller.instance.respawnpoint.position;
            }

            // Reset all obstacles immediately
            ResetAllObstacles();
        }
    }

    private void ResetObstacleImmediate()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = originalPosition;
        isFalling = false;
    }

    private static void ResetAllObstacles()
    {
        foreach (var obstacle in allObstacles)
        {
            obstacle.ResetObstacleImmediate();
        }
    }
}
