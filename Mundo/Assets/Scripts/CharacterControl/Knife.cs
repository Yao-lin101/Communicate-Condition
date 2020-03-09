using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Character
{
    private Rigidbody knife;
    private Transform m_Transform;
    private DamageRequest damageReqest;
    private Mundo mundo;
    public bool isMainKnife = false;

    void Start()
    {
        mundo = GameObject.FindWithTag("Player").GetComponent<Mundo>();
        damageReqest = GameObject.FindWithTag("UI").GetComponent<DamageRequest>();
        knife = GetComponent<Rigidbody>();
        knife.AddRelativeForce(Vector3.forward * 12, ForceMode.Impulse);//设置发射技能的力
        m_Transform = GetComponent<Transform>();
        InvokeRepeating("Rotation", 0, 0.01f);
        Destroy(gameObject, 0.6f);

    }
    private void Rotation()
    {
        //m_Transform.Translate(Vector3.forward * 0.05f, Space.Self);
        m_Transform.Rotate(Vector3.left, -5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMainKnife) 
        {
            if (other.gameObject.CompareTag("OtherPlayer"))
            {
                Hp += 20;
                damageReqest.DefaltRequest();
                mundo.DoDamageSound();
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                mundo.DoDamageSound();
                Destroy(gameObject);
            }
        }
    }
}
