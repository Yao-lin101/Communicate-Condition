using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RegisterRequest : Request
{
    public override void DefaltRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.Account, loginUI.account);
        data.Add((byte)ParameterCode.Password, loginUI.password);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        ReturnCode returnCode = (ReturnCode)operationResponse.ReturnCode;
        loginUI.OnRegisterResponse(returnCode);
    }
}
