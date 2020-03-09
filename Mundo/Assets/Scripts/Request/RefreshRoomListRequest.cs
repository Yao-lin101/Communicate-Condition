using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Common.Tools;
using System.Xml.Serialization;
using System.IO;

public class RefreshRoomListRequest : Request
{
    public override void DefaltRequest()
    {
        PhotonEngine.Peer.OpCustom((byte)OpCode, null, true);
    }

    public override void DefaltRequest(string select)
    {

    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        List<RoomData> roomDatas;

        string roomDataString = (string)DictTool.GteValue<byte, object>(operationResponse.Parameters, (byte)ParameterCode.RoomDataList);

        using (StringReader reader = new StringReader(roomDataString))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<RoomData>));
            roomDatas = (List<RoomData>)serializer.Deserialize(reader);
        }

        //Debug.Log(roomDataString);
        lobby.SyncRoom(roomDatas);
        roomDatas.Clear();
    }
}
