using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
using System.Xml.Serialization;

public class RoomCreateRequest : Request
{
    [HideInInspector]
    public string roomName;
    private RoomData roomData;

    public override void DefaltRequest()
    {
        roomData = new RoomData() { Account = PhotonEngine.account, Contain = 1, UserName = PhotonEngine.userName, RoomName = roomName };

        StringWriter sw = new StringWriter();
        XmlSerializer serializer = new XmlSerializer(typeof(RoomData));
        serializer.Serialize(sw, roomData);
        sw.Close();
        string roomDataString = sw.ToString();

        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.RoomData, roomDataString);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
    }
}
