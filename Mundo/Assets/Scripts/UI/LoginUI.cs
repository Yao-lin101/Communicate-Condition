using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FairyGUI;
using Common;

public class LoginUI : MonoBehaviour
{
    private GComponent mainUI;
    private GComponent registerFrame;
    private GComponent login;
    private LoginRequest loginRequest;
    private RegisterRequest registerRequest;

    public string account = null;
    public string password = null;
    private string again;

    void Start()
    {
        registerRequest = GetComponent<RegisterRequest>();
        loginRequest = GetComponent<LoginRequest>();
        mainUI = GetComponent<UIPanel>().ui;
        login = mainUI.GetChild("Login").asCom;
        registerFrame = UIPackage.CreateObject("LoginUI", "Registerframe").asCom;
        mainUI.GetChild("addRegister").onClick.Add(() =>{ AddRegisterFrame(); });
        registerFrame.GetChild("closeButton").onClick.Add(() => { CloseButton(); });
        registerFrame.GetChild("OkButton").onClick.Add(() => { RemoveRegisterFrame(); });
        mainUI.GetChild("GoButton").onClick.Add(() => { Login(); });
    }

    /// <summary>
    /// 创建注册窗口
    /// </summary>
    void AddRegisterFrame()
    {
        //禁用登陆UI
        registerFrame.GetChild("account").visible = true;
        mainUI.GetChild("Login").alpha = 0.6f;
        mainUI.GetChild("Login").touchable = false;
        mainUI.GetChild("addRegister").visible = false;
        mainUI.GetChild("GoButton").visible = false;
        login.GetChild("account").text = null;
        login.GetChild("password").text = null;
        //创建注册UI
        mainUI.AddChild(registerFrame);
        registerFrame.SetPosition(340, 100, 0);
        Transition t = registerFrame.GetTransition("t1");
        t.Play();
    }

    /// <summary>
    /// 关闭注册窗口
    /// </summary>
    void CloseButton()
    {
        Transition t = registerFrame.GetTransition("t3");
        t.Play(()=>
        {
            mainUI.RemoveChild(registerFrame);
        });
        mainUI.GetChild("Login").alpha = 1;
        mainUI.GetChild("Login").touchable = true;
        registerFrame.GetChild("account").visible = false;
        registerFrame.GetChild("password").text = null;
        registerFrame.GetChild("again").text = null;
        mainUI.GetChild("GoButton").visible = true;
        mainUI.GetChild("addRegister").visible = true ;

    }

    /// <summary>
    /// 登陆游戏
    /// </summary>
    void Login()
    {
        account = login.GetChild("account").text;
        password = login.GetChild("password").text;
        loginRequest.DefaltRequest();
    }

    /// <summary>
    /// 注册数据
    /// </summary>
    void RemoveRegisterFrame()
    {
        account = registerFrame.GetChild("account").text;
        password = registerFrame.GetChild("password").text;
        again = registerFrame.GetChild("again").text;
        if (again == password && again != "" && password != "" && account != "")
        {
            registerRequest.DefaltRequest();
        }
        else
        {
            Transition t = registerFrame.GetTransition("t2");
            t.Play();
        }
    }
    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Transition t = mainUI.GetTransition("t1");
            t.Play();
        }
    }
    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            login.GetChild("account").text = account;
            login.GetChild("password").text = password;
            CloseButton();
        }
        else
        {
            Transition t = mainUI.GetTransition("t1");
            t.Play();
        }
    }
}
