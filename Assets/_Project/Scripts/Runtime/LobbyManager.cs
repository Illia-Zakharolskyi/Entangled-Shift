/*using System.Collections;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour//PunCallbacks
{
    [Header("Панели")]
    public GameObject mainPanel;
    public GameObject lobbyPanel;

    [Header("Поля ввода")]
    public InputField joinCodeInput;

    private string currentRoomCode;

    public void OpenLobbyPanel()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Подключено к серверу Photon!");
    }

    public void OnClickCreateRoom()
    {
        StartCoroutine(CreationTimerRoutine());
    }

    IEnumerator CreationTimerRoutine()
    {
        Debug.Log("Создание комнаты через: 5 секунд...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Создание комнаты через: 4 секунды...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Создание комнаты через: 3 секунды...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Создание комнаты через: 2 секунды...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Создание комнаты через: 1 секунду...");
        yield return new WaitForSeconds(1f);

        int codeLength = Random.Range(4, 11);
        currentRoomCode = GenerateRandomCode(codeLength);

        Debug.Log("Сгенерированный код комнаты: " + currentRoomCode);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(currentRoomCode, roomOptions);
    }

    string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] stringChars = new char[length];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(stringChars);
    }

    public void OnClickJoinRoom()
    {
        string inputCode = joinCodeInput.text.ToUpper();

        if (string.IsNullOrEmpty(inputCode))
        {
            Debug.Log("Введите код комнаты!");
            return;
        }

        PhotonNetwork.JoinRoom(inputCode);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Успешный вход в комнату! Загрузка геймплея...");
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Комната не существует!");
    }
}*/