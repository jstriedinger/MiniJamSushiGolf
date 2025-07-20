using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Range(0f, 1f)]
    public float musicVolume = 0.7f; // 👈 Exposed in Inspector

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume; // 👈 Use editable volume
        audioSource.clip = menuMusic;
        audioSource.Play();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene") // Replace with your gameplay scene name
        {
            StartCoroutine(CrossfadeTo(gameplayMusic));
        }
        else
        {
            StartCoroutine(CrossfadeTo(menuMusic));
        }
    }

    private IEnumerator CrossfadeTo(AudioClip newClip)
    {
        float fadeDuration = 1f;
        float startVolume = musicVolume;

        // Fade out
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = musicVolume;
    }
}
