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
            photonView.RPC("ChangeTurnRPC", RpcTarget.Others, new object[] { lawnMower.NextTurn } as object);
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
        lawnMower.NextTurn = (int)objectArray[0];
    }
}
