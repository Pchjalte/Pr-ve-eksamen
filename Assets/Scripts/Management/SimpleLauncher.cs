using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SimpleLauncher : MonoBehaviourPunCallbacks {

    public PhotonView playerPrefab;
    private Transform spawnRoot;

    private void Start() {

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {

        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {

        if (spawnRoot == null)
            spawnRoot = GameObject.Find("SpawnPoints")?.transform;

        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnRoot.childCount;
        Transform spawn = spawnRoot.GetChild(index);

        PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);
    }
}