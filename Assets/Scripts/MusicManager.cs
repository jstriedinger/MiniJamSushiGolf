using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public float fadeDuration = 2f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource currentSource;
    private string currentSceneName = "";

    private void Awake()
    {
        // Singleton pattern
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Add two AudioSources
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true;
        sourceB.loop = true;

        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;

        currentSource = sourceA;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentSceneName)
        {
            currentSceneName = scene.name;
            PlayMusicForScene(currentSceneName);
        }
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip targetClip = (sceneName == "TestScene") ? gameplayMusic : menuMusic;
        if (currentSource.clip == targetClip) return;

        StartCoroutine(CrossfadeTo(targetClip));
    }

    private IEnumerator CrossfadeTo(AudioClip newClip)
    {
        AudioSource nextSource = (currentSource == sourceA) ? sourceB : sourceA;
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            currentSource.volume = Mathf.Lerp(1f, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 1f; // reset in case reused
        currentSource = nextSource;
    }
}
