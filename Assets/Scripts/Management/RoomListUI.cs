using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomListUI : MonoBehaviour {

    public static RoomListUI Instance;

    public Transform contentRoot;
    public GameObject roomEntryPrefab;

    private readonly List<GameObject> spawnedEntries = new();

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

        for (int i = 0; i < spawnedEntries.Count; i++)
            Destroy(spawnedEntries[i]);

        spawnedEntries.Clear();
    }
}