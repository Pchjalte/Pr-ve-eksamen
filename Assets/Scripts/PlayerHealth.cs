using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviourPun {

    public int maxHealth = 100;
    int currentHealth;

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
        StartCoroutine(DeathSequence());

        // Restart the whole match
        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.RestartRound();
    }

    IEnumerator DeathSequence() {

        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(transform.right * 8f, ForceMode.Impulse);

        // look straight down
        Quaternion startRot = cam.transform.rotation;
        Quaternion down = Quaternion.Euler(90f, cam.transform.eulerAngles.y, 0f);

        float t = 0;
        while (t < 1) {

            cam.transform.rotation = Quaternion.Slerp(startRot, down, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }


        // slowly raise camera upward
        Vector3 startPos = cam.transform.position;
        Vector3 raisedPos = startPos + Vector3.up * 1.5f;

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
}