using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class ReadySystem : MonoBehaviourPunCallbacks {
    public TMP_Text readyText;

    const string READY = "r";

    void Start() {
        UpdateReadyUI();
    }

    public void ToggleReady() {
        bool ready = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(READY, out object r))
            ready = (bool)r;

        Hashtable table = new Hashtable();
        table[READY] = !ready;

        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changed) {
        if (changed.ContainsKey(READY))
            UpdateReadyUI();

        CheckStartCondition();
    }

    void UpdateReadyUI() {
        int readyCount = 0;

        foreach (Player p in PhotonNetwork.PlayerList) {
            if (p.CustomProperties.TryGetValue(READY, out object r) && (bool)r)
                readyCount++;
        }

        readyText.text =
            readyCount + " / " + PhotonNetwork.PlayerList.Length + " Ready";
    }

    void CheckStartCondition() {
        if (!PhotonNetwork.IsMasterClient)
            return;

        foreach (Player p in PhotonNetwork.PlayerList) {
            if (!p.CustomProperties.TryGetValue(READY, out object r) || !(bool)r)
                return;
        }

        PhotonNetwork.LoadLevel(4);
    }
}