using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance {get; private set;}

    public DependencyStatus dependencyStatus;
    public FirebaseAuthCtrl AuthCtrl;
    public FirebaseRealtimeDatabase DatabaseCtrl;

    void Reset()
    {
        AuthCtrl = GetComponentInChildren<FirebaseAuthCtrl>();
        DatabaseCtrl = GetComponentInChildren<FirebaseRealtimeDatabase>();

    }
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Debug.LogWarning("there more than one FirebaseManager");
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 

            //Init firebase 
            Initialize();
            DontDestroyOnLoad(this);
        } 
    }

    private void Initialize()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                AuthCtrl.Initialize();
                DatabaseCtrl.Initialize();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void LoginViaFacebook()
    {
        AuthCtrl.LoginViaFacebook();
    }
    public string GetUserID()
    {
        return AuthCtrl.user.UserId;
    }
}
