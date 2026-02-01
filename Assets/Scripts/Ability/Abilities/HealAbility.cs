using UnityEngine;
using Photon.Pun;
using System.Collections;

public sealed class HealAbility : AbilityBase {

    public short healAmount = 30;
    public float cooldown = 5f;

    private PlayerHealth health;

    public override void Initialize() {
        health = GetComponent<PlayerHealth>();
    }

    public override void OnAbilityPressed() {

        if (!isReady || !photonView.IsMine) return;

        health.Heal(healAmount);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine() {

        isReady = false;
        yield return new WaitForSeconds(cooldown);
        isReady = true;
    }
}
