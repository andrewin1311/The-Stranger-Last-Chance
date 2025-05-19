using TMPro;
using UnityEngine;
public class GraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown; 

    void Start()
    {
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.RefreshShownValue();
        graphicsDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log("Graphics quality set to: " + index);
    }
}
