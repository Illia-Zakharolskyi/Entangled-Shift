
using System.Collections;
using UnityEngine;

public class PanelControll : MonoBehaviour
{
    [Header("Камера")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform settingsPoint;
    [SerializeField] private float transitionDuration = 0.8f;

    [Header("Панели")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject selectGame;
    [SerializeField] private GameObject devPanel;
    [SerializeField] private GameObject accoutPanel;

    private Coroutine transitionCoroutine;

    private void Start()
    {
        mainCamera.position = startPoint.position;
        mainCamera.rotation = startPoint.rotation;

        startPanel.SetActive(true);
        settingsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(true);

        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        MoveCameraToSettings();
    }

    public void MoveCameraToSettings()
    {
        MoveCameraTo(settingsPoint);
    }

    public void MoveCameraToStart()
    {
        Debug.Log("Камера: запускаю возврат");

        if (mainCamera == null || startPoint == null)
        {
            Debug.LogError("Не назначена камера или Start Point!");
            return;
        }

        Debug.Log("Камера сейчас: " + mainCamera.position);
        Debug.Log("Целевая позиция: " + startPoint.position);

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(MoveCamera(startPoint));
    }

    private void MoveCameraTo(Transform target)
    {
        if (mainCamera == null || target == null)
        {
            Debug.LogError("Не назначена камера или точка перемещения!");
            return;
        }

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(MoveCamera(target));
    }

    private IEnumerator MoveCamera(Transform target)
    {
        Vector3 startPosition = mainCamera.position;
        Quaternion startRotation = mainCamera.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                elapsedTime / transitionDuration
            );

            mainCamera.position = Vector3.Lerp(
                startPosition,
                target.position,
                t
            );

            mainCamera.rotation = Quaternion.Slerp(
                startRotation,
                target.rotation,
                t
            );

            yield return null;
        }

        mainCamera.position = target.position;
        mainCamera.rotation = target.rotation;

        transitionCoroutine = null;
    }

    public void OpenGraphics()
    {
        settingsPanel.SetActive(true);
        graphicsPanel.SetActive(true);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void OpenLanguage()
    {
        settingsPanel.SetActive(true);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void OpenAudio()
    {
        settingsPanel.SetActive(true);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        settingsPanel.SetActive(true);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }


    public void OpenSelectGame()
    {
        settingsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        audioPanel.SetActive(false);
        selectGame.SetActive(true);
        startPanel.SetActive(false);
    }

    public void CloseSelectGame()
    {
        selectGame.SetActive(false);
        settingsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void ExitToStart()
    {
        settingsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        languagePanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);

        startPanel.SetActive(true);

        MoveCameraToStart();
        Debug.Log("ExitToStart вызван");
    }
}