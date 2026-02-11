using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomListUI : MonoBehaviour {

    public static RoomListUI Instance;

    public Transform contentRoot;
    public GameObject roomEntryPrefab;

    private List<GameObject> spawnedEntries = new List<GameObject>();

    void Awake() {

        Instance = this;
    }

    public void Refresh(Dictionary<string, RoomInfo> rooms) {

        ClearList();

        foreach (RoomInfo room in rooms.Values) {

            if (!room.IsVisible || !room.IsOpen)
                continue;

            GameObject entry = Instantiate(roomEntryPrefab, contentRoot);

            RoomEntryUI ui = entry.GetComponent<RoomEntryUI>();

            ui.Setup(room);

            spawnedEntries.Add(entry);
        }
    }

    void ClearList() {

        foreach (GameObject go in spawnedEntries)
            Destroy(go);

        spawnedEntries.Clear();
    }
}