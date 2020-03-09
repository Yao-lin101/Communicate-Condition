using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using ExitGames.Client.Photon;
using Common.Tools;

public class UserNameRequest : Request
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
        ReturnCode returnCode = (ReturnCode)operationResponse.ReturnCode;
        string userName = (string)DictTool.GteValue<byte, object>(operationResponse.Parameters, (byte)ParameterCode.Username);
        PhotonEngine.userName = userName;
        //Debug.Log(userName);
        lobby.OnNameResponse(returnCode);
    }
}
