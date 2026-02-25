using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

    public void Exit() {

        Application.Quit();
    }

    public void MainMenu() {

        SceneManager.LoadScene(0);
    }

    public void RoomJoinOrCreate() {

        SceneManager.LoadScene(1);
    }

    public void RoomList() {

        SceneManager.LoadScene(2);
    }

    public void CharacterSelect() {

        SceneManager.LoadScene(3);
    }

    public void JoinGame() {

        SceneManager.LoadScene(4);
    }
}