using UnityEngine;
using Photon.Pun;
using System.Collections;

public sealed class DashAbility : AbilityBase {
    public float dashForce = 12f;
    public float cooldown = 2f;

    private Rigidbody rb;
    private Camera playerCam;

    public override void Initialize() {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
            playerCam = GetComponentInChildren<Camera>();
    }

    public override void OnAbilityPressed() {
        if (!isReady || !photonView.IsMine)
            return;

        if (playerCam == null)
            return;

        Vector3 dashDir = playerCam.transform.forward;
        dashDir.y = 0f;
        dashDir.Normalize();

        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine() {
        isReady = false;
        yield return new WaitForSeconds(cooldown);
        isReady = true;
    }
}