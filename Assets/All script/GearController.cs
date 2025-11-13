using System.Collections;
using UnityEngine;

public class GearController : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 180f;

    [Header("Chasing & Scaling")]
    public float activationDistance = 10f;
    public float maxScale = 3f;
    public float minScale = 1f;
    public float scaleSmoothSpeed = 3f;

    [Header("Chase Settings")]
    public float waitAfterCrossSeconds = 2f; // Wait time after player crosses gear before moving to player
    public float chaseSpeed = 2f;
    public float resetSpeed = 3f; // Speed to return to original position after chasing

    [Header("Audio")]
    public AudioClip hitSound;           // Assign this in inspector with your hit sound effect
    private AudioSource audioSource;

    private Transform player;
    private Vector3 originalScale;
    private Vector3 originalPosition;  // Store original position
    private dissolve playerDissolve;
    private Vector3 lastPlayerPosition;

    private bool hasScaledUp = false;
    private bool hasScaledDown = false;
    private bool hasCrossedGear = false;
    private bool isChasing = false;
    private bool isResetting = false;

    // Static event to notify all gears when any gear hits the player
    public static event System.Action OnAnyGearHitPlayer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerDissolve = playerObj.GetComponent<dissolve>();
            lastPlayerPosition = player.position;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found!");
        }

        originalScale = transform.localScale;
        originalPosition = transform.position;  // Save original position

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Add AudioSource component if not present
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        OnAnyGearHitPlayer += HandleResetAllGears;
    }

    private void OnDisable()
    {
        OnAnyGearHitPlayer -= HandleResetAllGears;
    }

    private void Update()
    {
        // Rotate gear constantly
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (player == null)
            return;

        if (isResetting)
        {
            ResetGearPosition();
            return; // Skip other logic during reset
        }

        float playerDeltaX = player.position.x - lastPlayerPosition.x;
        lastPlayerPosition = player.position;

        float distance = Mathf.Abs(transform.position.x - player.position.x);
        bool playerNear = distance <= activationDistance;

        if (!playerNear)
        {
            ResetFlagsAndScale();
            return;
        }

        // Scaling logic only when not chasing
        if (!isChasing)
        {
            if (playerDeltaX > 0f && !hasScaledUp && !hasScaledDown)
            {
                hasScaledUp = true;
                hasScaledDown = false;

                Vector3 targetScale = new Vector3(maxScale, maxScale, maxScale);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSmoothSpeed);
            }
            else if (playerDeltaX < 0f && hasScaledUp && !hasScaledDown)
            {
                hasScaledDown = true;
                hasScaledUp = false;

                Vector3 targetScale = new Vector3(minScale, minScale, minScale);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSmoothSpeed);
            }
            else
            {
                if (!hasCrossedGear && player.position.x > transform.position.x)
                {
                    hasCrossedGear = true;
                    StartCoroutine(WaitThenChase());
                }

                Vector3 targetScale = originalScale;
                if (hasScaledUp) targetScale = new Vector3(maxScale, maxScale, maxScale);
                if (hasScaledDown) targetScale = new Vector3(minScale, minScale, minScale);

                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSmoothSpeed);
            }
        }
        else
        {
            // Chase player
            MoveGearTowardsPlayer();

            // If close enough to player, reset
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                StartReset();
            }

            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSmoothSpeed);
        }
    }

    private IEnumerator WaitThenChase()
    {
        yield return new WaitForSeconds(waitAfterCrossSeconds);
        isChasing = true;
    }

    private void MoveGearTowardsPlayer()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(player.position.x, currentPos.y, currentPos.z);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, chaseSpeed * Time.deltaTime);
    }

    private void ResetGearPosition()
    {
        // Move gear smoothly back to original position
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, resetSpeed * Time.deltaTime);

        // Reset scale smoothly
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSmoothSpeed);

        // When gear reached original position, stop resetting and clear flags
        if (Vector3.Distance(transform.position, originalPosition) < 0.01f)
        {
            isResetting = false;
            ResetFlagsAndScale();
        }
    }

    private void ResetFlagsAndScale()
    {
        hasScaledUp = false;
        hasScaledDown = false;
        hasCrossedGear = false;
        isChasing = false;
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSmoothSpeed);
    }

    private void StartReset()
    {
        isChasing = false;
        isResetting = true;
    }

    private void HandleResetAllGears()
    {
        // Called on event to reset this gear
        StartReset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerDissolve != null)
        {
            Vector3 latestRespawnPoint = respawncontroller.instance != null && respawncontroller.instance.respawnpoint != null
                ? respawncontroller.instance.respawnpoint.position
                : Vector3.zero;

            playerDissolve.TriggerRespawn(latestRespawnPoint);

            // Play hit sound if assigned
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Invoke event so ALL gears reset
            OnAnyGearHitPlayer?.Invoke();
        }
    }
}
