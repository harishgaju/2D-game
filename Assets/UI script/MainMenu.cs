using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioSource backgroundMusic;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button playButton;
    public Button optionButton;
    public Button quitButton;

    [Header("Hover Backgrounds")]
    public GameObject newGameHoverBG;
    public GameObject continueHoverBG;
    public GameObject playHoverBG;
    public GameObject optionHoverBG;
    public GameObject quitHoverBG;

    private Vector3 originalScale = Vector3.one;
    private Vector3 zoomScale = new Vector3(1.1f, 1.1f, 1.1f);

    void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        // Hide all hover backgrounds at start
        HideAllHoverBGs();

        AddButtonListeners();
    }

    void AddButtonListeners()
    {
        AddButtonTriggers(newGameButton, newGameHoverBG);
        AddButtonTriggers(continueButton, continueHoverBG);
        AddButtonTriggers(playButton, playHoverBG);
        AddButtonTriggers(optionButton, optionHoverBG);
        AddButtonTriggers(quitButton, quitHoverBG);
    }

    void AddButtonTriggers(Button button, GameObject hoverBG)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Get Image & TMP components for color change
        Image buttonImage = button.GetComponent<Image>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        // Hover Enter
        EventTrigger.Entry entryHover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryHover.callback.AddListener((data) =>
        {
            PlayHoverSound();
            ZoomButton(button.transform, true);
            HideAllHoverBGs();       // Only one hover BG visible at a time
            if (hoverBG != null) hoverBG.SetActive(true);

            if (buttonImage != null) buttonImage.color = Color.black;
            if (buttonText != null) buttonText.color = Color.black;
        });
        trigger.triggers.Add(entryHover);

        // Hover Exit
        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) =>
        {
            ZoomButton(button.transform, false);
            if (hoverBG != null) hoverBG.SetActive(false);

            if (buttonImage != null) buttonImage.color = Color.white;
            if (buttonText != null) buttonText.color = Color.white;
        });
        trigger.triggers.Add(entryExit);

        // Click Sound
        button.onClick.AddListener(() => PlayClickSound());
    }

    void ZoomButton(Transform buttonTransform, bool zoomIn)
    {
        StopAllCoroutines();
        Vector3 targetScale = zoomIn ? zoomScale : originalScale;
        StartCoroutine(AnimateScale(buttonTransform, targetScale, 0.1f));
    }

    IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = target.localScale;
        float time = 0f;

        while (time < duration)
        {
            target.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        target.localScale = targetScale;
    }

    void HideAllHoverBGs()
    {
        if (newGameHoverBG != null) newGameHoverBG.SetActive(false);
        if (continueHoverBG != null) continueHoverBG.SetActive(false);
        if (playHoverBG != null) playHoverBG.SetActive(false);
        if (optionHoverBG != null) optionHoverBG.SetActive(false);
        if (quitHoverBG != null) quitHoverBG.SetActive(false);
    }

    public void PlayHoverSound()
    {
        if (audioSource != null && hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("PlayerControllerTutorial");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue game clicked!");
    }

    public void NewGame()
    {
        SceneManager.LoadSceneAsync("NewGameSceneName");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    //[Header("Audio")]
    //public AudioSource audioSource;
    //public AudioClip hoverClip;
    //public AudioClip clickClip;
    //public AudioSource backgroundMusic;

    //[Header("Buttons")]
    //public Button newGameButton;
    //public Button continueButton;
    //public Button playButton;      // You can keep or remove if you now have New Game
    //public Button optionButton;
    //public Button quitButton;

    //[Header("Hover Backgrounds")]
    //public GameObject newGameHoverBG;
    //public GameObject continueHoverBG;
    //public GameObject playHoverBG;
    //public GameObject optionHoverBG;
    //public GameObject quitHoverBG;

    //private Vector3 originalScale = Vector3.one;
    //private Vector3 zoomScale = new Vector3(1.1f, 1.1f, 1.1f);

    //void Start()
    //{
    //    if (backgroundMusic != null)
    //    {
    //        backgroundMusic.loop = true;
    //        backgroundMusic.Play();
    //    }

    //    // Hide all hover backgrounds at start
    //    HideAllHoverBGs();

    //    AddButtonListeners();
    //}

    //void AddButtonListeners()
    //{
    //    AddButtonTriggers(newGameButton, newGameHoverBG);
    //    AddButtonTriggers(continueButton, continueHoverBG);
    //    AddButtonTriggers(playButton, playHoverBG);
    //    AddButtonTriggers(optionButton, optionHoverBG);
    //    AddButtonTriggers(quitButton, quitHoverBG);
    //}

    //void AddButtonTriggers(Button button, GameObject hoverBG)
    //{
    //    if (button == null) return;

    //    EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
    //    if (trigger == null)
    //        trigger = button.gameObject.AddComponent<EventTrigger>();

    //    // Hover Enter
    //    EventTrigger.Entry entryHover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
    //    entryHover.callback.AddListener((data) =>
    //    {
    //        PlayHoverSound();
    //        ZoomButton(button.transform, true);
    //        HideAllHoverBGs();       // Only one hover BG visible at a time
    //        if (hoverBG != null) hoverBG.SetActive(true);
    //    });
    //    trigger.triggers.Add(entryHover);

    //    // Hover Exit
    //    EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
    //    entryExit.callback.AddListener((data) =>
    //    {
    //        ZoomButton(button.transform, false);
    //        if (hoverBG != null) hoverBG.SetActive(false);
    //    });
    //    trigger.triggers.Add(entryExit);

    //    // Click Sound
    //    button.onClick.AddListener(() => PlayClickSound());
    //}

    //void ZoomButton(Transform buttonTransform, bool zoomIn)
    //{
    //    StopAllCoroutines();
    //    Vector3 targetScale = zoomIn ? zoomScale : originalScale;
    //    StartCoroutine(AnimateScale(buttonTransform, targetScale, 0.1f));
    //}

    //IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    //{
    //    Vector3 initialScale = target.localScale;
    //    float time = 0f;

    //    while (time < duration)
    //    {
    //        target.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
    //        time += Time.unscaledDeltaTime;
    //        yield return null;
    //    }

    //    target.localScale = targetScale;
    //}

    //void HideAllHoverBGs()
    //{
    //    if (newGameHoverBG != null) newGameHoverBG.SetActive(false);
    //    if (continueHoverBG != null) continueHoverBG.SetActive(false);
    //    if (playHoverBG != null) playHoverBG.SetActive(false);
    //    if (optionHoverBG != null) optionHoverBG.SetActive(false);
    //    if (quitHoverBG != null) quitHoverBG.SetActive(false);
    //}

    //public void PlayHoverSound()
    //{
    //    if (audioSource != null && hoverClip != null)
    //    {
    //        audioSource.PlayOneShot(hoverClip);
    //    }
    //}

    //public void PlayClickSound()
    //{
    //    if (audioSource != null && clickClip != null)
    //    {
    //        audioSource.PlayOneShot(clickClip);
    //    }
    //}

    //public void PlayGame()
    //{
    //    SceneManager.LoadSceneAsync("PlayerControllerTutorial");
    //}

    //public void ContinueGame()
    //{
    //    // Load your saved game or continue scene
    //    Debug.Log("Continue game clicked!");
    //}

    //public void NewGame()
    //{
    //    // Start a new game scene
    //    SceneManager.LoadSceneAsync("NewGameSceneName");
    //}

    //public void QuitGame()
    //{
    //    Application.Quit();
    //}
}
