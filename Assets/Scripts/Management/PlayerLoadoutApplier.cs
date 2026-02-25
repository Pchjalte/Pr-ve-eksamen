using UnityEngine;

public class PlayerLoadoutApplier : MonoBehaviour {

    public GameObject[] gunPrefabs;
    public Transform gunHolder;

    public void Apply(GunID gun, AbilityID ability) {

        ApplyGun((int)gun);
        ApplyAbility(ability);
    }

    void ApplyGun(int id) {

        if (id < 0 || id >= gunPrefabs.Length)
            return;

        GameObject gunObj = Instantiate(gunPrefabs[id], gunHolder);

        GunSystem gun = gunObj.GetComponent<GunSystem>();

        if (gun == null)
            return;

        gun.fpsCam = GetComponentInChildren<Camera>();
        gun.text = GameObject.Find("Ammo")?.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void ApplyAbility(AbilityID id) {

        AbilityController controller = GetComponent<AbilityController>();

        if (controller == null) {

            Debug.LogError("AbilityController missing on player.");
            return;
        }

        AbilityBase ability = null;

        switch (id) {

            case AbilityID.Dash:
                ability = gameObject.AddComponent<DashAbility>();
                break;

            case AbilityID.Shield:
                ability = gameObject.AddComponent<ShieldAbility>();
                break;

            case AbilityID.Heal:
                ability = gameObject.AddComponent<HealAbility>();
                break;
        }

        controller.EquipAbility(ability);
    }
}