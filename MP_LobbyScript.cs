using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.SceneManagement;
using MLAPI.Connection;
using MLAPI;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;


public class MP_LobbyScript : NetworkBehaviour
{
    [SerializeField] private MP_LobbyPlayerCard[] lobbyPlayers;
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject playerPrefab;
    private NetworkList<MP_PlayerInfo> nwPlayers = new NetworkList<MP_PlayerInfo>();//this is the list of objects that represent the players joining the lobby 
    
    [SerializeField] private GameObject chatPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            UpdateConnListServerRpc(NetworkManager.LocalClientId);
        }
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    public override void NetworkStart()
    {
        nwPlayers.OnListChanged += HandlePlayersListChanged;

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }



    [ServerRpc]
    private void UpdateConnListServerRpc(ulong localClientId)
    {
        nwPlayers.Add(new MP_PlayerInfo(localClientId, PlayerPrefs.GetString("PName"), false));//
    }

    public void ReadyButtonToggled()
    {
        ReadyUpServerRpc();
    }
    public void StartGameButtonPressed()
    {
        if(IsServer)
        {
            NetworkSceneManager.OnSceneSwitched += SceneSwitched;
            NetworkSceneManager.SwitchScene("MainGame");
        }
        else
        {
            Debug.Log("You are not the host ");
        }
    }

    private void SceneSwitched()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach(MP_PlayerInfo tmpCLient in nwPlayers)
        {
            //get random spawn point location
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int index = UnityEngine.Random.Range(0, spawnPoints.Length);
            GameObject currentPoint= spawnPoints[index];


            //spawn player
            GameObject playerSpawn = Instantiate(playerPrefab, currentPoint.transform.position, Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpCLient.networkClientID);


            // add chat UI
            GameObject chatUISpawn = Instantiate(chatPrefab);
            chatUISpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpCLient.networkClientID);
            chatUISpawn.GetComponent<MP_ChatUIScript>().chatPlayers = nwPlayers;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void ReadyUpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for(int index = 0; index < nwPlayers.Count; index++)
        {
            if(nwPlayers[index].networkClientID == serverRpcParams.Receive.SenderClientId )
            {
                nwPlayers[index] = new MP_PlayerInfo(nwPlayers[index].networkClientID, nwPlayers[index].networkPlayerName, !nwPlayers[index].networkPlayerReady);
            }
        }
    }

    //HANDLERS
    private void HandleClientConnected(ulong clientID)
    {
        if(IsOwner)
        {
            UpdateConnListServerRpc(clientID);
        }
        Debug.Log("Client Connected: " + clientID);
    }
    private void HandleClientDisconnected(ulong clientID)
    {
        for(int index = 0; index < nwPlayers.Count; index ++)
        {
            if(clientID == nwPlayers[index].networkClientID)
            {
                nwPlayers.RemoveAt(index);
                break;
            }
        }
    }

    private void HandlePlayersListChanged(NetworkListEvent<MP_PlayerInfo> changeEvent)
    {
        int index = 0;
        // update ui on player connect 
        foreach(MP_PlayerInfo connectedPlayer in nwPlayers)
        {
            lobbyPlayers[index].playerName.text = connectedPlayer.networkPlayerName;
            lobbyPlayers[index].playerReadyToggle.SetIsOnWithoutNotify(connectedPlayer.networkPlayerReady);
            index++;
        }

        //update ui on player disconnect
        for(; index < 4; index ++)
        {
            lobbyPlayers[index].playerName.text = "Player Name";
            lobbyPlayers[index].playerReadyToggle.SetIsOnWithoutNotify(false);
        }
        
        if(IsHost)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = CheckIsEveryoneReady();
        }
    }

    private bool CheckIsEveryoneReady()
    {
        foreach (MP_PlayerInfo player in nwPlayers)
        {
            if(!player.networkPlayerReady)
            {
                return false;
            }
        }
        return true;
    }
}
