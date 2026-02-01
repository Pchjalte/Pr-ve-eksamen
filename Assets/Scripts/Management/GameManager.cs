using Photon.Pun;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RestartRound() {

        StartCoroutine(RestartDelay());
    }

    private IEnumerator RestartDelay() {

        yield return new WaitForSeconds(3f);
        photonView.RPC(nameof(RPC_RestartRound), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_RestartRound() {

        PlayerHealth[] players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
            players[i].ForceRespawn();
    }
}