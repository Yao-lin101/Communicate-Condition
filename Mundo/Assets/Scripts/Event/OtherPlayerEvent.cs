using Common;
using Common.Tools;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerEvent : BaseEvent
{
    private Spriteslider spriteslider;
    [SerializeField]
    private GameObject Trigger = null;
    private Mundo mundo;

    public override void OnEvent(EventData eventData)
    {
        spriteslider = GetComponent<Spriteslider>();
        mundo = GameObject.FindWithTag("Player").GetComponent<Mundo>();

        string cloneAccount = (string)DictTool.GteValue<byte, object>(eventData.Parameters, (byte)ParameterCode.Account);
        string cloneUserName = (string)DictTool.GteValue<byte, object>(eventData.Parameters, (byte)ParameterCode.Username);
        //Debug.Log(cloneAccount);
        Trigger.SetActive(true);
        spriteslider.NameSetOther(PosCode.R, cloneUserName);
        gameStart = GameObject.FindWithTag("GameStart").GetComponent<GameStart>();
        gameStart.Clone(gameStart.R, gameStart.Rq);
        GameObject.FindWithTag("Player").transform.SetPositionAndRotation(gameStart.L, gameStart.Lq);
        mundo.GameSetup();
    }
}
