using Common;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SyncPlayerRequest : Request
{
    private Spriteslider spriteslider;
 
    public override void DefaltRequest()
    {

    }

    public override void DefaltRequest(string select)
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.Account, select);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        ReturnCode returnCode = (ReturnCode)operationResponse.ReturnCode;
        //Debug.Log(returnCode);

        if (returnCode == ReturnCode.Success)
        {
            spriteslider = GameObject.FindWithTag("UI").GetComponent<Spriteslider>();
            gameStart = GameObject.FindWithTag("GameStart").GetComponent<GameStart>();

            gameStart.Self(gameStart.L,gameStart.Lq);
            Spriteslider.posCode = PosCode.L;
            spriteslider.NameSetSelf();
        }
        else if (returnCode == ReturnCode.Failed)
        {
            PhotonEngine.isJoin = true;
            SceneManager.LoadScene("Talk");
        }
        else
        {
            lobby =GameObject.FindWithTag("Lobby").GetComponent<LobbyUI>();
            lobby.OnJoinFailedResponse();
        }
    }
}
