using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToDev.Analytics;

public class DevtodevManager : MonoBehaviour
{
    public static DevtodevManager Instance {get; private set;}
    public DTDLogLevel logLevel;
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Debug.LogWarning("there more than one DevtodevManager");
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(this);
        } 
    }
    public void Initialize(string uid)
    {
        DTDAnalytics.CoppaControlEnable();
        DTDAnalytics.SetLogLevel(logLevel);
        var config = new DTDAnalyticsConfiguration
        {
            UserId = FirebaseManager.Instance.GetUserID()
        };
#if UNITY_ANDROID
            DTDAnalytics.Initialize(ConstString.DTD_ANDROID_APP_KEY, config);
// #elif UNITY_IOS
//         DTDAnalytics.Initialize("iOSAppID", config);
// #elif UNITY_WEBGL
//         DTDAnalytics.Initialize("WebAppID", config);
// #elif UNITY_STANDALONE_WIN
//         DTDAnalytics.Initialize("winAppID", config);
// #elif UNITY_STANDALONE_OSX
//         DTDAnalytics.Initialize("OSXAppID", config);
// #elif UNITY_WSA
//         DTDAnalytics.Initialize("UwpAppID", config);
#endif
    }

    public void Tutorial (int stage)
    {
        DTDAnalytics.Tutorial(stage);
        
    }
    public void LvlUp ()
    {
        DTDAnalytics.LevelUp(level: Random.Range(0,100));
    }
}
