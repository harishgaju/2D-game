using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform centerPoint; // Assign the orbit center object in inspector
    public float orbitRadius = 3f;
    public float orbitSpeed = 1f; // radians per second

    [Header("Line Arrange Settings")]
    public float arrangeDistance = 5f; // Player proximity to trigger line arrange
    public float lineSpacing = 2f; // Distance between platforms in line
    public Vector3 lineStartPosition = Vector3.zero; // relative to centerPoint

    [Header("Fade Settings")]
    public float blinkIntervalMin = 1f;    // Minimum random blink interval
    public float blinkIntervalMax = 3f;    // Maximum random blink interval
    public float fadeDuration = 0.5f;

    private SpriteRenderer sr;
    private Collider2D col;
    private ShadowCaster2D shadow;
    private Material shadowMat;

    private static readonly int AlphaCutoff = Shader.PropertyToID("_AlphaCutoff");

    private bool isPlayerOn = false;
    private bool isFading = false;

    private static List<DisappearingPlatform> allPlatforms = new List<DisappearingPlatform>();
    private int myIndex = 0;

    private float currentAngleOffset = 0f; // common rotation angle shared by group

    private Transform player;
    private bool inLineMode = false;

    private Coroutine moveCoroutine = null;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        shadow = GetComponent<ShadowCaster2D>();

        if (!sr || !col || !shadow || !centerPoint)
        {
            Debug.LogError("Missing required component(s) or centerPoint.");
            enabled = false;
            return;
        }

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            shadowMat = renderer.sharedMaterial;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void OnEnable()
    {
        allPlatforms.Add(this);
        UpdateIndices();

        // Start blinking coroutine once and keep it running
        StartCoroutine(BlinkPlatform());
    }

    private void OnDisable()
    {
        allPlatforms.Remove(this);
        UpdateIndices();
    }

    private void UpdateIndices()
    {
        for (int i = 0; i < allPlatforms.Count; i++)
        {
            allPlatforms[i].myIndex = i;
        }
    }

    private void Update()
    {
        if (player == null || centerPoint == null || allPlatforms.Count == 0)
            return;

        float distToPlayer = Vector3.Distance(player.position, centerPoint.position);
        bool shouldLineArrange = distToPlayer <= arrangeDistance;

        if (shouldLineArrange && !inLineMode)
        {
            // Switch to line arrange mode
            StartMovementCoroutine(MoveToLinePositions());
            inLineMode = true;
        }
        else if (!shouldLineArrange && inLineMode)
        {
            // Switch back to orbit mode
            StartMovementCoroutine(MoveToOrbitPositions());
            inLineMode = false;
        }

        if (!inLineMode)
        {
            currentAngleOffset += orbitSpeed * Time.deltaTime;
            if (currentAngleOffset > Mathf.PI * 2f)
                currentAngleOffset -= Mathf.PI * 2f;

            float angleBetween = 2 * Mathf.PI / allPlatforms.Count;
            float angle = currentAngleOffset + angleBetween * myIndex;
            Vector3 orbitPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;
            transform.position = centerPoint.position + orbitPos;
        }
    }

    private void StartMovementCoroutine(IEnumerator coroutine)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(coroutine);
    }

    private IEnumerator MoveToLinePositions()
    {
        float duration = 1f;
        float timer = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = centerPoint.position + lineStartPosition + Vector3.right * lineSpacing * myIndex;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator MoveToOrbitPositions()
    {
        float duration = 1f;
        float timer = 0f;

        Vector3 startPos = transform.position;

        float angleBetween = 2 * Mathf.PI / allPlatforms.Count;
        float angle = currentAngleOffset + angleBetween * myIndex;
        Vector3 targetPos = centerPoint.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            yield return null;
        }

        transform.position = targetPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOn = true;
            // Stop blinking coroutine only (don't stop movement coroutines)
            StopCoroutine(BlinkPlatform());
            StartCoroutine(FadePlatform(true));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOn = false;
            StartCoroutine(BlinkPlatform());
        }
    }

    private IEnumerator BlinkPlatform()
    {
        while (!isPlayerOn)
        {
            yield return StartCoroutine(FadePlatform(false));
            yield return new WaitForSeconds(Random.Range(blinkIntervalMin, blinkIntervalMax));
            yield return StartCoroutine(FadePlatform(true));
            yield return new WaitForSeconds(Random.Range(blinkIntervalMin, blinkIntervalMax));
        }

        // Ensure fully visible if player is on platform
        yield return StartCoroutine(FadePlatform(true));
    }

    private IEnumerator FadePlatform(bool fadeIn)
    {
        isFading = true;
        float timer = 0f;
        float startAlpha = sr.color.a;
        float endAlpha = fadeIn ? 1f : 0f;

        if (fadeIn)
        {
            sr.enabled = true;
            col.enabled = true;
            shadow.enabled = true;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            Color color = sr.color;
            color.a = alpha;
            sr.color = color;

            if (shadowMat != null)
            {
                shadowMat.SetFloat(AlphaCutoff, 1f - alpha);
            }

            yield return null;
        }

        Color finalColor = sr.color;
        finalColor.a = endAlpha;
        sr.color = finalColor;

        if (shadowMat != null)
        {
            shadowMat.SetFloat(AlphaCutoff, 1f - endAlpha);
        }

        col.enabled = fadeIn;
        sr.enabled = fadeIn;
        shadow.enabled = fadeIn;

        isFading = false;
    }



    //[Header("Orbit Settings")]
    //public Transform centerPoint; // Assign the orbit center object in inspector
    //public float orbitRadius = 3f;
    //public float orbitSpeed = 1f; // radians per second

    //[Header("Line Arrange Settings")]
    //public float arrangeDistance = 5f; // Player proximity to trigger line arrange
    //public float lineSpacing = 2f; // Distance between platforms in line
    //public Vector3 lineStartPosition = Vector3.zero; // relative to centerPoint

    //[Header("Fade Settings")]
    //public float blinkInterval = 2f;
    //public float fadeDuration = 0.5f;

    //private SpriteRenderer sr;
    //private Collider2D col;
    //private ShadowCaster2D shadow;
    //private Material shadowMat;

    //private static readonly int AlphaCutoff = Shader.PropertyToID("_AlphaCutoff");

    //private bool isPlayerOn = false;
    //private bool isFading = false;

    //private static List<DisappearingPlatform> allPlatforms = new List<DisappearingPlatform>();
    //private int myIndex = 0;

    //private float currentAngleOffset = 0f; // common rotation angle shared by group

    //private Transform player;
    //private bool inLineMode = false;

    //private Coroutine moveCoroutine = null;

    //private void Awake()
    //{
    //    sr = GetComponent<SpriteRenderer>();
    //    col = GetComponent<Collider2D>();
    //    shadow = GetComponent<ShadowCaster2D>();

    //    if (!sr || !col || !shadow || !centerPoint)
    //    {
    //        Debug.LogError("Missing required component(s) or centerPoint.");
    //        enabled = false;
    //        return;
    //    }

    //    var renderer = GetComponent<Renderer>();
    //    if (renderer != null)
    //        shadowMat = renderer.sharedMaterial;

    //    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //    if (playerObj != null)
    //        player = playerObj.transform;
    //}

    //private void OnEnable()
    //{
    //    allPlatforms.Add(this);
    //    UpdateIndices();

    //    // Start blinking coroutine once and keep it running
    //    StartCoroutine(BlinkPlatform());
    //}

    //private void OnDisable()
    //{
    //    allPlatforms.Remove(this);
    //    UpdateIndices();
    //}

    //private void UpdateIndices()
    //{
    //    for (int i = 0; i < allPlatforms.Count; i++)
    //    {
    //        allPlatforms[i].myIndex = i;
    //    }
    //}

    //private void Update()
    //{
    //    if (player == null || centerPoint == null || allPlatforms.Count == 0)
    //        return;

    //    float distToPlayer = Vector3.Distance(player.position, centerPoint.position);
    //    bool shouldLineArrange = distToPlayer <= arrangeDistance;

    //    if (shouldLineArrange && !inLineMode)
    //    {
    //        // Switch to line arrange mode
    //        StartMovementCoroutine(MoveToLinePositions());
    //        inLineMode = true;
    //    }
    //    else if (!shouldLineArrange && inLineMode)
    //    {
    //        // Switch back to orbit mode
    //        StartMovementCoroutine(MoveToOrbitPositions());
    //        inLineMode = false;
    //    }

    //    if (!inLineMode)
    //    {
    //        currentAngleOffset += orbitSpeed * Time.deltaTime;
    //        if (currentAngleOffset > Mathf.PI * 2f)
    //            currentAngleOffset -= Mathf.PI * 2f;

    //        float angleBetween = 2 * Mathf.PI / allPlatforms.Count;
    //        float angle = currentAngleOffset + angleBetween * myIndex;
    //        Vector3 orbitPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;
    //        transform.position = centerPoint.position + orbitPos;
    //    }
    //}

    //private void StartMovementCoroutine(IEnumerator coroutine)
    //{
    //    if (moveCoroutine != null)
    //        StopCoroutine(moveCoroutine);
    //    moveCoroutine = StartCoroutine(coroutine);
    //}

    //private IEnumerator MoveToLinePositions()
    //{
    //    float duration = 1f;
    //    float timer = 0f;

    //    Vector3 startPos = transform.position;
    //    Vector3 targetPos = centerPoint.position + lineStartPosition + Vector3.right * lineSpacing * myIndex;

    //    while (timer < duration)
    //    {
    //        timer += Time.deltaTime;
    //        transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
    //        yield return null;
    //    }

    //    transform.position = targetPos;
    //}

    //private IEnumerator MoveToOrbitPositions()
    //{
    //    float duration = 1f;
    //    float timer = 0f;

    //    Vector3 startPos = transform.position;

    //    float angleBetween = 2 * Mathf.PI / allPlatforms.Count;
    //    float angle = currentAngleOffset + angleBetween * myIndex;
    //    Vector3 targetPos = centerPoint.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;

    //    while (timer < duration)
    //    {
    //        timer += Time.deltaTime;
    //        transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
    //        yield return null;
    //    }

    //    transform.position = targetPos;
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        isPlayerOn = true;
    //        // Stop blinking coroutine only (don't stop movement coroutines)
    //        StopCoroutine(BlinkPlatform());
    //        StartCoroutine(FadePlatform(true));
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        isPlayerOn = false;
    //        StartCoroutine(BlinkPlatform());
    //    }
    //}

    //private IEnumerator BlinkPlatform()
    //{
    //    while (!isPlayerOn)
    //    {
    //        yield return StartCoroutine(FadePlatform(false));
    //        yield return new WaitForSeconds(blinkInterval);
    //        yield return StartCoroutine(FadePlatform(true));
    //        yield return new WaitForSeconds(blinkInterval);
    //    }

    //    // Ensure fully visible if player is on platform
    //    yield return StartCoroutine(FadePlatform(true));
    //}

    //private IEnumerator FadePlatform(bool fadeIn)
    //{
    //    isFading = true;
    //    float timer = 0f;
    //    float startAlpha = sr.color.a;
    //    float endAlpha = fadeIn ? 1f : 0f;

    //    if (fadeIn)
    //    {
    //        sr.enabled = true;
    //        col.enabled = true;
    //        shadow.enabled = true;
    //    }

    //    while (timer < fadeDuration)
    //    {
    //        timer += Time.deltaTime;
    //        float t = Mathf.Clamp01(timer / fadeDuration);
    //        float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

    //        Color color = sr.color;
    //        color.a = alpha;
    //        sr.color = color;

    //        if (shadowMat != null)
    //        {
    //            shadowMat.SetFloat(AlphaCutoff, 1f - alpha);
    //        }

    //        yield return null;
    //    }

    //    Color finalColor = sr.color;
    //    finalColor.a = endAlpha;
    //    sr.color = finalColor;

    //    if (shadowMat != null)
    //    {
    //        shadowMat.SetFloat(AlphaCutoff, 1f - endAlpha);
    //    }

    //    col.enabled = fadeIn;
    //    sr.enabled = fadeIn;
    //    shadow.enabled = fadeIn;

    //    isFading = false;
    //}
}
