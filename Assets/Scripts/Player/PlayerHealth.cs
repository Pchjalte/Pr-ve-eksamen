using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun {

    public short maxHealth = 100;
    public float camHeight = 1.5f;
    public float fadeSpeed = 1f;
    public Image deathFade;

    public bool shieldActive;

    private short currentHealth;
    private Rigidbody rb;
    private Camera cam;
    private bool dead;

    private static Transform spawnRoot;

    private void Awake() {

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        if (spawnRoot == null) {

            GameObject go = GameObject.Find("SpawnPoints");
            if (go != null)
                spawnRoot = go.transform;
        }
    }

    [PunRPC]
    public void TakeDamage(short dmg) {

        if (!photonView.IsMine || dead) return;

        if (shieldActive) {
            shieldActive = false;
            return;
        }

        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(short amount) {

        if (!photonView.IsMine || dead) return;

        currentHealth = (short)Mathf.Min(currentHealth + amount, maxHealth);
    }


    private void Die() {

        dead = true;

        cam.transform.SetParent(null);
        cam.GetComponent<AudioListener>().enabled = true;

        StartCoroutine(FadeToBlack());
        StartCoroutine(DeathSequence());

        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.RestartRound();
    }

    private IEnumerator DeathSequence() {

        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(transform.right * 8f, ForceMode.Impulse);

        Quaternion startRot = cam.transform.rotation;
        Quaternion down = Quaternion.Euler(90f, startRot.eulerAngles.y, 0f);

        for (float t = 0f; t < 1f; t += Time.deltaTime * 2f) {

            cam.transform.rotation = Quaternion.Slerp(startRot, down, t);
            yield return null;
        }

        Vector3 startPos = cam.transform.position;
        Vector3 raisedPos = startPos + Vector3.up * camHeight;

        for (float t = 0f; t < 1f; t += Time.deltaTime * 0.4f) {

            cam.transform.position = Vector3.Lerp(startPos, raisedPos, t);
            yield return null;
        }
    }

    public void ForceRespawn() {

        if (!photonView.IsMine || spawnRoot == null) return;

        int index = Random.Range(0, spawnRoot.childCount);
        Transform spawn = spawnRoot.GetChild(index);

        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);
    }

    private IEnumerator FadeToBlack() {

        Color c = deathFade.color;

        for (float t = 0f; t < 1f; t += Time.deltaTime * fadeSpeed) {

            c.a = t;
            deathFade.color = c;
            yield return null;
        }
    }
}