using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string roomName { get; set; }

    public delegate void OnRoomJoinedHandler(object sender);
    public event OnRoomJoinedHandler OnJoinedRoomEvent;

    public void Connect(string roomName)
    {
        this.roomName = roomName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server. Joining Room");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        OnJoinedRoomEvent?.Invoke(this);
    }
}
