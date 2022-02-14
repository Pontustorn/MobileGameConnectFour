using Firebase.Auth;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
   
	public static UserInfo data;
	static string userPath;
	public string userPathString;
	public Material mat;

	private void Start()
	{
		FindObjectOfType<FirebaseManager>().OnSignIn += OnSignIn;
		userPath = "users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId;
		userPathString = userPath;
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
		else
		{
			data = new UserInfo();
			SaveData();
		}
	}

	public static void SaveData()
	{
		SaveManager.Instance.SaveData(userPath, JsonUtility.ToJson(data));
	}
}