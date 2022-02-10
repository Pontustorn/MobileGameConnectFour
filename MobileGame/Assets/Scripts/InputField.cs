using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public int column;
    public SpawnChipScript sps;

    public void Start()
    {
        sps = GameObject.Find("GameController").GetComponent<SpawnChipScript>();
    }
    void OnMouseDown()
    {
        sps.SelectColumn(column);
    }
}
