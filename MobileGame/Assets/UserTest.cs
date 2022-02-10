using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTest : MonoBehaviour
{
    string userID;

    void Start()
    {
        userID = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        SaveManager.Instance.LoadData("users/" + userID, UserDataLoaded);
    }

    private void Update()
    {

    }
    public void UserDataLoaded(string jsonData)
    {
        var userInfo = JsonUtility.FromJson<UserInfo>(jsonData);
        Debug.Log(userInfo.username);
        Debug.Log(userInfo.losses);
    }
}
