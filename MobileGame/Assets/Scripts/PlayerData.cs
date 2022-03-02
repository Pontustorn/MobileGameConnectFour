using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
	[SerializeField]
	public static UserInfo data;
	static string userPath;
	public string userPathString;
	public Material mat;

	public void MyStart()
	{
		FindObjectOfType<FirebaseManager>().OnSignIn += OnSignIn;
		userPath = "users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId;
		userPathString = userPath;

		if(userPath != null)
        {
			OnSignIn();
        }
	}

	public void OnSignIn()
	{
		userPath = "users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId;
		SaveManager.Instance.LoadData(userPath, OnLoadData);
		userPathString = userPath;
	}

	void OnLoadData(string json)
	{
		if (json != null)
		{
			data = JsonUtility.FromJson<UserInfo>(json);
			mat.color = new Color(data.colorR, data.colorG, data.colorB);
		}
		
		data ??= new UserInfo();
		data.activeGames ??= new List<string>();
		SaveData();
		
	}

	public static void SaveData()
	{
		SaveManager.Instance.SaveData(userPath, JsonUtility.ToJson(data));
	}
}