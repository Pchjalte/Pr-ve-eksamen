using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalOnlyVisibility : MonoBehaviourPun {
    [Header("Visible ONLY for Local Player")]
    public List<GameObject> localOnlyObjects = new List<GameObject>();

    [Header("Hidden ONLY for Local Player")]
    public List<GameObject> hideForLocalPlayer = new List<GameObject>();

    private void Start() {
        if (photonView.IsMine) {
            // Show only for me
            SetObjectsActive(localOnlyObjects, true);

            // Hide for me
            SetObjectsActive(hideForLocalPlayer, false);
        } else {
            // Hide local-only objects from other players
            SetObjectsActive(localOnlyObjects, false);
        }
    }

    private void SetObjectsActive(List<GameObject> objects, bool state) {
        foreach (GameObject obj in objects) {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}