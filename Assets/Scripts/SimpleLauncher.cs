using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SimpleLauncher : MonoBehaviourPunCallbacks {

    public PhotonView playerPrefab;

    void Start() {

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {

        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {

        Debug.Log("Joined a room.");
        Transform spawns = GameObject.Find("SpawnPoints").transform;

        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawns.childCount;
        Transform spawn = spawns.GetChild(index);

        PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);
    }

}