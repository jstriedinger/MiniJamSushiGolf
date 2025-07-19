using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnStartGame()
    {
        SceneManager.LoadScene("TestScene");
        // This will be expanded in later steps
    }

    public void OnRules()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void OnCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OnQuit()
    {
        Application.Quit();
        Debug.Log("Quit Game clicked.");
    }
}