using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text winText;
    public Text win2Text;
    public GameObject confettiPE;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
