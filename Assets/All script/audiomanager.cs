using System.Collections;
using UnityEngine;

public class audiomanager : MonoBehaviour
{
    public static audiomanager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    public AudioClip background;
    public AudioClip checkpoint;
    public AudioClip portalA;
    public AudioClip portaloB;
    public void FadeToMusic(AudioClip newClip, float fadeDuration) { /*...*/ }
    public void FadeOutMusic(float duration) { /*...*/ }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void playSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    //public void FadeToMusic(AudioClip newClip, float fadeDuration)
    //{
    //    StartCoroutine(FadeMusicRoutine(newClip, fadeDuration));
    //}

    private IEnumerator FadeMusicRoutine(AudioClip newClip, float duration)
    {
        if (musicSource.clip == newClip) yield break;

        float time = 0f;
        float startVolume = musicSource.volume;

        // Fade out
        while (time < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        time = 0f;
        while (time < duration)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = startVolume;
    }



    ////audiosource
    //[SerializeField] AudioSource musicSource;
    //[SerializeField] AudioSource SFXSource;

    ////auidoclip
    //public AudioClip background;
    ////public AudioClip Playerstepsound;
    //public AudioClip checkpoint;
    //public AudioClip portalA;
    //public AudioClip portaloB;
    //private void Start()
    //{
    //    musicSource.clip = background;
    //    musicSource.Play();
    //}
    //public void playSFX(AudioClip clip)
    //{
    //    SFXSource.PlayOneShot(clip);
    //}
    //public void PlayMusic(AudioClip clip)
    //{
    //    if (clip == null) return;

    //    if (musicSource.clip != clip)
    //    {
    //        musicSource.clip = clip;
    //        musicSource.Play();
    //    }
    //}

    //public void StopMusic()
    //{
    //    musicSource.Stop();
    //}
}
