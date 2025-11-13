using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingplatform : MonoBehaviour
{
    // Move Timing Settings
    public float shakeDuration = 0.5f;
    public float shake = 0.1f;
    public float fallDelay = 0.5f;

    // FX & Audio
    public AudioClip shakeSound;
    public ParticleSystem fallParticles;

    private Rigidbody2D rb;
    private bool hasFallen = false;
    private Vector3 originalLocalPosition;
    private AudioSource audioSource;

    // Static list to store all platforms
    private static List<fallingplatform> allPlatforms = new List<fallingplatform>();

    private void Awake()
    {
        allPlatforms.Add(this);
    }

    private void OnDestroy()
    {
        allPlatforms.Remove(this);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        originalLocalPosition = transform.localPosition;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasFallen && collision.gameObject.CompareTag("Player"))
        {
            hasFallen = true;
            if (fallParticles != null)
                fallParticles.Play();

            StartCoroutine(ShakeAndFall());
        }
    }

    private IEnumerator ShakeAndFall()
    {
        if (shakeSound != null)
            audioSource.PlayOneShot(shakeSound);

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shake;
            float y = Random.Range(-1f, 1f) * shake;

            transform.localPosition = originalLocalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;

        yield return new WaitForSeconds(fallDelay);

        if (fallParticles != null)
            fallParticles.Play();

        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // Reset logic
    public static void ResetAllPlatforms()
    {
        foreach (fallingplatform platform in allPlatforms)
        {
            platform.ResetPlatform();
        }
    }

    private void ResetPlatform()
    {
        StopAllCoroutines();
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.localPosition = originalLocalPosition;
        rb.linearVelocity = Vector2.zero;
        hasFallen = false;

        if (fallParticles != null)
            fallParticles.Stop();
    }
}
