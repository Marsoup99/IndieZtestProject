using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class FirebaseRealtimeDatabase : MonoBehaviour
{
    public void Initialize()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SavingPlayerData(string UserID)
    {

    }

    public void LoadPlayerData(string UserID)
    {

    }
}
