using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public sealed class AbilityController : MonoBehaviourPun {

    [Tooltip("Exactly one ability for now")]
    public AbilityBase equippedAbility;

    private InputSystem_Actions input;

    private void Awake() {

        if (!photonView.IsMine) {

            enabled = false;
            return;
        }

        input = new InputSystem_Actions();
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