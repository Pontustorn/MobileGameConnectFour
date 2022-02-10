using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeChip : MonoBehaviour
{

    Vector3 mousePos;
    Vector3 newMousePos;

    public Slider sliderRed;
    public Slider sliderGreen;
    public Slider sliderBlue;

    public Material mat;

    public GameObject usedImage;

    SpriteRenderer sr;

    public Sprite[] imageList;

    public int changeCounter = 1;

    ChipInfo chipInfo;
    

    // Start is called before the first frame update
    void Start()
    {
        chipInfo = new ChipInfo();
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

    public void SetSliders(float r, float g, float b)
    {
        sliderRed.value = r;
        sliderGreen.value = g;
        sliderBlue.value = b;
    }

    public void UpdateValues()
    {

        chipInfo.colorR = sliderRed.value;
        chipInfo.colorG = sliderGreen.value;
        chipInfo.colorB = sliderBlue.value;

        string jsonString = JsonUtility.ToJson(chipInfo);

        SaveManager.Instance.SaveData("users/" + Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/chip", jsonString);
    }

    public void LoadValues()
    {
        SaveManager.Instance.LoadData("users/" + Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId + "/chip", UserDataLoaded);
    }

    public void UserDataLoaded(string jsonData)
    {
        var chipInfo = JsonUtility.FromJson<ChipInfo>(jsonData);

        sliderRed.value = chipInfo.colorR;
        sliderGreen.value = chipInfo.colorG;
        sliderBlue.value = chipInfo.colorB;

    }


}
