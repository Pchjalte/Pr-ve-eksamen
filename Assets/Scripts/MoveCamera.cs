using Photon.Pun;
using UnityEngine;

public class MoveCamera : MonoBehaviourPun {

    public Transform player;
    void Update() {

        if (!photonView.IsMine) return;

        transform.position = player.position;
    }
}