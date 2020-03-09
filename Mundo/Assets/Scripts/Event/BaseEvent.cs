using Common;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvent : MonoBehaviour
{
    public EventCode EventCode;
    public abstract void OnEvent(EventData eventData);
    [HideInInspector]
    public LoginUI loginUI;
    [HideInInspector]
    public LobbyUI lobby;
    [HideInInspector]
    public GameStart gameStart;

    public virtual void Start()
    {
        loginUI = GetComponent<LoginUI>();
        lobby = GetComponent<LobbyUI>();
        gameStart = GetComponent<GameStart>();
        PhotonEngine.Instance.AddEvent(this);
    }
    public void OnDestroy()
    {
        PhotonEngine.Instance.RemoveEvent(this);
    }
}
