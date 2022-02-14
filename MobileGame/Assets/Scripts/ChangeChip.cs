using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeChip : MonoBehaviour
{
    Vector3 newMousePos;

    public Slider sliderRed;
    public Slider sliderGreen;
    public Slider sliderBlue;

    public Material mat;

    public GameObject usedImage;

    SpriteRenderer sr;

    public Sprite[] imageList;

    public int changeCounter = 1;

    UserInfo userInfo;
    

    // Start is called before the first frame update
    void Start()
    {
        userInfo = new UserInfo();
        sr = usedImage.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        newMousePos = Camera.main.WorldToScreenPoint(Input.mousePosition);

        mat.color = new Color(sliderRed.value, sliderGreen.value, sliderBlue.value);
    }

    private void OnMouseDrag()
    {
        transform.rotation = Quaternion.Euler(90, 90, newMousePos.x / 150);
    }

    public void ChangeImageRight()
    {
        if (changeCounter < imageList.Length - 1)
        {
            changeCounter++;
        }

        else
        {
            changeCounter = 0;
        }

        sr.sprite = imageList[changeCounter];
    }

    public void ChangeImageLeft()
    {
        if (changeCounter >= 0)
        {
            changeCounter--;
            if(changeCounter == -1)
            {
                changeCounter = imageList.Length;
            }

        }
        sr.sprite = imageList[changeCounter];
    }
    public void UpdateValues()
    {

        PlayerData.data.colorR = sliderRed.value;
        PlayerData.data.colorG = sliderGreen.value;
        PlayerData.data.colorB = sliderBlue.value;

        PlayerData.SaveData();
    }

    public void ChipDataLoaded()
    {
        sliderRed.value = PlayerData.data.colorR;
        sliderGreen.value = PlayerData.data.colorG;
        sliderBlue.value = PlayerData.data.colorB;
    }


}
