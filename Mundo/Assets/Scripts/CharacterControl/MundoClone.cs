using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MundoClone : MonoBehaviour
{
    private Transform position;

    private Animator ani;
    private AudioSource source;
    public AudioClip QSound;                    //指定Q技能的音效
    public AudioClip DeathSound;             //指定死亡的音效

    public GameObject knife;                    //获取技能模型预制体    

    void Start()
    {
        source = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();
        position = GetComponent<Transform>();
        source.volume = Setting.Sound;
    }

    public void TransformSet(Vector3 pos, Quaternion rot, short aniSet)
    {
        position.position = pos;
        position.rotation = rot;
        switch (aniSet)
        {
            case 0:
                Idleanima();
                break;
            case 1:
                Runanima();
                break;
            case 2:
                Qanima();
                break;
            case 3:
                Deathanima();
                break;
        }
    }

    #region  Skill
    public void Qanima()//播放技能动画以及技能效果
    {
        ani.SetInteger("stat", 2);
    }
    public void CreateKnife()
    {
        GameObject.Instantiate(knife, position.position, position.rotation);
        //Debug.Log("刀来？");
    }
    #endregion
    #region Animation Settings

    public void Idleanima()    //待机动画
    {
        ani.SetInteger("stat", 0);
    }

    public void Runanima()    //移动动画
    {
        ani.SetInteger("stat", 1);
    }


    public void Deathanima()//播放死亡动画
    {
        ani.SetInteger("stat", 3);

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

    public void Resetanima()//防止动画控制器报错（事件丢失组件）
    {

    }
    #endregion

}
