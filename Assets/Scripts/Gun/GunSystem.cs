using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.Collections;

public class GunSystem : MonoBehaviourPun {

    [Header("Gun Stats")]
    public short damage = 25;
    public float timeBetweenShooting = 0.1f;
    public float spread = 0.01f;
    public float range = 100f;
    public float reloadTime = 1.2f;
    public float timeBetweenShots = 0.1f;
    public short magazineSize = 30;
    public byte bulletsPerTap = 1;
    public bool allowButtonHold = true;

    private short bulletsLeft;
    private byte bulletsShot;
    private bool shooting, readyToShoot = true, reloading;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;

    private InputSystem_Actions input;

    private void Awake() {

        if (!photonView.IsMine) {

            enabled = false;
            return;
        }

        bulletsLeft = magazineSize;
        input = new InputSystem_Actions();
    }

    private void OnEnable() {

        input.Enable();
        input.Player.Attack.performed += _ => shooting = true;
        input.Player.Attack.canceled += _ => shooting = false;
        input.Player.Reload.performed += _ => { if (!reloading && bulletsLeft < magazineSize) Reload(); };
    }

    private void OnDisable() {

        input.Disable();
    }

    private void Update() {

        HandleShooting();
        text.SetText($"{bulletsLeft} / {magazineSize}");
    }

    private void HandleShooting() {

        if (!allowButtonHold)
            shooting = input.Player.Attack.triggered;

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {

            bulletsShot = bulletsPerTap;
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine() {

        readyToShoot = false;

        do {

            Vector3 dir = fpsCam.transform.forward +
                          new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f);

            if (Physics.Raycast(fpsCam.transform.position, dir, out RaycastHit hit, range)) {

                PhotonView hitPV = hit.collider.GetComponentInParent<PhotonView>();
                if (hitPV != null && hitPV != photonView)
                    hitPV.RPC("TakeDamage", hitPV.Owner, damage);

                Instantiate(bulletHoleGraphic, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation, attackPoint);

            bulletsLeft--;
            bulletsShot--;

            yield return new WaitForSeconds(timeBetweenShots);

        } while (bulletsShot > 0 && bulletsLeft > 0);

        yield return new WaitForSeconds(timeBetweenShooting);
        readyToShoot = true;
    }

    private void Reload() {

        reloading = true;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine() {

        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = magazineSize;
        reloading = false;
    }
}