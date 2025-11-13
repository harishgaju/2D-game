using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "SampleScene";
    public float fadeDuration = 2f;

    [Header("UI References")]
    public Image fadeOverlay;            
    public Image tutorialImage;           
    public TextMeshProUGUI pressSpaceTMP; 

    private bool isTransitioning = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        
        if (fadeOverlay) fadeOverlay.color = new Color(0, 0, 0, 1);
        if (tutorialImage) tutorialImage.color = new Color(1, 1, 1, 0);
        if (pressSpaceTMP) pressSpaceTMP.color = new Color(1, 1, 1, 0);

        StartCoroutine(SceneIntro());
    }

    void Update()
    {
        if (!isTransitioning && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ExitTutorial());
        }
    }

    IEnumerator SceneIntro()
    {
        
        if (fadeOverlay) yield return StartCoroutine(FadeImage(fadeOverlay, 1f, 0f));

       
        if (tutorialImage) yield return StartCoroutine(FadeImage(tutorialImage, 0f, 1f));

        if (pressSpaceTMP)
        {
            yield return StartCoroutine(FadeTMPText(pressSpaceTMP, 0f, 1f));
            blinkCoroutine = StartCoroutine(BlinkTMPTextLoop());
        }
    }

    IEnumerator ExitTutorial()
    {
        isTransitioning = true;

        // Stop blinking
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        // Fade out text
        if (pressSpaceTMP) yield return StartCoroutine(FadeTMPText(pressSpaceTMP, pressSpaceTMP.color.a, 0f));

        // Fade out image
        if (tutorialImage) yield return StartCoroutine(FadeImage(tutorialImage, 1f, 0f));

        // Fade in black overlay
        if (fadeOverlay) yield return StartCoroutine(FadeImage(fadeOverlay, 0f, 1f));

        // Load next scene
        SceneManager.LoadSceneAsync(nextSceneName);
    }

    IEnumerator FadeImage(Image image, float from, float to)
    {
        float time = 0f;
        Color color = image.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.SmoothStep(from, to, time / fadeDuration);
            color.a = alpha;
            image.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        color.a = to;
        image.color = color;
    }

    IEnumerator FadeTMPText(TextMeshProUGUI tmpText, float from, float to)
    {
        float time = 0f;
        Color color = tmpText.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.SmoothStep(from, to, time / fadeDuration);
            color.a = alpha;
            tmpText.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        color.a = to;
        tmpText.color = color;
    }

    IEnumerator BlinkTMPTextLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FadeTMPText(pressSpaceTMP, 1f, 0f));
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(FadeTMPText(pressSpaceTMP, 0f, 1f));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
