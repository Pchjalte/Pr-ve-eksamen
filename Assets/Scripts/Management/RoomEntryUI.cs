using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomEntryUI : MonoBehaviour {

    public TMP_Text label;
    public Button joinButton;

    private string roomName;

    public void Setup(RoomInfo info) {

        roomName = info.Name;

        label.text = $"{info.Name} ({info.PlayerCount}/{info.MaxPlayers})";

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(JoinRoom);
    }

    void JoinRoom() {

        NetworkManager.Instance.JoinPublicRoom(roomName);
    }
}