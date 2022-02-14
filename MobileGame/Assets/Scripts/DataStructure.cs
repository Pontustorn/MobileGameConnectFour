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
    public List<string> activeGames;
    public float colorR;
    public float colorG;
    public float colorB;
    public int image;
}

[System.Serializable]
public class MatchInfo
{
    public string matchid;
    public string displayName;
    public List<UserInfo> players;
    public string player1;
    public string player2;
    public bool full;
    public string playerTurn;
    public List<int> chipPlacements;
}
