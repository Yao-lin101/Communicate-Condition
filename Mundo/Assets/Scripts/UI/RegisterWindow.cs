using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RegisterWindow :Window
{
    private GComponent ani;
    private LoginUI LoginUI;
    protected override void OnInit()
    {
        this.contentPane = UIPackage.CreateObject("LoginUI", "RegisterWindow").asCom;
        ani = this.contentPane.GetChild("frame").asCom;
    }
    protected override void DoShowAnimation()
    {
        Transition t = ani.GetTransition("t1");
        t.Play();
    }
    protected override void OnHide()
    {
        
    }
}
