using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRequest : Request
{
    public override void DefaltRequest()
    {
        PhotonEngine.Peer.OpCustom((byte)OpCode, null, true);
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {

    }
}
