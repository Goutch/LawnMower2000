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
            photonView.RPC("ChangeTurnRPC", RpcTarget.Others, new object[] { lawnMower.NextTurn, lawnMower.NumberOfTurn, lawnMower.OrientationLawnMower, lawnMower.transform.position, lawnMower.NextTilePosition.x, lawnMower.NextTilePosition.y, lawnMower.Points } as object);
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
        if (lawnMower.NumberOfTurn != (int)objectArray[1])
        {
            lawnMower.NextTurn = (int)objectArray[0];
            lawnMower.NumberOfTurn = (int)objectArray[1];
            lawnMower.OrientationLawnMower = (LawnMower.Orientation)objectArray[2];
            lawnMower.transform.position = (Vector3)objectArray[3];
            lawnMower.NextTilePosition = new Vector2Int((int)objectArray[4], (int)objectArray[5]);
            lawnMower.Points = (int)objectArray[5];
        }
        else
        {
            lawnMower.NextTurn = (int)objectArray[0];
        }
    }
}
