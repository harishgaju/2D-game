using System.Collections;
using UnityEngine;

public class PendulumHammer : MonoBehaviour
{
    public Transform pivotPoint;  // The point where the hammer rotates around
    public float swingSpeed = 4f;  // Speed at which the hammer swings (degrees per second)
    public float swingAngle = 45f;  // Angle to swing (half of the full swing, e.g., 45 degrees left and right)
    public Transform respawnPoint;  // The point where the player will respawn after being crushed
    public float resetTime = 2f;  // Time to wait before the hammer resets its position

    private bool isCrushing = false;  // Is the hammer crushing the player?
    private Rigidbody2D rb;  // Rigidbody2D of the hammer
    private bool isResetting = false;  // Is the hammer resetting after crushing?

    void Start()
    {
        // Get the Rigidbody2D attached to the hammer
        rb = GetComponent<Rigidbody2D>();
        // Set the Rigidbody2D to Kinematic so we can control it manually
        rb.isKinematic = true;

        // Automatically start the pendulum swing when the game starts
        SwingHammer();
    }

    void Update()
    {
        if (!isCrushing && !isResetting)
        {
            // Keep the hammer swinging back and forth automatically
            SwingHammer();
        }
    }

    // Swing the hammer back and forth
    private void SwingHammer()
    {
        // Apply a rotation to the hammer to simulate the pendulum swing
        float swingDirection = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = Quaternion.Euler(0, 0, swingDirection);
    }

    // Detect collision with the player and handle respawn or damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCrushing)
        {
            isCrushing = true;
            // Respawn the player when crushed
            RespawnPlayer(collision.gameObject);
            // Optionally, reset the hammer after a delay
            StartCoroutine(ResetHammer());
        }
    }

    // Respawn the player at the designated respawn point
    private void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPoint.position;
        // You can also reset player health here if needed
        // Example: player.GetComponent<PlayerHealth>().ResetHealth();
    }

    // Reset the hammer after it crushes the player
    private IEnumerator ResetHammer()
    {
        // Wait for a few seconds before resetting the hammer
        yield return new WaitForSeconds(resetTime);
        // Reset the hammer to its initial position
        isResetting = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(1f); // Wait a bit for the hammer to settle
        isResetting = false;
        isCrushing = false;
    }
}
