using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Panel references")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private UIEvents _events;
    [SerializeField] private GameUIData _data;
    [SerializeField] private GameObject chestPanel;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard is null)
        {
            return;
        }

        if (keyboard.escapeKey.wasPressedThisFrame || keyboard.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void OnEnable()
    {
        _events.OnInventoryOpen += OnInventoryOpen;
        _events.OnInventoryClose += OnInventoryClose;
    }

    private void OnDisable()
    {
        _events.OnInventoryOpen -= OnInventoryOpen;
        _events.OnInventoryClose -= OnInventoryClose;
    }

    public void TogglePause()
    {
        if (_data.isChestOpen) return;

        if (isPaused)
        {
            ResumeGame();
            return;
        }

        PauseGame();
    }

    public void PauseGame()
    {
        _events.InvokePause();
        isPaused = true;
        gamePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _events.InvokeUnPause();
        Time.timeScale = 1f;
        isPaused = false;
        gamePanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (_data.isChestOpen) return;
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        pauseMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnInventoryOpen()
    {
        if (_data.isChestOpen) return;
        inventoryPanel.SetActive(true);
        _inventoryEvents.InvokeOpen();
        gamePanel.SetActive(false);
    }

    public void OnInventoryClose()
    {
        inventoryPanel.SetActive(false);
        _inventoryEvents.InvokeClose();
        gamePanel.SetActive(true);
    }

    public void OnChestClose()
    {
        chestPanel.SetActive(false);
        _events.InvokeChestClose();
    }
}