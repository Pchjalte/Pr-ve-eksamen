using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour {
    public TMP_InputField roomNameField;
    public TMP_InputField createCodeField;
    public TMP_InputField joinCodeField;

    public Button createButton;
    public Button joinPrivateButton;

    void Start() {
        roomNameField.onValueChanged.AddListener(_ => ValidateButtons());
        createCodeField.onValueChanged.AddListener(_ => ValidateButtons());
        joinCodeField.onValueChanged.AddListener(_ => ValidateButtons());

        ValidateButtons();
    }

    void ValidateButtons() {
        bool publicName = !string.IsNullOrWhiteSpace(roomNameField.text);
        bool privateCreate = !string.IsNullOrWhiteSpace(createCodeField.text);
        bool privateJoin = !string.IsNullOrWhiteSpace(joinCodeField.text);

        createButton.interactable = publicName || privateCreate;
        joinPrivateButton.interactable = privateJoin;
    }

    public void CreateRoom() {
        string publicName = roomNameField.text.Trim();
        string privateCode = createCodeField.text.Trim();

        if (!string.IsNullOrEmpty(privateCode)) {
            NetworkManager.Instance.CreateRoom("", privateCode, false);
            return;
        }

        if (!string.IsNullOrEmpty(publicName)) {
            NetworkManager.Instance.CreateRoom(publicName, "", true);
        }
    }

    public void JoinPrivateRoom() {
        string code = joinCodeField.text.Trim();

        if (string.IsNullOrEmpty(code))
            return;

        NetworkManager.Instance.JoinPrivate(code);
    }
}