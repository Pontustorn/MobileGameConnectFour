using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public Text winText;
    public Text win2Text;
    public TMP_Text player1;
    public TMP_Text player2;
    public GameObject confettiPE;
    public Material opponentMaterial;
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerNames();
        SetPlayerChips();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playerWon()
    {
        winText.enabled = true;
    }

    public void player2Won()
    {
        win2Text.enabled = true;
    }

    public void SetPlayerNames()
    {
        if (GameManager.matchInfo.players[0].username == PlayerData.data.username)
        {
            player1.text = GameManager.matchInfo.players[0].username;
            player2.text = GameManager.matchInfo.players[1].username;
        }
        
        else
        {
            player1.text = GameManager.matchInfo.players[1].username;
            player2.text = GameManager.matchInfo.players[0].username;
        }
        
    }

    public void SetPlayerChips()
    {
        if (GameManager.matchInfo.players[0].username == PlayerData.data.username)
        {
            opponentMaterial.color = new Color(GameManager.matchInfo.players[1].colorR, GameManager.matchInfo.players[1].colorG, GameManager.matchInfo.players[1].colorB);
        }
        else
        {
            opponentMaterial.color = new Color(GameManager.matchInfo.players[0].colorR, GameManager.matchInfo.players[1].colorG, GameManager.matchInfo.players[0].colorB);
        }
    }


}
