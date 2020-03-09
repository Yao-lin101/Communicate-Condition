using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using ExitGames.Client.Photon;

public abstract class Request :MonoBehaviour
{
    public OperationCode OpCode;
    public abstract void DefaltRequest();
    public abstract void DefaltRequest(string select);
    public abstract void OnOperationResponse(OperationResponse operationResponse);
    [HideInInspector]
    public LoginUI loginUI;
    [HideInInspector]
    public LobbyUI lobby;
    [HideInInspector]
    public GameStart gameStart;

    public void Start()
    {
        try
        {
            loginUI = GetComponent<LoginUI>();
            lobby = GameObject.FindWithTag("Lobby").GetComponent<LobbyUI>();
            gameStart = GameObject.FindWithTag("GameStart").GetComponent<GameStart>();
        }
        catch
        {

        }
        PhotonEngine.Instance.AddRequest(this);
    }
    public void OnDestroy()
    {
        PhotonEngine.Instance.RemoveRequest(this);
    }
}
