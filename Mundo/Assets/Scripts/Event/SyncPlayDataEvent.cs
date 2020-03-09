using Common;
using Common.Tools;
using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SyncPlayDataEvent : BaseEvent
{
    private MundoClone clone;
    private Spriteslider spriteslider;

    public override void OnEvent(EventData eventData)
    {
        try
        {
            clone = GameObject.FindWithTag("OtherPlayer").GetComponent<MundoClone>();
            spriteslider = GameObject.FindWithTag("UI").GetComponent<Spriteslider>();
        }
        catch
        {

        }

        string playerDataString = (string)DictTool.GteValue<byte, object>(eventData.Parameters, (byte)ParameterCode.PlayData);
        PlayerData playerData;

        using (StringReader reader = new StringReader(playerDataString))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
            playerData = (PlayerData)serializer.Deserialize(reader);
        }

        Vector3 pos = new Vector3() { x = playerData.Pos.x, y = playerData.Pos.y, z = playerData.Pos.z };
        Quaternion rot = new Quaternion() { x = playerData.Rot.x, y = playerData.Rot.y, z = playerData.Rot.z, w = playerData.Rot.w };
        short aniSet = playerData.AniSet;
        float hp = playerData.Hp;

        clone.TransformSet(pos, rot, aniSet);
        spriteslider.OtherHpSet(hp);
    }
}
