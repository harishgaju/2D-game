using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntroFader : MonoBehaviour
{
    public Image fadeImage;          
    public TMPro.TextMeshProUGUI levelText; 
    public float fadeDuration = 2f;
    public float delayBeforeText = 0.5f;
    public float textFadeDuration = 1.5f;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));
        yield return new WaitForSeconds(delayBeforeText);
        yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration));
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, to);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = levelText.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            levelText.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        levelText.color = new Color(color.r, color.g, color.b, to);
    }
}
