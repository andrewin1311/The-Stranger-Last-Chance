#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;
    public GameObject audioPanel;
    public GameObject mainButtonsPanel;
    public GameObject graphicsPanel;
    public GameObject controlsPanel;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;

    void Start()
    {
    resolutions = Screen.resolutions;
    resolutionDropdown.ClearOptions();

    var options = new System.Collections.Generic.List<string>();
    int currentResolutionIndex = 0;

    for (int i = 0; i < resolutions.Length; i++)
    {
        string option = resolutions[i].width + " x " + resolutions[i].height;
        options.Add(option);

        if (resolutions[i].width == Screen.currentResolution.width &&
            resolutions[i].height == Screen.currentResolution.height)
        {
            currentResolutionIndex = i;
        }
    }

    resolutionDropdown.AddOptions(options);
    resolutionDropdown.value = currentResolutionIndex;
    resolutionDropdown.RefreshShownValue();

    fullscreenToggle.isOn = Screen.fullScreen;
    }
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
        graphicsPanel.SetActive(true);
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        mainButtonsPanel.SetActive(false);
    }    public void OpenControlsSettings()
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
    public void ApplyGraphicsSettings()
    {
    int resolutionIndex = resolutionDropdown.value;
    Resolution selectedResolution = resolutions[resolutionIndex];
    Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenToggle.isOn);
    Debug.Log("Applied Graphics: " + selectedResolution.width + "x" + selectedResolution.height + " | Fullscreen: " + fullscreenToggle.isOn);
    }


    public void CloseOptions()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        Debug.Log("Options menu closed");
    }

    public void QuitGame()
    {
        
        Debug.Log("Quit Game");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
