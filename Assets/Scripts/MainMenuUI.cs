using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public AudioClip buttonClickSFX;
    public float sceneLoadDelay = 1.0f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSFX != null)
        {
            audioSource.PlayOneShot(buttonClickSFX);
        }
    }

    public void LoadMainMenu() => StartCoroutine(LoadSceneWithSFX("MainMenu"));
    public void OnPlayerSelect() => StartCoroutine(LoadSceneWithSFX("PlayerSelect"));
    public void OnStartGame() => StartCoroutine(LoadSceneWithSFX("TestScene"));
    public void OnRules() => StartCoroutine(LoadSceneWithSFX("HowToPlay"));
    public void OnCredits() => StartCoroutine(LoadSceneWithSFX("Credits"));
    public void OnQuit()
    {
        PlayClickSound();
        Debug.Log("Quit Game clicked.");
        Application.Quit();
    }

    private System.Collections.IEnumerator LoadSceneWithSFX(string sceneName)
    {
        PlayClickSound();
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(sceneName);
    }
}
