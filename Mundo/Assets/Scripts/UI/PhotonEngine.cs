using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Common;
using Common.Tools;

public class PhotonEngine : MonoBehaviour,IPhotonPeerListener
{
    public static PhotonPeer Peer
    {
        get
        {
            return peer;
        }
    }
    public static PhotonEngine Instance;
    static PhotonPeer peer;

    private Dictionary<OperationCode, Request> RequestDict = new Dictionary<OperationCode, Request>();
    private Dictionary<EventCode, BaseEvent> EventDict = new Dictionary<EventCode, BaseEvent>();
    public static string account;
    public static string userName;
    public static string selectName;
    public static bool isJoin = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //通过Listender接收服务器端的响应
        peer = new PhotonPeer(this,ConnectionProtocol.Udp);
        peer.Connect("127.0.0.1:5055","CommunicateCondition");
    }

    // Update is called once per frame
    void Update()
    {
        peer.Service();
    }

    void OnDestroy()
    {
        if (peer != null && peer.PeerState == PeerStateValue.Connected)
        {
            peer.Disconnect();
        }  
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnEvent(EventData eventData)
    {
        EventCode code = (EventCode)eventData.Code;
        BaseEvent baseEvent = DictTool.GteValue<EventCode, BaseEvent>(EventDict, code);
        try
        {
            baseEvent.OnEvent(eventData);
        }
        catch
        {

        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        OperationCode opCode = (OperationCode)operationResponse.OperationCode;
        Request request = null;
        bool temp = RequestDict.TryGetValue(opCode, out request);
        if (temp)
        {
            request.OnOperationResponse(operationResponse);
        }
        else
        {
            Debug.Log("找不到对象");
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        Debug.Log(statusCode);
    }

    public void AddRequest(Request request)
    {
        RequestDict.Add(request.OpCode,request);
    }

    public void RemoveRequest(Request request)
    {
        RequestDict.Remove(request.OpCode);
    }

    public void AddEvent(BaseEvent baseEvent)
    {
        EventDict.Add(baseEvent.EventCode, baseEvent);
    }

    public void RemoveEvent(BaseEvent baseEvent)
    {
        EventDict.Remove(baseEvent.EventCode);
    }
}
