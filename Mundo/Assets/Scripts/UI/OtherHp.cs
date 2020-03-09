using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherHp : MonoBehaviour
{
    private Text text;
    private int hpint;
    private Image m_image;
    private Text userName;
    void Start()
    {
        m_image = GameObject.Find("hpClone").GetComponent<Image>();
        text = GameObject.Find("hpvalueClone").GetComponent<Text>();
        userName = text.transform.Find("NameClone").GetComponent<Text>();
    }
    public void NameSet(string name)
    {
        userName.text = name;
    }

    public void SetHp(float Hp)
    {
        hpint = (int)Hp;
        m_image.fillAmount = Hp / 598;
        text.text = "598/" + hpint;
    }
}
