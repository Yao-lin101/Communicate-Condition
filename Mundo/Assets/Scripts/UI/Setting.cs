using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    private static float sound = 0.5f;
    private static float bgm = 0.5f;

    [HideInInspector]
    public static float Sound { get { return sound; } set { sound = value; } }
    [HideInInspector]
    public static float BGM { get { return bgm; } set { bgm = value; } }

    public static Setting setting;

    private void Awake()
    {
        if (setting == null)
        {
            setting = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (setting != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

}
