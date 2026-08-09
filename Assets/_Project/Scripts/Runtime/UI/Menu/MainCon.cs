using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCon : MonoBehaviour
{
    //[SerializeField] private GameObject nullPanel1;
    //[SerializeField] private GameObject nullPanel2;
    //[SerializeField] private GameObject nullPanel3;
    //[SerializeField] private GameObject nullPanel4;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject devPanel;
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject gtPanel;
    [SerializeField] private Animator fade;
    [SerializeField] private float fadeTime = 0.5f;

    private bool isBusy;

    public void Game(string sceneName)
    {
        DoFade(() => SceneManager.LoadScene(sceneName));
    }

    public void GTpanel()
    {
        DoFade(() =>
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (gtPanel) gtPanel.SetActive(true);

        });
    }

    public void CloseGTpanel()
    {
        DoFade(() =>
        {
            if (gtPanel) gtPanel.SetActive(false);
            if (mainPanel) mainPanel.SetActive(true);

        });
    }

    public void DevPanel()
    {
        DoFade(() =>
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (devPanel) devPanel.SetActive(true);
        });
    }

    public void CloseDevPanel()
    {
        DoFade(() =>
        {
            if (devPanel) devPanel.SetActive(false);
            if (mainPanel) mainPanel.SetActive(true);
        });
    }

    public void Setting()
    {
        DoFade(() =>
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (setting) setting.SetActive(true);
        });
    }

    public void CloseSetting()
    {
        DoFade(() =>
        {
            if (setting) setting.SetActive(false);
            if (mainPanel) mainPanel.SetActive(true);
        });
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void DoFade(Action action)
    {
        if (isBusy) return;
        StartCoroutine(Process(action));
    }

    private IEnumerator Process(Action action)
    {
        isBusy = true;

        if (fade)
        {
            fade.SetTrigger("Fade");
        }

        yield return new WaitForSeconds(fadeTime);

        action?.Invoke();

        yield return new WaitForSeconds(fadeTime);

        isBusy = false;
    }
}