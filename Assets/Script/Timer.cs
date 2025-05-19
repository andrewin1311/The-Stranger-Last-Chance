// I used https://www.youtube.com/watch?v=POq1i8FyRyQ to figure out how to make a timer in Unity.

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] float remainingTime;

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            time.text = "00:00";
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
