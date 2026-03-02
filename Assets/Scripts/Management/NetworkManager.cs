using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public static NetworkManager Instance;

    public PhotonView playerPrefab;

    Transform spawnRoot;

    bool playerSpawned;

    Dictionary<string, RoomInfo> cachedRooms = new();

    string pendingRoomName;
    bool pendingPublic;
    bool pendingCreate;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected) {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        Debug.Log("Joined Lobby");

        if (pendingCreate) {
            pendingCreate = false;
            CreateRoomInternal(pendingRoomName, pendingPublic);
        }
    }

    public void CreateRoom(string name, string code, bool isPublic) {
        Debug.Log("CreateRoom pressed");

        string roomName = isPublic ? name : code;

        if (!PhotonNetwork.IsConnected) {
            Debug.LogWarning("Photon not connected yet");
            return;
        }

        if (!PhotonNetwork.InLobby) {
            Debug.Log("Not in lobby yet, joining lobby first");

            pendingRoomName = roomName;
            pendingPublic = isPublic;
            pendingCreate = true;

            PhotonNetwork.JoinLobby();
            return;
        }

        CreateRoomInternal(roomName, isPublic);
    }

    void CreateRoomInternal(string roomName, bool isPublic) {
        Debug.Log("Creating room: " + roomName);

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = isPublic,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, options);
        Debug.Log("Creating room v2: " + roomName);

    }

    public void JoinPrivate(string code) {
        if (!PhotonNetwork.InLobby) {
            Debug.LogWarning("Not in lobby yet.");
            return;
        }
        Debug.Log("joining private room : " + code);

        PhotonNetwork.JoinRoom(code);
    }

    public void JoinPublicRoom(string name) {
        if (!PhotonNetwork.InLobby) {
            Debug.LogWarning("Not in lobby yet.");
            return;
        }
        Debug.Log("joining public room : " + name);

        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined Room successfully");

        SceneManager.LoadScene(3);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogError("CreateRoom failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("JoinRoom failed: " + message);
    }

    public override void OnLeftRoom() {
        Debug.Log("Left Room");

        playerSpawned = false;

        PhotonNetwork.JoinLobby();

        SceneManager.LoadScene(0);
    }

    public override void OnRoomListUpdate(List<RoomInfo> rooms) {
        foreach (RoomInfo info in rooms) {
            if (info.RemovedFromList)
                cachedRooms.Remove(info.Name);
            else
                cachedRooms[info.Name] = info;
        }

        RoomListUI.Instance?.Refresh(cachedRooms);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex != 4)
            return;

        if (!PhotonNetwork.InRoom)
            return;

        if (playerSpawned)
            return;

        playerSpawned = true;

        spawnRoot = GameObject.Find("SpawnPoints")?.transform;

        if (spawnRoot == null || spawnRoot.childCount == 0) {
            Debug.LogError("SpawnPoints missing.");
            return;
        }

        int index =
            (PhotonNetwork.LocalPlayer.ActorNumber - 1)
            % spawnRoot.childCount;

        Transform spawn = spawnRoot.GetChild(index);

        PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawn.position,
            spawn.rotation
        );
    }
}