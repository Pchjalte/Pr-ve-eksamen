using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviourPun {

    public void LoadScene(int sceneID) {

        SceneManager.LoadScene(sceneID);
    }

    public void Exit() {

        Application.Quit();
    }
}