using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceManager : MonoBehaviour
{
    [SerializeField] private string MY_APP_KEY_ANDROID;
    void Awake()
    {
        // Init
        InitIronSource();
    }

    private void InitIronSource()
    {
        //For Rewarded Video
        IronSource.Agent.init (MY_APP_KEY_ANDROID, IronSourceAdUnits.REWARDED_VIDEO);
        //For Interstitial
        IronSource.Agent.init (MY_APP_KEY_ANDROID, IronSourceAdUnits.INTERSTITIAL);
        // //For Offerwall
        // IronSource.Agent.init (MY_APP_KEY, IronSourceAdUnits.OFFERWALL);
        //For Banners
        IronSource.Agent.init (MY_APP_KEY_ANDROID, IronSourceAdUnits.BANNER);

        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }
    
        private void SdkInitializationCompletedEvent()
        {
            IronSource.Agent.validateIntegration();
        }
    
    void OnApplicationPause(bool isPaused) 
    {                 
        IronSource.Agent.onApplicationPause(isPaused);
    }
}
