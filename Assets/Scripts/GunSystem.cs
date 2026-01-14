using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviourPun {

    [Header("Gun Stats")]
    public int damage = 25;
    public float timeBetweenShooting = 0.1f;
    public float spread = 0.01f;
    public float range = 100f;
    public float reloadTime = 1.2f;
    public float timeBetweenShots = 0.1f;
    public int magazineSize = 30;
    public int bulletsPerTap = 1;
    public bool allowButtonHold = true;

    private int bulletsLeft, bulletsShot;
    private bool shooting, readyToShoot = true, reloading;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask playerLayer;

    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;

    private InputSystem_Actions input;

    private void Awake() {

        bulletsLeft = magazineSize;

        if (!photonView.IsMine) {

            enabled = false;
            return;
        }

        input = new InputSystem_Actions();
    }

    private void OnEnable() {

        if (!photonView.IsMine) return;

        input.Enable();

        input.Player.Attack.performed += _ => shooting = true;
        input.Player.Attack.canceled += _ => shooting = false;

        input.Player.Reload.performed += _ => {

            if (bulletsLeft < magazineSize && !reloading)
                Reload();
        };
    }

    private void OnDisable() {

        if (!photonView.IsMine) return;
        input.Disable();
    }

    private void Update() {

        if (!photonView.IsMine) return;

        HandleShooting();
        text.SetText($"{bulletsLeft} / {magazineSize}");
    }

    private void HandleShooting() {

        if (!allowButtonHold)
            shooting = input.Player.Attack.triggered;

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {

            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot() {

        readyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit hit, range)) {

            PhotonView hitPV = hit.collider.GetComponentInParent<PhotonView>();

            // Hit another player (not yourself)
            if (hitPV != null && hitPV != photonView) {

                hitPV.RPC("TakeDamage", hitPV.Owner, damage);
            }

            Instantiate(bulletHoleGraphic, hit.point, Quaternion.LookRotation(hit.normal));
        }

        GameObject flash = Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation, attackPoint);

        Destroy(flash, 0.15f);


        bulletsLeft--;
        bulletsShot--;

        Invoke(nameof(ResetShot), timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);
    }

    private void ResetShot() {

        readyToShoot = true;
    }

    private void Reload() {

        reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished() {

        bulletsLeft = magazineSize;
        reloading = false;
    }
}