using System.Collections;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject defaultPanel;
    public GameObject graphicsPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;
    public GameObject gameplayPanel;
    public GameObject otherPanel;

    [Header("Graphics")]
    public GameObject dayImage;
    public GameObject nightImage;
    public Material daySkybox;
    public Material nightSkybox;
    public GameObject Light;

    [Header("Cameras")]
    public GameObject firstCamera;
    public GameObject secondCamera;

    [Header("Canvas")]
    public GameObject firstCanvas;
    public GameObject secondCanvas;

    [Header("Camera Transition")]
    public float transitionDuration = 0.5f;

    private void Start()
    {
        SetDay();

        firstCamera.SetActive(true);
        secondCamera.SetActive(false);

        firstCanvas.SetActive(true);
        secondCanvas.SetActive(false);

        CloseAllPanels();
        defaultPanel.SetActive(true);
    }

    public void OpenGraphics()
    {
        CloseAllPanels();
        graphicsPanel.SetActive(true);
    }

    public void OpenAudio()
    {
        CloseAllPanels();
        audioPanel.SetActive(true);
    }

    public void OpenControls()
    {
        CloseAllPanels();
        controlsPanel.SetActive(true);
    }

    public void OpenGameplay()
    {
        CloseAllPanels();
        gameplayPanel.SetActive(true);
    }

    public void OpenOther()
    {
        CloseAllPanels();
        otherPanel.SetActive(true);
    }

    public void Back()
    {
        CloseAllPanels();
        defaultPanel.SetActive(true);

        StartCoroutine(SwitchToFirstCamera());
    }

    public void SetDay()
    {
        dayImage.SetActive(true);
        nightImage.SetActive(false);

        RenderSettings.skybox = daySkybox;
        DynamicGI.UpdateEnvironment();

        Light.SetActive(true);
    }

    public void SetNight()
    {
        dayImage.SetActive(false);
        nightImage.SetActive(true);

        RenderSettings.skybox = nightSkybox;
        DynamicGI.UpdateEnvironment();

        Light.SetActive(false);
    }

    public void OpenSettings()
    {
        CloseAllPanels();

        secondCamera.SetActive(true);
        secondCanvas.SetActive(true);

        firstCamera.SetActive(false);
        firstCanvas.SetActive(false);

        defaultPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        defaultPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        otherPanel.SetActive(false);
    }

    private IEnumerator SwitchToFirstCamera()
    {
        Vector3 startPosition = secondCamera.transform.position;
        Quaternion startRotation = secondCamera.transform.rotation;

        Vector3 targetPosition = firstCamera.transform.position;
        Quaternion targetRotation = firstCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            secondCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            secondCamera.transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        secondCamera.transform.position = targetPosition;
        secondCamera.transform.rotation = targetRotation;

        secondCamera.SetActive(false);
        secondCanvas.SetActive(false);

        firstCamera.SetActive(true);
        firstCanvas.SetActive(true);
    }
}