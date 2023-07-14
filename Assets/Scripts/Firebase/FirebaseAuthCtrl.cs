using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Include Facebook namespace
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class FirebaseAuthCtrl : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public FirebaseAuth auth;    
    public FirebaseUser User;

    public Image img;
    public TextMeshProUGUI text;
    void Awake()
    {
        //Init fb
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void Initialize()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            //Check if player had logged before
            if(FB.IsLoggedIn)
            {
                FB.Mobile.RefreshCurrentAccessToken();
                var accessToken = AccessToken.CurrentAccessToken;
                Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

                StartCoroutine(AuthWithFirebase(credential));
            }
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void LoginViaFacebook()
    {
        if(FB.IsLoggedIn) return;
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
            return;
            //TODO: Show a text for player to try agin
        }
        var perms = new List<string>(){"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, FacebookAuthCallback);
    }

    private void FacebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn) 
        {
            var accessToken = AccessToken.CurrentAccessToken;
            Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

            StartCoroutine(AuthWithFirebase(credential));
        }
        else 
        {
            Debug.Log("User cancel login");
        }
    }

    private IEnumerator  AuthWithFirebase(Credential credential)
    {
        var task = auth.SignInAndRetrieveDataWithCredentialAsync(credential);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsCanceled) 
        {
            Debug.Log("SignInAndRetrieveDataWithCredentialAsync was canceled.");
            yield break;
        }
        else if (task.IsFaulted) 
        {
            Debug.Log("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
            yield break;
        }
        else 
        {
            // Firebase.Auth.AuthResult result = task.Result;
            User = task.Result.User;
            SetText();
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.UserId);
        }
    }

    private void SetText()
    {
        text.SetText(User.DisplayName);
        StartCoroutine(GetTexture(User.PhotoUrl.ToString()));
    }
    IEnumerator GetTexture(string url) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            Texture2D imageTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite newSprite = Sprite.Create(imageTex, new Rect(0, 0, imageTex.width, imageTex.height), new Vector2(.5f, .5f));
            img.sprite = newSprite;
        }
    }
}


