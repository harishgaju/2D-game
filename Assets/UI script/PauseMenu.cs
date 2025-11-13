using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    public AudioMixer audioMixer;

    public AudioSource backgroundMusicSource; 

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        SetVolume(volumeSlider.value);

        if (backgroundMusicSource != null)
            backgroundMusicSource.ignoreListenerPause = true; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;  
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; 
        isPaused = false;
    }
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MasterVolume", dB);
        UpdateVolumeText(volume);
    }

    void UpdateVolumeText(float volume)
    {
        int percent = Mathf.RoundToInt(volume * 100);
        volumeValueText.text = $"Volume: {percent}%";
    }
}
