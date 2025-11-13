using UnityEngine;
using UnityEngine.Rendering.Universal;

public class flickeringlightscript : MonoBehaviour
{
    public Light2D spotlight;
    //flash
    public float minFlashIntensity = 0.8f;
    public float maxFlashIntensity = 1.5f;
    public int minFlashes = 1;
    public int maxFlashes = 3;
    public float flashDuration = 0.05f;
    public float flashInterval = 0.1f;
    //Lightning Strikes
    public float minStrikeDelay = 2.0f;
    public float maxStrikeDelay = 5.0f;

    private bool isFlashing = false;
    void Start()
    {
        if (spotlight == null)
        {
            spotlight = GetComponent<Light2D>();
        }
        spotlight.intensity = 0f;
        StartCoroutine(LightningLoop());
    }
    System.Collections.IEnumerator LightningLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minStrikeDelay, maxStrikeDelay);
            yield return new WaitForSeconds(waitTime);

            int flashCount = Random.Range(minFlashes, maxFlashes + 1);
            yield return StartCoroutine(FlashLightning(flashCount));
        }
    }
    System.Collections.IEnumerator FlashLightning(int flashes)
    {
        isFlashing = true;

        for (int i = 0; i < flashes; i++)
        {
            float intensity = Random.Range(minFlashIntensity, maxFlashIntensity);
            spotlight.intensity = intensity;

            yield return new WaitForSeconds(flashDuration);

            spotlight.intensity = 0f;

            yield return new WaitForSeconds(flashInterval);
        }

        isFlashing = false;
    }
}
