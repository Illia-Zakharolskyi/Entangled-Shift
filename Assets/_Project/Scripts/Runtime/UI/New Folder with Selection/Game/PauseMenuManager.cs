using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Панели интерфейса")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;

    /*[Header("Настройки сцен")]
    [SerializeField] private string gameTypeSceneName = "GTS";*/

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        
        if (keyboard.escapeKey.wasPressedThisFrame || keyboard.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {

        isPaused = true;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        if (settingsMenuPanel) settingsMenuPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel) settingsMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel) settingsMenuPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsMenuPanel) settingsMenuPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
    }

    /*public void GoToGameTypeSelection()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameTypeSceneName);
    }*/

    public void QuitGame()
    {
        Application.Quit();
    }
}