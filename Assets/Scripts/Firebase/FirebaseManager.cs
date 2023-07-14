using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance {get; private set;}

    public DependencyStatus dependencyStatus;
    public FirebaseAuthCtrl AuthCtrl;

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                AuthCtrl.Initialize();
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
}
