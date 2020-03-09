using Common;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEvent : BaseEvent
{
    private MundoClone clone;
    private Spriteslider spriteslider;

    public override void OnEvent(EventData eventData)
    {
        clone = GameObject.FindWithTag("OtherPlayer").GetComponent<MundoClone>();
        if (Spriteslider.posCode == PosCode.L)
        {
            Spriteslider.HpR.value = 0;
        }
        else
        {
            Spriteslider.HpL.value = 0;
        }
        clone.Deathanima();
        spriteslider = GameObject.FindWithTag("UI").GetComponent<Spriteslider>();
        spriteslider.Win();
        PhotonEngine.isJoin = false;
    }
}
