using UnityEngine;
using UnityEngine.SceneManagement; // Needed to switch scenes

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Player took damage! Current health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("💀 Player died! Switching to GameOver scene...");
        SceneManager.LoadScene("GameOverScene"); // ✅ This line triggers the scene change
    }
}
