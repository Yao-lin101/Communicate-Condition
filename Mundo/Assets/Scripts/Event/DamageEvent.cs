using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEvent : BaseEvent
{
    private Spriteslider spriteslider;

    public override void OnEvent(EventData eventData)
    {
        spriteslider = GameObject.FindWithTag("UI").GetComponent<Spriteslider>();
        spriteslider.GetDamage();
    }
}
