using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;
using MLAPI.SceneManagement;

//MP_Start is gonna take the player name from the lobby scene and store it as a universal/global variable using the player pref stuff
public class MP_Start : MonoBehaviour
{
    [SerializeField] //Allows us to access private field in editor: the resason we do this is because it allows us to access the variables in the editor but they will  still be provate
    private InputField playerName;

    public void HostButtonClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartHost();
        NetworkSceneManager.SwitchScene("Lobby");
    }

    public void ClientButtonClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();

    }
}
