using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    private MenuHandler menuHandler;
    private SpawnChipScript spc;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;


    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_Text wins;
    public TMP_Text losses;
    public TMP_Text chipsPlaced;
    public GameObject scoreElement;
    public Transform scoreBoardContent;


    string owner = "";

    public Material mat;
    public ChangeChip cc;

    private void Start()
    {
        menuHandler = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();
        spc = GameObject.Find("GameController").GetComponent<SpawnChipScript>();
    }

    // Start is called before the first frame update
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();

            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies");
            }
        });

        
    }


    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    // Update is called once per frame
    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void StartGameButton()
    {
        StartCoroutine(CheckIfAvailableGame());
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        StartCoroutine(UpdateUserWins(0));
        StartCoroutine(UpdateUserLosses(0));
        StartCoroutine(UpdateUserChipsPlaced(0));

    }


    public void Test()
    {
        
    }

    public void ClearLoginField()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterField()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    public void SignOutButton()
    {
        auth.SignOut();
        menuHandler.Login();
        ClearLoginField();
        ClearRegisterField();
    }
    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "User Not Found";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            usernameField.text = User.DisplayName;
            StartCoroutine(LoadUserData());
            menuHandler.MainMenu();
            ClearLoginField();
            ClearRegisterField();
            //StartCoroutine(checkIfUserIsPlaying());
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if(_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
        }

        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if(RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";

                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;

            }

            else
            {
                User = RegisterTask.Result;

                if(User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    var ProfileTask = User.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if(ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed";
                    }
                    else
                    {
                        usernameField.text = User.DisplayName;
                        menuHandler.MainMenu();
                        ClearLoginField();
                        ClearRegisterField();
                        StartCoroutine(UpdateUserWins(0));
                        StartCoroutine(UpdateUserLosses(0));
                        StartCoroutine(UpdateUserChipsPlaced(0));
                        StartCoroutine(UpdateUserPlaying("false"));
                        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        UserProfile profile = new UserProfile { DisplayName = _username };

        var ProfileTask = User.UpdateUserProfileAsync(profile);

        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if(ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");

        }

        else
        {
            //Auth username is updated.
        }

    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            
        }
    }

    private IEnumerator UpdateUserWins(int _wins)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("wins").SetValueAsync(_wins);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            
        }
    }

    private IEnumerator UpdateUserPlaying(string _playing)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("playing").SetValueAsync(_playing);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            
        }
    }

    private IEnumerator UpdateUserLosses(int _losses)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("losses").SetValueAsync(_losses);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Username is updated.
        }
    }



    private IEnumerator UpdateUserChipsPlaced(int _chipsplaced)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("chipsPlaced").SetValueAsync(_chipsplaced);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Username is updated.
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        
        else if(DBTask.Result.Value == null)
        {
            //No data exists.
            wins.text = "";
            losses.text = "";
            chipsPlaced.text = "";
        }

        else
        {
            DataSnapshot snapshot = DBTask.Result;

            wins.text = snapshot.Child("wins").Value.ToString();
            losses.text = snapshot.Child("losses").Value.ToString();
            chipsPlaced.text = snapshot.Child("chipsPlaced").Value.ToString();
            

            if(snapshot.Child("playing").Value.ToString() == "true")
            {
                menuHandler.ActivateGameScreen();
                StartCoroutine(getOwner());
            }


        }

    }

   

    private IEnumerator CreateGame()
    {
        var DBTask = DBreference.Child("games").Child(User.UserId).Child("player1").SetValueAsync(User.UserId);
        

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            var DBTask2 = DBreference.Child("games").Child(User.UserId).Child("player2").SetValueAsync("");
            yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

            if (DBTask2.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
            }
            else
            {
                var DBTask3 = DBreference.Child("games").Child(User.UserId).Child("gameFull").SetValueAsync("false");
                yield return new WaitUntil(predicate: () => DBTask3.IsCompleted);

                if (DBTask3.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask3.Exception}");
                }
                else
                {
                    var DBTask4 = DBreference.Child("games").Child(User.UserId).Child("playerTurn").SetValueAsync(User.UserId);
                    yield return new WaitUntil(predicate: () => DBTask4.IsCompleted);

                    if (DBTask4.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {DBTask4.Exception}");
                    }
                    else
                    {
                        owner = User.UserId;
                    }
                }
            }
        }
    }

    private IEnumerator AddPlayerToGame(string _userId, string _owner)
    {
        var DBTask = DBreference.Child("games").Child(_owner).Child("player2").SetValueAsync(_userId);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            var DBTask2 = DBreference.Child("games").Child(_owner).Child("gameFull").SetValueAsync("true");

            yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

            if (DBTask2.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
            }

            else
            {
                var DBTask3 = DBreference.Child("users").Child(_owner).Child("playing").SetValueAsync("true");

                yield return new WaitUntil(predicate: () => DBTask3.IsCompleted);

                if (DBTask3.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask3.Exception}");
                }

                else
                {
                    var DBTask4 = DBreference.Child("users").Child(_userId).Child("playing").SetValueAsync("true");

                    yield return new WaitUntil(predicate: () => DBTask4.IsCompleted);

                    if (DBTask4.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {DBTask4.Exception}");
                    }
                }
            }
        }
    }

    private IEnumerator CheckIfAvailableGame()
    {
        bool MatchHasNotBeenFound = false;
        var DBTask = DBreference.Child("games").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else if (DBTask.Result.Value == null)
        {
            StartCoroutine(CreateGame());

            yield return new WaitForSeconds(4);
            menuHandler.ActivateLoadingScreen();
        }

        else
        {
            DataSnapshot snapShot = DBTask.Result;



            foreach(DataSnapshot childSnapShot in snapShot.Children)
            {
                if(childSnapShot.Child("gameFull").Value.ToString() == "false" && MatchHasNotBeenFound == false)
                {
                    owner = childSnapShot.Child("player1").Value.ToString();
                    if (owner != User.UserId)
                    {
                        StartCoroutine(AddPlayerToGame(User.UserId, owner));
                        MatchHasNotBeenFound = true;
                        yield return new WaitForSeconds(4);
                        menuHandler.ActivateGameScreen();
                    }

                    else
                    {
                        StartCoroutine(CreateGame());
                    }
                }
            }

            if(MatchHasNotBeenFound == false)
            {
                StartCoroutine(CreateGame());
                yield return new WaitForSeconds(4);
                menuHandler.ActivateLoadingScreen();
            }
        }
    }

    public IEnumerator playerPlayedChip(string column, string placement)
    {
        string player2 = "";
        string playerturn = "";
        var DBTask = DBreference.Child("games").Child(owner).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {

            DataSnapshot snapShot = DBTask.Result;

           
                
                    player2 = snapShot.Child("player2").Value.ToString();
                    playerturn = snapShot.Child("playerTurn").Value.ToString();
                    
                 
                

                if (owner != null)
                {

                    var DBTask2 = DBreference.Child("games").Child(owner).Child("column").Child(placement).SetValueAsync(column);

                    yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

                    if (DBTask2.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
                    }

                    else
                    {
                        if (player2 == User.UserId)
                        {

                            var DBTask3 = DBreference.Child("games").Child(owner).Child("playerTurn").SetValueAsync(owner);

                            yield return new WaitUntil(predicate: () => DBTask3.IsCompleted);

                            if (DBTask3.Exception != null)
                            {
                                Debug.LogWarning(message: $"Failed to register task with {DBTask3.Exception}");
                            }

                            else
                            {

                            }
                        }

                        else
                        {
                            var DBTask3 = DBreference.Child("games").Child(User.UserId).Child("playerTurn").SetValueAsync(player2);

                            yield return new WaitUntil(predicate: () => DBTask3.IsCompleted);

                            if (DBTask3.Exception != null)
                            {
                                Debug.LogWarning(message: $"Failed to register task with {DBTask3.Exception}");
                            }

                            else
                            {

                            }
                        }


                    }
                }

            }
    }

    public IEnumerator CheckWhosTurnItIs(System.Action<bool> callback)
    {

        var DBTask = DBreference.Child("games").Child(owner).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapShot = DBTask.Result;

            
                    if (snapShot.Child("playerTurn").Value.ToString() == User.UserId)
                    {
                        callback(true);
                        yield return null;
                    }
                }
            
        
    }

    public IEnumerator LoadChips(System.Action<List<int>> callback)
    {

        yield return new WaitForSeconds(4f);
        var DBTask2 = DBreference.Child("games").Child(owner).Child("column").GetValueAsync();
        List<int> intList = new List<int>();
        string placementString = "";

        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

        if (DBTask2.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }

        else
        {
            
                
                    DataSnapshot snapShot2 = DBTask2.Result;

                    foreach (DataSnapshot childSnapShot2 in snapShot2.Children)
                    {

                    placementString = childSnapShot2.Value.ToString();
                    intList.Add(int.Parse(placementString));
                    }

            
            callback(intList);
            yield return null;
        }
    }

    public IEnumerator ReturnLatestChip(System.Action<int> callback)
    {
        int returnValue = 0;
        var DBTask2 = DBreference.Child("games").Child(owner).Child("column").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

        if (DBTask2.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
        }

        else
        {

                DataSnapshot snapShot2 = DBTask2.Result;

                foreach (DataSnapshot childSnapShot2 in snapShot2.Children)
                {
                    returnValue++;
                }

            callback(returnValue);
            yield return null;
        }
    }

    public IEnumerator WhichPlayerAmI(System.Action<bool> callback)
    {
        bool returnValue = false;
        bool MatchHasBeenFound = false;
        var DBTask = DBreference.Child("games").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {
            DataSnapshot snapShot = DBTask.Result;

            foreach (DataSnapshot childSnapShot in snapShot.Children)
            {
                if (childSnapShot.Child("player1").Value.ToString() == User.UserId && MatchHasBeenFound == false)
                {
                    returnValue = true;
                }

                else if(childSnapShot.Child("player2").Value.ToString() == User.UserId && MatchHasBeenFound == false)
                {
                    returnValue = false;
                }
            }

            callback(returnValue);
            yield return null;
        }
    }



    public IEnumerator getOwner()
    {
        bool MatchHasBeenFound = false;
        var DBTask = DBreference.Child("games").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else
        {

            DataSnapshot snapShot = DBTask.Result;

            foreach (DataSnapshot childSnapShot in snapShot.Children)
            {
                if (childSnapShot.Child("player1").Value.ToString() == User.UserId && MatchHasBeenFound == false || childSnapShot.Child("player2").Value.ToString() == User.UserId && MatchHasBeenFound == false)
                {
                    owner = childSnapShot.Child("player1").Value.ToString();
                    Debug.Log(owner);
                    MatchHasBeenFound = true;
                }
            }
        }
    }

    //void HandleChildAdded(object sender, ChildChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);


    //        return;
    //    }

    //        StartCoroutine(ReturnLatestChip((myReturnValue) => {

    //            spc.SpawnChipOnChange(myReturnValue);

    //        }));



    //    // Do something with the data in args.Snapshot
    //}

    //public void setupListener()
    //{

    //    StartCoroutine(getOwner((myReturnValue) => {
    //        owner = myReturnValue;
    //        Debug.Log(owner);
    //    }));



    //    var dbref = DBreference.Child("games").Child(owner).Child("column");

    //    dbref.ChildAdded += HandleChildAdded;


    //}
}
