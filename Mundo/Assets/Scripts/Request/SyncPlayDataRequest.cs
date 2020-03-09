using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
using System.Xml.Serialization;

public class SyncPlayDataRequest : Request
{
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Quaternion quaternion;
    [HideInInspector]
    public short aniSet;

    public override void DefaltRequest()
    {
        PlayerData playerData = new PlayerData();
        playerData.Pos = new Vector3Data() { x = position.x, y = position.y, z = position.z };
        playerData.Rot = new QuaternionData() { x = quaternion.x, y = quaternion.y, z = quaternion.z, w = quaternion.w };
        playerData.Hp = Character.Hp;
        playerData.AniSet = aniSet;

        StringWriter sw = new StringWriter();
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(sw, playerData);
        sw.Close();
        string playerDataString = sw.ToString();

        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.PlayData, playerDataString);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {

    }
}
