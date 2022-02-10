using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo
{
    public string username;
    public int wins;
    public int losses;
    public int chipsPlaced;
}

public class ChipInfo
{
    public float colorR;
    public float colorG;
    public float colorB; 
}
