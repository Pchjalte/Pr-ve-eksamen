using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPun {

    public int maxHealth = 100;
    private int currentHealth;

    private void Awake() {

        currentHealth = maxHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage) {

        if (!photonView.IsMine) return;

        currentHealth -= damage;

        if (currentHealth <= 0) {

            Die();
        }
    }

    private void Die() {

        PhotonNetwork.Destroy(gameObject);
    }
}