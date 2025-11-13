using System.Collections;
using UnityEngine;

public class RandomCrushPlatform : MonoBehaviour
{
    [Header("Crush Settings")]
    public float crushSpeed = 5f;
    public float crushDistance = 3f;

    [Header("Reset Settings")]
    public float resetDelay = 1.5f;

    [Header("Crush Trigger Zone")]
    public Collider2D crushZoneTrigger; // Assign in Inspector
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource audioSource; // Assign AudioSource component in Inspector
    public AudioClip crushSound;    // Assign crush sound clip in Inspector

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isCrushing = false;
    private bool isResetting = false;

    // Shared across all platforms
    private static bool isPlayerCrushing = false;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos - Vector3.up * crushDistance;

        if (crushZoneTrigger != null)
            crushZoneTrigger.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only start crushing when player enters the trigger zone
        if (!isCrushing && !isResetting && other.CompareTag(playerTag))
        {
            StartCoroutine(StartCrushSequence());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only crush if platform is moving and hits the player
        if (isCrushing && collision.collider.CompareTag(playerTag))
        {
            CrushPlayer(collision.gameObject);
        }
    }

    private IEnumerator StartCrushSequence()
    {
        isCrushing = true;

        yield return new WaitForSeconds(0.5f);

        if (crushZoneTrigger != null)
            crushZoneTrigger.enabled = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, crushSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(resetDelay);

        ResetPlatform();
    }

    private void CrushPlayer(GameObject player)
    {
        if (isPlayerCrushing) return;

        isPlayerCrushing = true;

        // Play crush sound
        if (audioSource != null && crushSound != null)
        {
            audioSource.PlayOneShot(crushSound);
        }

        dissolve dissolveScript = player.GetComponent<dissolve>();
        if (dissolveScript != null)
        {
            Vector3 respawnPos = respawncontroller.instance.respawnpoint.position;
            dissolveScript.TriggerRespawn(respawnPos);
        }
        else
        {
            Destroy(player);
        }

        Debug.Log("Player crushed!");
        StartCoroutine(ResetPlayerCrushingFlag());
    }

    private IEnumerator ResetPlayerCrushingFlag()
    {
        yield return new WaitForSeconds(1f);
        isPlayerCrushing = false;
    }

    private void ResetPlatform()
    {
        isCrushing = false;
        isResetting = false;

        if (crushZoneTrigger != null)
            crushZoneTrigger.enabled = false;

        transform.position = startPos;
    }
}
