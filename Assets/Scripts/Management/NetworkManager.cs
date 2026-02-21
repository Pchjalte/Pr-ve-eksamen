using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks {

    public static NetworkManager Instance;

    public PhotonView playerPrefab;

    private Transform spawnRoot;
    private Dictionary<string, RoomInfo> cachedRooms = new Dictionary<string, RoomInfo>();

    private bool isReadyForRooms = false;

    void Awake() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {

            Destroy(gameObject);
        }
    }

    void Start() {

        PhotonNetwork.LogLevel = PunLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable() {

        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable() {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDisable();
    }

    public override void OnConnectedToMaster() {

        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {

        isReadyForRooms = true;
        Debug.Log("Photon ready for matchmaking.");
    }
    public void JoinPrivate(string code) {
        if (!isReadyForRooms) {

            Debug.LogWarning("Photon not ready yet.");
            return;
        }

        PhotonNetwork.JoinRoom(code);
    }

    public void CreateRoom(string name, string code, bool isPublic) {

        if (!isReadyForRooms) {

            Debug.LogWarning("Photon not ready yet.");
            return;
        }

        string roomName = isPublic ? name : code;

        RoomOptions options = new RoomOptions {

            MaxPlayers = 2,
            IsVisible = isPublic,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, options);
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

    public void JoinPublicRoom(string name) {

        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinedRoom() {

        SceneManager.LoadScene(3);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (scene.buildIndex != 3 || !PhotonNetwork.InRoom)
            return;

        spawnRoot = GameObject.Find("SpawnPoints")?.transform;

        if (spawnRoot == null || spawnRoot.childCount == 0) {

            Debug.LogError("SpawnPoints object missing or empty.");
            return;
        }

        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnRoot.childCount;

        Transform spawn = spawnRoot.GetChild(index);

        PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);

        GameObject playerObj = PhotonNetwork.Instantiate (
        playerPrefab.name,
        spawn.position,
        spawn.rotation
        );

        Player player = PhotonNetwork.LocalPlayer;

        GunID gun = PlayerLoadout.GetGun(player);
        AbilityID ability = PlayerLoadout.GetAbility(player);

        playerObj.GetComponent<PlayerLoadoutApplier>()
            .Apply(gun, ability);
    }
}