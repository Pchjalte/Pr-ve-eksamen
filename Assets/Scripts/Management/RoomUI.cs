using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour {

    public TMP_InputField roomNameField;
    public TMP_InputField roomCodeField;

    public Button createButton;

    void Start() {

        roomNameField.onValueChanged.AddListener(_ => ValidateCreateButton());
        roomCodeField.onValueChanged.AddListener(_ => ValidateCreateButton());

        ValidateCreateButton();
    }

    void ValidateCreateButton() {

        bool hasPublicName = !string.IsNullOrWhiteSpace(roomNameField.text);
        bool hasCode = !string.IsNullOrWhiteSpace(roomCodeField.text);

        createButton.interactable = hasPublicName || hasCode;
    }

    public void CreateRoom() {

        string name = roomNameField.text.Trim();
        string code = roomCodeField.text.Trim();

        bool isPrivate = !string.IsNullOrEmpty(code);

        NetworkManager.Instance.CreateRoom(name, code, !isPrivate);
    }
}