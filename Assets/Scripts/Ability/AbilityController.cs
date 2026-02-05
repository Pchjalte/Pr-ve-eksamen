using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public sealed class AbilityController : MonoBehaviourPun {

    public AbilityBase equippedAbility;

    private InputSystem_Actions input;

    private void Awake() {

        if (!photonView.IsMine) {

            enabled = false;
            return;
        }

        input = new InputSystem_Actions();
    }

    public void EquipAbility(AbilityBase newAbility) {

        if (equippedAbility == newAbility)
            return;

        if (equippedAbility != null)
            equippedAbility.enabled = false;

        equippedAbility = newAbility;

        if (equippedAbility != null) {
            equippedAbility.enabled = true;
            equippedAbility.Initialize();
        }
    }

    private void OnEnable() {

        input.Enable();
        input.Player.Ability.performed += _ => equippedAbility?.OnAbilityPressed();
    }

    private void OnDisable() {

        input.Disable();
    }

    private void Start() {

        equippedAbility?.Initialize();
    }
}