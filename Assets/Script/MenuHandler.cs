using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;
    public GameObject audioPanel;
    public GameObject mainButtonsPanel;
    public GameObject graphicsPanel;
    public GameObject controlsPanel;

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
        graphicsPanel.SetActive(false);
        controlsPanel.SetActive(false);
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
        mainButtonsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControlsSettings()
    {
        mainButtonsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackToOptionsFromGraphics()
    {
        graphicsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToOptionsFromControls()
    {
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(true);
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
