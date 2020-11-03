using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkLawnMower : MonoBehaviourPun, IPunObservable
{
    private LawnMower lawnMower = null;

    private void OnEnable()
    {
        lawnMower = GetComponentInChildren<LawnMower>();
        lawnMower.OnChangeNextTurn += OnChangeNextTurn;
    }

    private void OnDisable()
    {
        lawnMower.OnChangeNextTurn -= OnChangeNextTurn;
    }

    private void OnChangeNextTurn()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("ChangeTurnRPC", RpcTarget.Others, new object[] { lawnMower.NextTurn, lawnMower.NumberOfTurn, lawnMower.OrientationLawnMower, lawnMower.transform.position } as object);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lawnMower.Ready);
        }
        else
        {
            lawnMower.Ready = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void ChangeTurnRPC(object[] objectArray)
    {
        if(lawnMower.NumberOfTurn != (int)objectArray[1])
        {
            Debug.Log("NextTurn Local: " + lawnMower.NextTurn + "Away: " + (LawnMower.Orientation)objectArray[0]);
            Debug.Log("NumberOfTurn Local: " + lawnMower.NumberOfTurn + "Away: " + (int)objectArray[1]);
            Debug.Log("OrientationLawnMower Local: " + lawnMower.OrientationLawnMower + "Away: " + (LawnMower.Orientation)objectArray[2]);
            Debug.Log("Position Local: " + lawnMower.transform.position + "Away: " + (Vector3)objectArray[3]);
            Debug.Log("!!!");

            lawnMower.NextTurn = (int)objectArray[0];
            lawnMower.NumberOfTurn = (int)objectArray[1];
            lawnMower.OrientationLawnMower = (LawnMower.Orientation)objectArray[2];
            lawnMower.transform.position = (Vector3)objectArray[3];
        }
        else
        {
            lawnMower.NextTurn = (int)objectArray[0];
        }
    }
}
