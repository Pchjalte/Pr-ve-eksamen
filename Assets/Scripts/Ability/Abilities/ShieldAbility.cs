using UnityEngine;
using Photon.Pun;

public sealed class ShieldAbility : AbilityBase {

    private PlayerHealth health;

    public override void Initialize() {

        if (!photonView.IsMine) return;

        health = GetComponent<PlayerHealth>();
        health.shieldActive = true;
    }
}