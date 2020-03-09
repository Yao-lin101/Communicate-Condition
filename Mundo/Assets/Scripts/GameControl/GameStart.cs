using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject player;
    public GameObject clone;

    [HideInInspector]
    public Vector3 L = new Vector3(0.43f, 0.15f, -3.89f);
    [HideInInspector]
    public Quaternion Lq = new Quaternion(0, -40f, 0,80);
    [HideInInspector]
    public Vector3 R = new Vector3(-4.43f, 0.15f, -0.32f);
    [HideInInspector]
    public Quaternion Rq = new Quaternion(0, -137.608f, 0, -28);

    public void Self(Vector3 Pos ,Quaternion rotation)
    {
        GameObject.Instantiate(player, Pos, rotation);
    }


    public void Clone(Vector3 Pos, Quaternion rotation)
    {
        GameObject.Instantiate(clone, Pos, rotation);
    }
}
