using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MP_LobbyPlayerCard : MonoBehaviour
{
    [SerializeField] public Text playerName;
    [SerializeField] public Image playerIcon;
    [SerializeField] public Toggle playerReadyToggle;

   internal void UpdatePlayerName(Text playerNameIn)
   {
       playerName = playerNameIn;
   }
   internal void UpdatePlayerIcon(Image playerIconIn)
   {
       playerIcon = playerIconIn;
   }
   internal void UpdatePlayerReadyToggle(Toggle playerReadyToggleIn)
   {
       playerReadyToggle = playerReadyToggleIn;
   }
}
