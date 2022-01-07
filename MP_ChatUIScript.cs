using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable;
using System;
using MLAPI.Messaging;
using MLAPI;
using MLAPI.NetworkVariable.Collections;

public class MP_ChatUIScript : NetworkBehaviour

{
    public Text chatText = null;
    public InputField chatInput = null;

    NetworkVariableString messages = new NetworkVariableString("Temp");

    public NetworkList<MP_PlayerInfo> chatPlayers;
    private string playerName = "N/A";

    private bool showScore = false;
    public GameObject scoreCardPanel;
    public Text scorePlayerName;
    public Text scoreKills;
    public Text scoreDeaths;
    // Start is called before the first frame update
    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        foreach(MP_PlayerInfo player in chatPlayers)
        {
            if(NetworkManager.LocalClientId == player.networkClientID)
            {
                playerName = player.networkPlayerName;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            //show score UI
            showScore = true;
            
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            //Hide Score Ui
            showScore = false;
        }
        
        if(showScore)
        {
            scoreCardPanel.SetActive(showScore);
            if(IsOwner)
            {
                updateUIScoreServerRpc();
            }
        }
        else
        {
            scoreCardPanel.SetActive(showScore);
        }
    }

 

    public void handleSend()
    {
       if(!IsServer)
       {
           sendMessageServerRpc(chatInput.text);
       }
       else
       {
           messages.Value += "\n" + playerName + " says: " + chatInput.text;
       }   
   
    }
    [ClientRpc]
    private void updateUIClientRpc(string previousValue, string newValue)
    {
        chatText.text += newValue.Substring(previousValue.Length, newValue.Length - previousValue.Length);
    }

    [ServerRpc]
    private void sendMessageServerRpc(string text, ServerRpcParams svrParam = default)
    {
        foreach(MP_PlayerInfo player in chatPlayers)
        {
            if(svrParam.Receive.SenderClientId == player.networkClientID)
            {
                playerName = player.networkPlayerName;
            }
        }
        messages.Value += "\n" + playerName + " says: " + text;
    }

    [ServerRpc]
    private void updateUIScoreServerRpc(ServerRpcParams svrParams = default)
    {
        //clear out old scores
        clearUIScoreClientRpc();
        //get each players info
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in currentPlayers)//this is going to loop through all the players we can find in the game. 
        {
            foreach(MP_PlayerInfo playerInfo in chatPlayers)// has all hte info of each player
            {
                if(playerObj.GetComponent<NetworkObject>().OwnerClientId == playerInfo.networkClientID)
                {
                    updateUIScoreClientRpc(playerInfo.networkPlayerName, playerObj.GetComponent<MP_PlayerAttribs>().kills.Value, playerObj.GetComponent<MP_PlayerAttribs>().deaths.Value);
                }
            }
        }
    }

    [ClientRpc]
    private void updateUIScoreClientRpc(string networkPlayerName, int kills, int deaths)
    {
        if(IsOwner)
        {
            scorePlayerName.text += networkPlayerName + "\n";
            scoreKills.text += kills + "\n";
            scoreDeaths.text += deaths + "\n";
        }
    }
    [ClientRpc]
    private void clearUIScoreClientRpc()
    {
        if(IsOwner)
        {
            scorePlayerName.text = "";
            scoreKills.text = "";
            scoreDeaths.text = "";
        }
    }
}
