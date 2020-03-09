using Common;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserNameSetRequest : Request
{
    public override void DefaltRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.Username, lobby.userName);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
        PhotonEngine.userName = lobby.userName;
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {

    }
}
