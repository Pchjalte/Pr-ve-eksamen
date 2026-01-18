using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager Instance;

    private void Awake() {

        Instance = this;
    }

    public void RestartRound() {

        StartCoroutine(RestartDelay());
    }

    System.Collections.IEnumerator RestartDelay() {

        yield return new WaitForSeconds(3f);
        photonView.RPC(nameof(RPC_RestartRound), RpcTarget.All);
    }


    [PunRPC]
    void RPC_RestartRound() {

        PlayerHealth[] players = Object.FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (PlayerHealth p in players) {

            p.ForceRespawn();
        }
    }
}