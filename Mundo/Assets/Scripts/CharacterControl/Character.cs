using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public const int Q = 2;//技能动画值
    public const int Idle = 0;//待机动画值
    public const int Run = 1;//移动动画值
    public const int Death = 3;//死亡动画值
    static public bool qJoyStick = false;//控制左摇杆生效
    public float Angle { get; set; }//存储技能释放的角度
    public static float Hp;
    public static float Speed { get; set; }
    public static float aniSpeed { get; set; }
    public string Heroname { get; set; } 
    public virtual void Idleanima() { }//待机动画
    public virtual void Qanima() { }//技能动画
    public virtual void Runanima() { }//移动动画
    public virtual void Deathanima() { }//死亡动画
}
