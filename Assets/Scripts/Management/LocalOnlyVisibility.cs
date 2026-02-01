using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalOnlyVisibility : MonoBehaviourPun {

    public List<GameObject> localOnlyObjects;
    public List<GameObject> hideForLocalPlayer;

    private void Start() {

        bool isMine = photonView.IsMine;

        SetObjects(localOnlyObjects, isMine);
        SetObjects(hideForLocalPlayer, !isMine);
    }

    private static void SetObjects(List<GameObject> list, bool state) {

        if (list == null) return;

        for (int i = 0; i < list.Count; i++) {

            if (list[i] != null)
                list[i].SetActive(state);
        }
    }
}