using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;
    public GameObject audioPanel;
    public GameObject mainButtonsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        Debug.Log("Options menu opened");
    }
    
    
    public void OpenAudioSettings()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(true);
        Debug.Log("Audio settings opened");
    }

    public void BackToOptions()
    {
        audioPanel.SetActive(false);
        optionsPanel.SetActive(true);
        mainButtonsPanel.SetActive(false);
        Debug.Log("Returned to Options");
    }
    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
        Debug.Log("Back to main menu");
    }

    public void OpenGraphicsSettings()
    {
        Debug.Log("Graphics settings clicked");
    }

    public void OpenControlsSettings()
    {
        Debug.Log("Controls settings clicked");
    }


    public void CloseOptions()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        Debug.Log("Options menu closed");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
