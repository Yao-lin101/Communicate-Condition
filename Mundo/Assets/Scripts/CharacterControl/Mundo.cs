using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Common;

public class Mundo : Character
{
    private CharacterController cc;
    private Transform postion;
    public static float time = 3.96f;
    private Spriteslider spriteslider;
    private short aniSet;

    private Animator ani;
    private AudioSource source;
    public AudioClip QSound;                    //指定Q技能的音效
    public AudioClip DeathSound;             //指定死亡的音效
    public AudioClip DamageSound;             //指定的音效

    public GameObject knife;                    //获取技能模型预制体    
    
    private ETCJoystick stick;
    private Image m_image;                     
    public ETCJoystick mainStick;
    private GameObject easyTouch;

    private SyncPlayDataRequest syncCharacter;
    private SyncPlayerRequest syncPlayerRequest;
    private GameOverRequest overRequest;

    void Start()
    {
        aniSpeed = 1;
        Spriteslider.mundo = this;
        ETCJoystick.anima = GameObject.FindWithTag("Player").GetComponent<Mundo>();
        syncPlayerRequest = GameObject.FindGameObjectWithTag("UI").GetComponent<SyncPlayerRequest>();
        easyTouch = GameObject.FindWithTag("EasyTouch");
        syncPlayerRequest.DefaltRequest();
        spriteslider = GameObject.FindWithTag("UI").GetComponent<Spriteslider>();
        overRequest = GameObject.FindWithTag("UI").GetComponent<GameOverRequest>();
        //将this Object 上面的Component赋值给定义的AudioSource
        source = GetComponent<AudioSource>();
        source.volume = Setting.Sound;
        cc = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
        postion = GetComponent<Transform>();
        stick = GameObject.FindWithTag("Q Joystick").GetComponent<ETCJoystick>();
        mainStick = GameObject.FindWithTag("Main Joystick").GetComponent<ETCJoystick>();
        m_image = GameObject.FindWithTag("QMask").GetComponent<Image>();
        syncCharacter = GetComponent<SyncPlayDataRequest>();
        InvokeRepeating("SyncPlayData", 0, 0.01f);
        GameSetup();
    }
    #region  Skill
    public override void Qanima()//播放技能动画以及技能效果
    {
        qJoyStick = true;
        //失效右摇杆使技能进入CD
        stick.activated = false;
        //设置CD中的UI图像遮罩
        InvokeRepeating("ImageSet", 0, 0.01f);
        //CD后重启右摇杆
        Invoke("StickReset",time);
        spriteslider.QHpexpend();
        if (Hp > 0)
        {
            ani.SetInteger("stat", Q);
            aniSet = Q;
            //控制角色攻击转向
            cc.transform.rotation = Quaternion.Euler(0, 315 - Angle, 0);
        }
    }

    public void CreateKnife()
    {
        GameObject.Instantiate(knife, postion.position, postion.rotation);
    }
    public void StickReset()
    {
        stick.activated = true;
    }
    public void ImageSet()
    {
        if (time > 0)
        {
            m_image.fillAmount = time / 3.96f;
            time -= 0.01f;
        }
        else 
        {
            time = 3.96f;
            CancelInvoke("ImageSet");
        }
    }

    public void GameSetup()
    {
        Hp = 598;
        stick.activated = false;
        InvokeRepeating("ImageSet", 0, 0.01f);
        Invoke("StickReset", time);
    }
    #endregion
    #region Animation Settings

    public override void Idleanima()    //待机动画
    {
        ani.SetInteger("stat", Idle);
        aniSet = Idle;
    }

    public override void Runanima()    //移动动画
    {
        ani.SetInteger("stat", Run);
        ani.speed = aniSpeed;
        aniSet = Run;
    }
    
    public void Resetanima()//使用技能后重置动画
    {
        qJoyStick = false;
        if (cc.isGrounded && ETCInput.GetAxis("Vertical") == 0 && ETCInput.GetAxis("Horizontal") == 0)
        {
            ani.SetInteger("stat", Idle);
            aniSet = Idle;
        }
        if (cc.isGrounded && (ETCInput.GetAxis("Vertical") != 0) || (ETCInput.GetAxis("Horizontal") != 0))
        {
            ani.SetInteger("stat", Run);
            aniSet = Run;
        }
    }

    public override void Deathanima()//播放死亡动画
    {
        ani.SetInteger("stat", Death);
        aniSet = Death;
    }
    #endregion
    #region Skill sound Settings
    public void Doqsounds()//播放技能音效
    {
        source.PlayOneShot(QSound, 1F);
    }
    public void Dodeathsounds()//播放死亡音效
    {
        source.PlayOneShot(DeathSound, 1F);
    }
    public void DoDamageSound()//播放死亡音效
    {
        source.PlayOneShot(DamageSound, 1F);
    }
    #endregion
    #region Sync
    /// <summary>
    /// 同步位置
    /// </summary>
    void SyncPlayData()
    {
        syncCharacter.position =transform.position;
        syncCharacter.quaternion = transform.rotation;
        syncCharacter.aniSet = aniSet;
        syncCharacter.DefaltRequest();
        if (Hp <= 0)
        {
            easyTouch.SetActive(false);
            CancelInvoke("SyncPlayData");
            PhotonEngine.isJoin = false;
            overRequest.DefaltRequest();
            spriteslider.Defeated();
        }
    }
    #endregion

    private void FixedUpdate()
    {
        if (Hp > 0)
        {
            //记录技能摇杆松开前的角度并保存
            if (cc.isGrounded && ETCInput.GetAxis("QVertical") != 0 || ETCInput.GetAxis("QHorizontal") != 0)
            {
                Angle = Mathf.Atan2(ETCInput.GetAxis("QVertical"), ETCInput.GetAxis("QHorizontal")) * Mathf.Rad2Deg;
            }
        }
        else { Deathanima(); }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "trigger1" && Hp > 0 && Spriteslider.posCode == PosCode.L)
        {
            Hp--;
            Spriteslider.HpL.value = Hp;
        }
        else if (other.gameObject.name == "trigger2" && Hp > 0 && Spriteslider.posCode == PosCode.R)
        {
            Spriteslider.HpR.value = Hp;
            Hp--;
        }
    }
}
