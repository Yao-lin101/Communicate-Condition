using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using HedgehogTeam.EasyTouch;
using System;
using Common;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private GameObject zed = null;
    [SerializeField]
    private AudioSource audioSource = null;
    private GameObject Zedclone;
    private Animator[] ani;

    private float Volume;

    private GComponent mainUI;
    private GComponent list;
    private GComponent list_sub;
    private GComponent Alert;
    private GList sub;
    private GComponent roomList;
    private RoomCreateRequest roomCreateRequest;
    private RefreshRoomListRequest refreshRoomListRequest;
    private UserNameRequest userNameRequest;
    private UserNameSetRequest userNameSet;
    private SyncPlayerRequest syncPlayerRequest;

    private bool change = true;
    private bool isRoom = false;

    Gesture gesture;
    private float start_x;
    private float start_y;
    private float end_x;
    private float end_y;
    private bool haveList = false;

    [HideInInspector]
    public string userName = null;

    void Start()
    {
        SoundSetup();
        userNameRequest = GetComponent<UserNameRequest>();
        userNameRequest.DefaltRequest();
        userNameSet = GetComponent<UserNameSetRequest>();
        roomCreateRequest = GetComponent<RoomCreateRequest>();
        refreshRoomListRequest = GetComponent<RefreshRoomListRequest>();
        syncPlayerRequest = GetComponent<SyncPlayerRequest>();

        mainUI = GetComponent<UIPanel>().ui;
        list = UIPackage.CreateObject("Lobby", "List").asCom;
        list_sub = UIPackage.CreateObject("Lobby", "List_sub").asCom;
        sub = list_sub.GetChild("n0").asList;
        Alert = UIPackage.CreateObject("Lobby", "Alert").asCom;
        roomList = UIPackage.CreateObject("Lobby", "RoomList").asCom;
        //监听按钮
        mainUI.GetChild("mainButton").onClick.Set(() =>
        {
            mainUI.GetChild("mainButton").visible = false;
            OpenList();
        });
        mainUI.GetChild("n3").onClick.Set(() => {CloseList();});

    }
    private void Update()
    {
        gesture = EasyTouch.current;
        if (!isRoom)
        {
            if (gesture != null && !haveList) 
            {
                if (EasyTouch.EvtType.On_TouchStart == gesture.type)
                {
                    start_x = gesture.startPosition.x;
                    start_y = gesture.startPosition.y;
                }
                if (EasyTouch.EvtType.On_TouchUp == gesture.type)
                {
                    end_x = gesture.position.x;
                    end_y = gesture.position.y;
                    if (start_y - end_y > 200 && Mathf.Abs(start_x - end_x) < 50)
                    {
                        mainUI.GetChild("mainButton").visible = false;
                        start_y = 0;
                        end_y = 0;
                        OpenList();
                    }
                }
            }
        }
    }
    #region List Control
    /// <summary>
    /// 重置“？”可触摸
    /// </summary>
    private void ResetButton()
    {
        list.GetChild("n0").asCom.GetChildAt(2).touchable = true;
        list.GetChild("n0").asCom.GetChildAt(0).touchable = true;
    }
    /// <summary>
    /// 关闭列表
    /// </summary>
    private void CloseList()
    {
        if (haveList)
        {
            haveList = false;
            //Transition t1 = list.GetTransition("t1");
            list.GetTransition("t1").Play(() =>
            {
                mainUI.GetChild("mainButton").visible = true;
                if (list.numChildren > 5)
                {
                    for (int i = 5; i <= list.numChildren; i++)
                    {
                        list.GetChildAt(i).asCom.GetChildAt(0).asList.GetChildAt(0).asButton.selected = false;
                        list.GetChildAt(i).asCom.GetChildAt(0).asList.GetChildAt(1).asButton.selected = false;
                        list.RemoveChildAt(i);
                    }
                }
                mainUI.RemoveChild(list);
            });
        }
    }
    /// <summary>
    /// 打开列表
    /// </summary>
    private void OpenList()
    {
        haveList = true;
        mainUI.AddChild(list);
        if ((float)Screen.width / 39 >= (float)Screen.height / 18.0f) 
        {
            list.SetPosition(1280 * 0.8f, 720 * 0.4f, 0);
        }
        else if((float)Screen.width / 2 >= Screen.height)
        {
            list.SetPosition(1280 * 0.75f, 720 * 0.4f, 0);
        }
        else
        {
            list.SetPosition(1280 * 0.7f, 720 * 0.4f, 0);
        }
        //mainUI.GetChild("Screen").text = list.position.x +"/"+Screen.width+ "\n" + list.position.y+"/"+Screen.height;
        list.touchable = false;
        Transition t = list.GetTransition("t0");
        t.Play(()=> { list.touchable = true; });
        list.GetChild("n0").asCom.GetChildAt(0).onClick.Set(() =>
        {
            CloseList();
            Room();
            //SceneManager.LoadScene("Talk"); 
        });
        list.GetChild("n0").asCom.GetChildAt(1).onClick.Set(CloseList);
        list.GetChild("n0").asCom.GetChildAt(2).onClick.Set(() =>
        {
            if (change)
            {
                CloseList();
                mainUI.GetTransition("t0").Play();
                Zedclone = GameObject.Instantiate(zed, new Vector3(0.36f, 1.390813f, 1.14685f), Quaternion.identity);
                ani = GameObject.FindWithTag("Player").GetComponentsInChildren<Animator>();
                Invoke("ChangeAnima", 5.1f);
                audioSource.Play();
                list.GetChild("n0").asCom.GetChildAt(2).touchable = false;
                list.GetChild("n0").asCom.GetChildAt(0).touchable = false;
                Invoke("ResetButton", 6f);
            }
            else
            {
                CloseList();
                ChangeAnima();
            }
        });
        list.GetChild("n0").asCom.GetChildAt(3).onClick.Set(SubList);
    }
    /// <summary>
    /// 实现“？”
    /// </summary>
    private void ChangeAnima()
    {
        if (change)
        {
            for(int i=0; i< ani.Length; i++)
            {
                ani[i].SetBool("Start", true);
            }
            change = false;
        }
        else
        {
            if(Zedclone != null)
            {
                GameObject.Destroy(Zedclone);
                audioSource.GetComponent<AudioSource>().Stop();
                mainUI.GetChild("n8").alpha = 1;
                change = true;
                ani = null;
            }
        }
    }
    /// <summary>
    /// 实现设置列表
    /// </summary>
    private void SubList()
    {
        if (list.numChildren > 6)
        {
            list.RemoveChildAt(7);
        }
        list.AddChild(list_sub);
        list_sub.AddRelation(list.GetChild("n14"), RelationType.Top_Top);
        list_sub.AddRelation(list.GetChild("n14"), RelationType.Left_Left);
        Transition t = list_sub.GetTransition("t0");
        t.Play();
        list_sub.SetPosition(list.GetChild("n0").asCom.GetChildAt(3).position.x+120, 92, 0);
        sub.numItems = 2;
        sub.GetChildAt(0).icon = UIPackage.GetItemURL("Lobby", "Sethover1");
        sub.GetChildAt(0).text = "Volume";
        sub.GetChildAt(1).icon= UIPackage.GetItemURL("Lobby", "Sethover2");
        sub.GetChildAt(1).text = "Logout";
        //监听列表中的按钮
        sub.GetChildAt(0).onClick.Add(VolumeAlert);
        sub.GetChildAt(1).onClick.Add(Logout);
    }
    /// <summary>
    /// 实现音效和BGM的音量设置
    /// </summary>
    private void VolumeAlert()
    {
        list.touchable = false;
        list_sub.touchable = false;
        Transition t = Alert.GetTransition("t0");
        t.Play();
        mainUI.AddChild(Alert);
        Alert.GetChildAt(1).visible = false;
        Alert.GetChildAt(2).asLoader.url = "ui://se5xss9vi9wvvd";
        CloseAlter();
        Alert.GetChildAt(3).onClick.Set(()=>
        {
            Setting.Sound = Convert.ToSingle(Alert.GetChild("n2").asLoader.component.GetChildAt(0).asCom.GetChild("title").text)/100;
            Setting.BGM = Convert.ToSingle(Alert.GetChild("n2").asLoader.component.GetChildAt(1).asCom.GetChild("title").text)/100;
            SoundSetup();
            AlertClose();
        });
    }
    /// <summary>
    /// 关闭Alert
    /// </summary>
    private void AlertClose()
    {
        Alert.GetChildAt(1).visible = false;
        Transition t = Alert.GetTransition("t1");
        t.Play(() =>
        {
                mainUI.RemoveChild(Alert);
                roomList.RemoveChild(Alert);
        });
        list.touchable = true;
        list_sub.touchable = true;
    }
    /// <summary>
    /// 实现退出游戏
    /// </summary>
    private void Logout()
    {
        list.touchable = false;
        list_sub.touchable = false;
        mainUI.AddChild(Alert);
        Transition t = Alert.GetTransition("t0");
        t.SetValue("endsize", 700, 500);
        t.Play(() => { Alert.GetChildAt(1).visible = true; });
        t.SetValue("endsize", 700, 620);
        Alert.GetChildAt(2).visible = false;
        CloseAlter();
        Alert.GetChildAt(3).onClick.Set(() =>
        {
            try
            {
                audioSource.Stop();
            }
            catch
            {

            }
            AlertClose();
            Application.Quit();
        });
    }
    /// <summary>
    /// 进入游戏房间列表
    /// </summary>
    private void Room()
    {
        //向服务器发起获取房间列表的请求
        refreshRoomListRequest.DefaltRequest();
    }
    /// <summary>
    /// 同步房间列表
    /// </summary>
    public void SyncRoom(List<RoomData> syncRoomDataList)
    {
        isRoom = true;
        int listnum = syncRoomDataList.Count;
        string select=PhotonEngine.account;

        if (Zedclone != null)
        {
            GameObject.Destroy(Zedclone);
            audioSource.GetComponent<AudioSource>().Stop();
            mainUI.GetChild("n8").alpha = 1;
            change = true;
            ani = null;
        }
        mainUI.AddChild(roomList);
        roomList.GetChild("Back").asButton.title = "Back";
        roomList.GetChild("Create").asButton.title = "Create";

        GList list = roomList.GetChild("List").asList;
        list.numItems = listnum;
        for (int i = 0; i < listnum; i++)
        {
            list.GetChildAt(i).asButton.GetChild("title").text = syncRoomDataList[i].RoomName;
            list.GetChildAt(i).asButton.GetChild("uesr").text = syncRoomDataList[i].UserName;
            list.GetChildAt(i).asButton.GetChild("n6").text = syncRoomDataList[i].Contain + "/2";
            list.GetChildAt(i).asButton.GetChild("account").text = syncRoomDataList[i].Account;
        }

        for (int i = 0; i < listnum; i++)
        {
            GButton button = list.GetChildAt(i).asButton;
            button.onClick.Add(() =>
            {
                select = button.GetChild("account").text;
                PhotonEngine.selectName = button.GetChild("uesr").text;
            });
        }

        roomList.GetChild("Back").onClick.Set(() => { mainUI.RemoveChild(roomList); isRoom = false; });
        roomList.GetChild("Create").onClick.Set(() =>
        {
            roomList.AddChild(Alert);
            Alert.SetPosition(290, 50, 0);
            Transition t = Alert.GetTransition("t0");
            t.Play();
            Alert.GetChildAt(1).visible = false;
            Alert.GetChild("n2").asLoader.url = "ui://se5xss9vjnr4vy";
            Alert.GetChildAt(3).onClick.Set(() =>
            {
                syncPlayerRequest.DefaltRequest(select);
                SceneManager.LoadScene("Talk");
                roomCreateRequest.roomName = Alert.GetChild("n2").asLoader.component.GetChild("title").text; AlertClose();
                roomCreateRequest.DefaltRequest();
            });
            CloseAlter();
        });
        roomList.GetChild("Join").onClick.Set(() => {
            if (select != PhotonEngine.account)
            {
                syncPlayerRequest.DefaltRequest(select);
            }
        });
    }
    /// <summary>
    /// 同步用户名
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnNameResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Failed)
        {
            mainUI.AddChild(Alert);
            Transition t = Alert.GetTransition("t0");
            t.SetValue("endsize", 700, 500);
            t.Play();
            t.SetValue("endsize", 700, 620);
            Alert.GetChild("n2").asLoader.url = "ui://se5xss9vui8vvz";
            CloseAlter();
            Alert.GetChildAt(3).onClick.Set(() =>
            {
                userName = Alert.GetChild("n2").asLoader.component.GetChildAt(2).text;
                userNameSet.DefaltRequest();
                AlertClose();
            });
        }
    }

    public void OnJoinFailedResponse()
    {
        GComponent join = UIPackage.CreateObject("Lobby", "JoinFailed").asCom;
        roomList.AddChild(join);
        join.position = new Vector2(515,235);
        Transition t = join.GetTransition("Open");
        t.Play();
        join.onClick.Set(() =>
        {
            join.touchable = false;
            Transition t1 = join.GetTransition("Close");
            t1.Play(() =>
            {
                roomList.RemoveChild(join);
                join.touchable = true;
            });
        });
    }

    #endregion
    #region Shaer Button
    /// <summary>
    /// 监听Alert的关闭按钮
    /// </summary>
    private void CloseAlter()
    {
        Alert.GetChildAt(4).onClick.Set(AlertClose);
    }
    #endregion

    private void SoundSetup()
    {
        audioSource.volume = Setting.BGM;
        GRoot.inst.soundVolume = Setting.Sound;
    }
}
