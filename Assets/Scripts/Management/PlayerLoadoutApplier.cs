using UnityEngine;

public class PlayerLoadoutApplier : MonoBehaviour {

    public GameObject[] gunPrefabs;
    public MonoBehaviour[] abilityScripts;

    public Transform gunSocket;

    public void Apply(GunID gun, AbilityID ability) {

        SpawnGun((int)gun);
        EnableAbility((int)ability);
    }

    void SpawnGun(int id) {

        Instantiate(
            gunPrefabs[id],
            gunSocket.position,
            gunSocket.rotation,
            gunSocket
        );
    }

    void EnableAbility(int id) {

        for (int i = 0; i < abilityScripts.Length; i++)
            abilityScripts[i].enabled = (i == id);
    }
}