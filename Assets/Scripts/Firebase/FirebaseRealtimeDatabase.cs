using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
public class FirebaseRealtimeDatabase : MonoBehaviour
{
    private DatabaseReference dbReference;
    public void Initialize()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser(string UserID)
    {
        UserData newUser = new UserData();
        string jsonData = JsonUtility.ToJson(newUser);

        dbReference.Child("user").Child(UserID).SetRawJsonValueAsync(jsonData);
    }

    public IEnumerator GetUserFirstTimeLogin(string UserID)
    {
        UIManager.Instance?.LogTextDebug("waiting");
        
        var task = dbReference.Child("user").Child(UserID).Child("firstTimeLogin").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);


        UIManager.Instance?.LogTextDebug(task.Result.ToString());
    }
    public void SavingPlayerData(string UserID)
    {
        
    }

    public void LoadPlayerData(string UserID)
    {

    }
}

public class UserData
{
    public string firstTimeLogin;
    public UserData()
    {
        this.firstTimeLogin = System.DateTime.Now.ToShortTimeString();
    }
}
