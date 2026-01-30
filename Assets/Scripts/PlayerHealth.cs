using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun {

    public int maxHealth = 100;
    int currentHealth;
    public float camHeight = 1.5f;
    public float fadeSpeed;

    public Image deathFade;

    Rigidbody rb;
    Camera cam;

    bool dead = false;

    void Awake() {

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
    }

    [PunRPC]
    public void TakeDamage(int dmg) {

        if (!photonView.IsMine || dead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    void Die() {

        dead = true;
        cam.transform.SetParent(null);
        cam.GetComponent<AudioListener>().enabled = true;
        StartCoroutine(FadeToBlack());
        StartCoroutine(DeathSequence());

        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.RestartRound();
    }

    IEnumerator DeathSequence() {

        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(transform.right * 8f, ForceMode.Impulse);

        Quaternion startRot = cam.transform.rotation;
        Quaternion down = Quaternion.Euler(90f, cam.transform.eulerAngles.y, 0f);

        float t = 0;
        while (t < 1) {

            cam.transform.rotation = Quaternion.Slerp(startRot, down, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }

        Vector3 startPos = cam.transform.position;
        Vector3 raisedPos = startPos + Vector3.up * camHeight;

        t = 0;
        while (t < 1) {

            cam.transform.position = Vector3.Lerp(startPos, raisedPos, t);
            t += Time.deltaTime * 0.4f;
            yield return null;
        }

    }

    public void ForceRespawn() {

        if (!photonView.IsMine) return;

        Transform spawns = GameObject.Find("SpawnPoints").transform;
        Transform spawn = spawns.GetChild(Random.Range(0, spawns.childCount));

        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);
    }

    IEnumerator FadeToBlack() {
        float t = 0;
        Color c = deathFade.color;

        while (t < 1) {
            t += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(0, 1, t);
            deathFade.color = c;
            yield return null;
        }
    }
}