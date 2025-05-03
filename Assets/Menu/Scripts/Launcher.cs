using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks {
  public static Launcher Instance;

  [SerializeField] TMP_InputField playerNameInputField;
  [SerializeField] TMP_Text titleWelcomeText;
  [SerializeField] TMP_InputField roomNameInputField;
  [SerializeField] Transform roomListContent;
  [SerializeField] GameObject roomListItemPrefab;
  [SerializeField] TMP_Text roomNameText;
  [SerializeField] Transform playerListContent;
  [SerializeField] GameObject playerListItemPrefab;
  [SerializeField] GameObject startGameButton;
  [SerializeField] TMP_Text errorText;
    [SerializeField] GameObject myPlayerObject; 
    [SerializeField] GameObject emptySpawnPoint;
    [SerializeField] TMP_InputField maxPlayersInputField;
    [SerializeField] TMP_Text playerCountText; 

    private void Awake() {
    Instance = this;
  }

  private void Start() {
    Debug.Log("Connecting to master...");
    PhotonNetwork.ConnectUsingSettings();
  }

  public override void OnConnectedToMaster() {
    Debug.Log("Connected to master!");
    PhotonNetwork.JoinLobby();
    // Automatically load scene for all clients when the host loads a scene
    PhotonNetwork.AutomaticallySyncScene = true;
  }

  public override void OnJoinedLobby() {
    if (PhotonNetwork.NickName == "") {
      PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString(); // Set a default nickname, just as a backup
      MenuManager.Instance.OpenMenu("name");
    } else {
      MenuManager.Instance.OpenMenu("title");
    }
    Debug.Log("Joined lobby");
  }

  public void SetName() {
    string name = playerNameInputField.text;
    if (!string.IsNullOrEmpty(name)) {
      PhotonNetwork.NickName = name;
      titleWelcomeText.text = $"Welcome, {name}!";
      MenuManager.Instance.OpenMenu("title");
      playerNameInputField.text = "";
    } else {
      Debug.Log("No player name entered");
      // TODO: Display an error to the user
    }
  }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
        {
            // Configura las opciones de la sala, incluyendo el máximo de jugadores
            RoomOptions roomOptions = new RoomOptions();

            // Si no se introduce un número, usa un valor por defecto (ej: 4)
            int maxPlayers = 4; // Valor por defecto
            if (!string.IsNullOrEmpty(maxPlayersInputField.text))
            {
                maxPlayers = int.Parse(maxPlayersInputField.text);
            }

            roomOptions.MaxPlayers = (byte)maxPlayers; // Photon usa byte para MaxPlayers

            PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
            MenuManager.Instance.OpenMenu("loading");
            roomNameInputField.text = "";
            maxPlayersInputField.text = ""; // Limpiar el campo después de usarlo

            if (myPlayerObject != null)
            {
                myPlayerObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("No room name entered");
        }
    }

    public override void OnJoinedRoom()
    {
        // Reactivar y mover el objeto del jugador (tu código actual)
        if (myPlayerObject != null && emptySpawnPoint != null)
        {
            myPlayerObject.SetActive(true);
            myPlayerObject.transform.position = emptySpawnPoint.transform.position;
        }

        // Actualizar UI
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // Mostrar el límite de jugadores (ej: "2/4")
        playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        // Limpiar y repoblar la lista de jugadores
        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
    startGameButton.SetActive(PhotonNetwork.IsMasterClient);
  }

  public void LeaveRoom() {
    PhotonNetwork.LeaveRoom();
    MenuManager.Instance.OpenMenu("loading");
  }

  public void JoinRoom(RoomInfo info) {
    PhotonNetwork.JoinRoom(info.Name);
    MenuManager.Instance.OpenMenu("loading");

  }

  public override void OnLeftRoom() {
    MenuManager.Instance.OpenMenu("title");
  }

  public override void OnRoomListUpdate(List<RoomInfo> roomList) {
    foreach (Transform trans in roomListContent) {
      Destroy(trans.gameObject);
    }
    for (int i = 0; i < roomList.Count; i++) {
      if (roomList[i].RemovedFromList) {
        // Don't instantiate stale rooms
        continue;
      }
      Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
    }
  }

  public override void OnCreateRoomFailed(short returnCode, string message) {
    errorText.text = "Room Creation Failed: " + message;
    MenuManager.Instance.OpenMenu("error");
  }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}"; // Actualizar
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Photon ya actualiza PlayerCount automáticamente, solo refrescamos UI
        playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void StartGame() {

        myPlayerObject.SetActive(false);
        MenuManager.Instance.OpenMenu("loading");
        // 1 is used as the build index of the game scene, defined in the build settings
        // Use this instead of scene management so that *everyone* in the lobby goes into this scene
        PhotonNetwork.LoadLevel(1); 
  }

  public void QuitGame() {
    Application.Quit();
  }
}
