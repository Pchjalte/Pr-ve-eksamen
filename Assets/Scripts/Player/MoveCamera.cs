using Photon.Pun;
using UnityEngine;

public class MoveCamera : MonoBehaviourPun {

    public Transform player;
    private Transform cachedTransform;

    private void Awake() {

        cachedTransform = transform;
    }

    private void LateUpdate() {

        if (!photonView.IsMine || player == null) return;
        cachedTransform.position = player.position;
    }
}