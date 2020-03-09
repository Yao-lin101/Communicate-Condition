using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Common;
using UnityEngine.SceneManagement;

public class Spriteslider : Character
{
    private GComponent mainUI;
    public static GProgressBar HpR;
    public static GProgressBar HpL;
    public static PosCode posCode;
    private GameStart gameStart;
    [SerializeField]
    private GameObject Trigger = null;
    private GComponent Alert;
    private GButton outButton;
    private GameOverRequest overRequest;
    private GameObject easyTouch;
    public static Mundo mundo;

    void Start()
    {
        Hp = 598;
        GRoot.inst.soundVolume = Setting.Sound;
        easyTouch = GameObject.FindWithTag("EasyTouch");
        gameStart = GameObject.FindWithTag("GameStart").GetComponent<GameStart>();
        overRequest = GetComponent<GameOverRequest>();
        mainUI = GetComponent<UIPanel>().ui;
        HpL = mainUI.GetChild("HpL").asProgress;
        HpR = mainUI.GetChild("HpR").asProgress;
        Alert = UIPackage.CreateObject("Lobby", "Alert").asCom;
        outButton = mainUI.GetChild("out").asButton;

        if (PhotonEngine.isJoin)
        {
            Setup();
        }

        outButton.onClick.Set(() =>
        {
            mainUI.AddChild(Alert);
            Alert.SetPosition(290, 50, 0);
            Transition t = Alert.GetTransition("t0");
            t.SetValue("endsize", 700, 500);
            t.Play(() => { Alert.GetChildAt(1).visible = true; });
            t.SetValue("endsize", 700, 620);
            Alert.GetChildAt(2).visible = false;
            Alert.GetChildAt(3).onClick.Set(() =>
            {
                overRequest.DefaltRequest();
                Transition t1 = Alert.GetTransition("t1");
                t1.Play(() =>
                {
                    SceneManager.LoadScene("Lobby");
                });
                Alert.GetChild("title").visible = false;
            });
            Alert.GetChildAt(4).onClick.Set(() =>
            {
                Transition t1 = Alert.GetTransition("t1");
                t1.Play(() => { mainUI.RemoveChild(Alert); });
                Alert.GetChild("title").visible = false;
            });
        });
    }
    /// <summary>
    /// 设置自己的名字
    /// </summary>
    /// <param name="pos"></param>
    public void NameSetSelf()
    {
        if (posCode == PosCode.L)
        {
            HpL.GetChild("UserName").text = PhotonEngine.userName;
            InvokeRepeating("HpRestoreL", 0, 1);
        }
        else
        {
            HpR.GetChild("UserName").text = PhotonEngine.userName;
            InvokeRepeating("HpRestoreR", 0, 1);
        }
    }

/// <summary>
/// 设置连接进来的客户端的名字
/// </summary>
/// <param name="pos"></param>
/// <param name="name"></param>
    public void NameSetOther(PosCode pos, string name)
    {
        if (pos == PosCode.L)
        {
            HpL.GetChild("UserName").text = name;
        }
        else
        {
            HpR.GetChild("UserName").text = name;
        }
    }

    public void HpRestoreL()
    {
        HpRestore();
        HpL.value = Hp;
    }

    public void HpRestoreR()
    {
        HpRestore();
        HpR.value = Hp;
    }

    public void QHpexpend()
    {
        if (Hp > 50)
        {
            Hp -= 50;
        }
        else if (Hp <= 50 && Hp >= 0)
        {
            Hp = 1;
        }
        if (posCode == PosCode.L)
        {
            HpL.value = Hp;
        }
        else
        {
            HpR.value = Hp;
        }
    }

    /// <summary>
    /// Hp自动恢复
    /// </summary>
    public void HpRestore()
    {
        if (Hp > 0)
        {
            if (Hp < 598)
            {
                Hp += 3.4f;
            }
            else Hp = 598;
        }
    }

    public void OtherHpSet(float hp)
    {
        if (posCode == PosCode.L)
        {
            HpR.value = hp;
        }
        else
        {
            HpL.value = hp;
        }
    }

    public void Defeated()
    {
        outButton.visible = false;
        Transition t = mainUI.GetTransition("Death");
        t.Play(() =>
        {
            Transition t1 = mainUI.GetTransition("DeathT");
            mainUI.onClick.Set(() =>
            {
                t1.Play();
                SceneManager.LoadScene("Lobby");
            });
        });
    }

    public void Win()
    {
        easyTouch.SetActive(false);
        outButton.visible = false;
        Transition t = mainUI.GetTransition("Win");
        t.Play(() =>
        {
            Transition t1 = mainUI.GetTransition("WinT");
            mainUI.onClick.Set(() =>
            {
                t1.Play();
                SceneManager.LoadScene("Lobby");
            });
        });
    }

    private void Setup()
    {
        Spriteslider.posCode = PosCode.R;
        gameStart.Clone(gameStart.L, gameStart.Lq);
        gameStart.Self(gameStart.R, gameStart.Rq);
        NameSetSelf();
        NameSetOther(PosCode.L, PhotonEngine.selectName);
        Trigger.SetActive(true);
    }

    private void SpeedReset()
    {
        mundo.mainStick.tmSpeed = Speed;
        aniSpeed = 1;
    }

    public void GetDamage()
    {
        aniSpeed *= 0.6f;
        mundo.mainStick.tmSpeed *= 0.6f;
        Invoke("SpeedReset", 6);
        if (0.2f * Hp >= 80)
            Hp -= 0.2f * Hp;
        else
            Hp -= 80;
        if (posCode == PosCode.L)
            HpL.value = Hp;
        else
            HpR.value = Hp;
    }

    public void FixedUpdate()
    {
        if (Hp <= 0) 
        {
            CancelInvoke();
        }
    }
}
