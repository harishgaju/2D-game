using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public enum RocketType { Vertical, ZigZag }
    public enum MoveDirection { Up, Down }

    [Header("Rocket Mode")]
    public RocketType rocketType = RocketType.Vertical;

    [Header("Vertical Movement Settings")]
    public MoveDirection moveDirection = MoveDirection.Down;
    public float speed = 5f;
    public float travelDistance = 6f;

    [Header("ZigZag Settings")]
    [Tooltip("True = vertical zigzag, False = horizontal")]
    public bool zigzagMoveVertical = false;
    public float zigzagSegmentLength = 1f;
    public float zigzagAngle = 45f;

    [Header("Audio Settings")]
    public AudioClip rocketHitSound;

    [Header("Particle Effects")]
    public ParticleSystem rocketTrail;

    // Internal components
    private AudioSource audioSource;
    private dissolve playerDissolve;
    private Vector3 startPosition;

    // ZigZag logic
    private float segmentProgress = 0f;
    private bool goingUp = true;
    private bool movingForward = true;
    private int backwardIndex = -1;
    private readonly List<Vector3> zigZagPath = new();

    void Start()
    {
        startPosition = transform.position;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerDissolve = player.GetComponent<dissolve>();

        audioSource = GetComponent<AudioSource>();

        if (rocketTrail != null)
            rocketTrail.Play();
    }

    void Update()
    {
        if (rocketType == RocketType.Vertical)
        {
            MoveVertical();
            CheckVerticalReset();
        }
        else
        {
            MoveZigZagLoop();
        }
    }

    #region Vertical Movement

    void MoveVertical()
    {
        Vector3 direction = (moveDirection == MoveDirection.Up) ? Vector3.up : Vector3.down;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void CheckVerticalReset()
    {
        if (Vector3.Distance(transform.position, startPosition) >= travelDistance)
            transform.position = startPosition;
    }

    #endregion

    #region ZigZag Movement

    void MoveZigZagLoop()
    {
        if (movingForward)
            MoveZigZagForward();
        else
            MoveZigZagBackward();
    }

    void MoveZigZagForward()
    {
        Vector3 direction = CalculateZigZagDirection(goingUp);
        Vector3 move = direction * speed * Time.deltaTime;
        transform.Translate(move);

        segmentProgress += move.magnitude;

        if (segmentProgress >= zigzagSegmentLength)
        {
            ClampToSegment(direction);
            zigZagPath.Add(direction);

            segmentProgress = 0f;
            goingUp = !goingUp;

            if (Vector3.Distance(transform.position, startPosition) >= travelDistance)
            {
                movingForward = false;
                backwardIndex = zigZagPath.Count - 1;
            }
        }
    }

    void MoveZigZagBackward()
    {
        if (backwardIndex < 0)
        {
            ResetZigZag();
            return;
        }

        Vector3 direction = -zigZagPath[backwardIndex];
        Vector3 move = direction * speed * Time.deltaTime;
        transform.Translate(move);

        segmentProgress += move.magnitude;

        if (segmentProgress >= zigzagSegmentLength)
        {
            ClampToSegment(direction);
            segmentProgress = 0f;
            backwardIndex--;
        }
    }

    Vector3 CalculateZigZagDirection(bool up)
    {
        float angle = up ? zigzagAngle : -zigzagAngle;
        float radians = angle * Mathf.Deg2Rad;

        return zigzagMoveVertical
            ? new Vector3(Mathf.Sin(radians), Mathf.Cos(radians)).normalized
            : new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    void ClampToSegment(Vector3 direction)
    {
        float overflow = segmentProgress - zigzagSegmentLength;
        if (overflow > 0f)
            transform.Translate(-direction * overflow);
    }

    void ResetZigZag()
    {
        movingForward = true;
        goingUp = true;
        segmentProgress = 0f;
        zigZagPath.Clear();
    }

    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerDissolve != null)
        {
            if (rocketHitSound != null && audioSource != null)
                audioSource.PlayOneShot(rocketHitSound);

            playerDissolve.TriggerRespawn(respawncontroller.instance.respawnpoint.position);
        }
    }





    //public enum MoveDirection { Up, Down }
    //public MoveDirection moveDirection = MoveDirection.Down;

    //[Header("Movement Settings")]
    //public float speed = 5f;
    //public float travelDistance = 6f;

    //[Header("Respawn Settings")]
    //private Vector3 startPosition;

    //[Header("Player Detection")]
    //private dissolve playerDissolve;

    //void Start()
    //{
    //    startPosition = transform.position;

    //    // Get reference to player dissolve script
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    if (player != null)
    //    {
    //        playerDissolve = player.GetComponent<dissolve>();
    //    }
    //}

    //void Update()
    //{
    //    MoveRocket();
    //    CheckReset();
    //}

    //void MoveRocket()
    //{
    //    Vector3 direction = (moveDirection == MoveDirection.Up) ? Vector3.up : Vector3.down;
    //    transform.Translate(direction * speed * Time.deltaTime);
    //}

    //void CheckReset()
    //{
    //    float traveled = Vector3.Distance(transform.position, startPosition);
    //    if (traveled >= travelDistance)
    //    {
    //        transform.position = startPosition;
    //    }
    //}

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player") && playerDissolve != null)
    //    {
    //        Debug.Log("🚀 Rocket hit player");
    //        playerDissolve.TriggerRespawn(respawncontroller.instance.respawnpoint.position);
    //    }
    //}
}
