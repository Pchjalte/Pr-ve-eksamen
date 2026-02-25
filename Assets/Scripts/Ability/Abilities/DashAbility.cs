using UnityEngine;
using Photon.Pun;
using System.Collections;

public sealed class DashAbility : AbilityBase {

    public float dashForce = 200f;
    public float cooldown = 10f;

    private Rigidbody rb;

    public override void Initialize() {

        rb = GetComponent<Rigidbody>();
    }

    public override void OnAbilityPressed() {

        if (!isReady || !photonView.IsMine) return;

        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine() {

        isReady = false;
        yield return new WaitForSeconds(cooldown);
        isReady = true;
    }
}