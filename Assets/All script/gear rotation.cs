using UnityEngine;

public class gearrotation : MonoBehaviour
{
    [Header("Gear Movement")]
    public float rotationspeed;
    public float upspeed;
    public float downspeed;
    public Transform up;
    public Transform down;
    bool chop;

    [Header("Audio Settings")]
    public AudioClip soundClip;
    public Collider2D soundTriggerZone;  // Assign your child trigger collider here (Is Trigger = true)

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private float targetVolume = 1f;
    private float currentFadeTime = 0f;

    private bool playerInside = false;

    // Reference to player dissolve script
    private dissolve playerDissolve;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = soundClip;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.spatialBlend = 0f;  // 2D sound

        if (soundTriggerZone == null)
            Debug.LogWarning("Assign a child trigger collider to soundTriggerZone.");

        // Find player dissolve component once at start
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerDissolve = player.GetComponent<dissolve>();
    }

    private void Update()
    {
        // Rotate gear
        transform.Rotate(0, 0, rotationspeed * Time.deltaTime);

        // Move gear up/down
        float distanceToUp = Vector2.Distance(transform.position, up.position);
        float distanceToDown = Vector2.Distance(transform.position, down.position);
        float epsilon = 0.01f;

        if (distanceToUp < epsilon) chop = true;
        if (distanceToDown < epsilon) chop = false;

        if (chop)
            transform.position = Vector2.MoveTowards(transform.position, down.position, downspeed * Time.deltaTime);
        else
            transform.position = Vector2.MoveTowards(transform.position, up.position, upspeed * Time.deltaTime);

        // Handle audio fade in/out
        if (isFadingIn)
        {
            currentFadeTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, currentFadeTime / fadeDuration);
            if (currentFadeTime >= fadeDuration)
            {
                audioSource.volume = targetVolume;
                isFadingIn = false;
            }
        }
        else if (isFadingOut)
        {
            currentFadeTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(targetVolume, 0f, currentFadeTime / fadeDuration);
            if (currentFadeTime >= fadeDuration)
            {
                audioSource.volume = 0f;
                isFadingOut = false;
                audioSource.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        if (soundTriggerZone == null) return;

        Collider2D playerCollider = GetPlayerCollider();
        if (playerCollider == null) return;

        bool isOverlapping = soundTriggerZone.IsTouching(playerCollider);

        if (isOverlapping && !playerInside)
        {
            playerInside = true;
            StartFadeIn();
        }
        else if (!isOverlapping && playerInside)
        {
            playerInside = false;
            StartFadeOut();
        }
    }

    private Collider2D GetPlayerCollider()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return null;
        return player.GetComponent<Collider2D>();
    }

    private void StartFadeIn()
    {
        if (!audioSource.isPlaying) audioSource.Play();
        isFadingIn = true;
        isFadingOut = false;
        currentFadeTime = 0f;
    }

    private void StartFadeOut()
    {
        isFadingOut = true;
        isFadingIn = false;
        currentFadeTime = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (playerDissolve != null && respawncontroller.instance != null && respawncontroller.instance.respawnpoint != null)
            {
                Vector3 checkpointPos = respawncontroller.instance.respawnpoint.position;
                playerDissolve.TriggerRespawn(checkpointPos);
            }
        }
    }

    //public float rotationspeed;
    //public float upspeed;
    //public float downspeed;
    //public Transform up;
    //public Transform down;
    //bool chop;

    //private void Update()
    //{
    //    transform.Rotate(new Vector3(0, 0, rotationspeed));

    //    float distanceToUp = Vector2.Distance(transform.position, up.position);
    //    float distanceToDown = Vector2.Distance(transform.position, down.position);
    //    float epsilon = 0.01f;

    //    if (distanceToUp < epsilon)
    //    {
    //        chop = true;
    //    }
    //    if (distanceToDown < epsilon)
    //    {
    //        chop = false;
    //    }
    //    if (chop)
    //    {
    //        transform.position = Vector2.MoveTowards(transform.position, down.position, downspeed * Time.deltaTime);
    //    }
    //    else
    //    {
    //        transform.position = Vector2.MoveTowards(transform.position, up.position, upspeed * Time.deltaTime);
    //    }
    //}

}


