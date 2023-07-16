using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceManager : MonoBehaviour
{
    private string myAppKey;
    private bool bannerDisplay = false;
    [SerializeField] private IronSourceBannerPosition BannerPosition = IronSourceBannerPosition.BOTTOM;
    public static IronSourceManager Instance {get; private set;}
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

            //Init IronSource 
            DontDestroyOnLoad(this);
        } 
    }
    void Start()
    {
        myAppKey = ConstString.IRONSOURCE_API_KEY_ANDROID;

        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        InitIronSource();
    }
    
    private void InitIronSource()
    {
        //For Rewarded Video
        IronSource.Agent.init (myAppKey, IronSourceAdUnits.REWARDED_VIDEO);
        //For Interstitial
        IronSource.Agent.init (myAppKey, IronSourceAdUnits.INTERSTITIAL);
        // //For Offerwall
        // IronSource.Agent.init (MY_APP_KEY, IronSourceAdUnits.OFFERWALL);
        //For Banners
        IronSource.Agent.init (myAppKey, IronSourceAdUnits.BANNER);

        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }
    
    private void SdkInitializationCompletedEvent()
    {
        IronSource.Agent.validateIntegration();
        LoadInterstitial();
    }
    
    void OnApplicationPause(bool isPaused) 
    {                 
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        }
    }

    public void LoadInterstitial()
    {
        Debug.Log("unity-script: LoadInterstitialButtonClicked");
        IronSource.Agent.loadInterstitial();
    }
    
    public void ShowInterstitial()
    {
        Debug.Log("unity-script: ShowInterstitialButtonClicked");
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
        }
    }

    public void HideBanner()
    {
        Debug.Log("loadBanner hide");
        if(bannerDisplay)
        {
            IronSource.Agent.hideBanner();
            bannerDisplay = false;
        }
        else 
        {
            IronSource.Agent.displayBanner();
            bannerDisplay = true;
        }
            
            
    }
    public void LoadBanner()
    {
        bannerDisplay = true;
        Debug.Log("unity-script: loadBannerButtonClicked");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, BannerPosition);
    }

    public void DestroyBanner()
    {
        bannerDisplay = false;
        Debug.Log("unity-script: loadBannerButtonClicked");
        IronSource.Agent.destroyBanner();
    }

}
