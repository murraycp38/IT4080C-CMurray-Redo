using UnityEngine.UI;
using System;
using MLAPI.Serialization;



public struct MP_PlayerInfo : INetworkSerializable
{
    public ulong networkClientID;
    public string networkPlayerName;
    public bool networkPlayerReady;

    public MP_PlayerInfo(ulong nwClientID, string nwPName, bool playerReady)
    {
        networkClientID = nwClientID;
        networkPlayerName = nwPName;
        networkPlayerReady = playerReady;
    }
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref networkClientID);
        serializer.Serialize(ref networkPlayerName);
        serializer.Serialize(ref networkPlayerReady);
    }
}
