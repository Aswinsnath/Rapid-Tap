using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pausePanel; // Panel containing the pause menu UI

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause panel is hidden at the start
        pausePanel.SetActive(false);

        // Make sure the cursor is always visible
        Cursor.visible = true;
    }

    // Function to pause the game
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); // Show the pause menu
        Time.timeScale = 0f;        // Freeze the game mechanics
        AudioListener.pause = true; // Pause all game audio

        // Cursor remains visible and unlocked during pause
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for menu interaction
    }

    // Function to resume the game
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;         // Unfreeze the game mechanics
        AudioListener.pause = false; // Resume all game audio

        // Cursor remains visible and unlocked during gameplay
        Cursor.lockState = CursorLockMode.None; // Keep the cursor unlocked
    }

    // Function to restart the current level
    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    // Function to load the main menu
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading the main menu
        SceneManager.LoadScene("Main Menu"); // Replace with the name of your main menu scene
    }
}
