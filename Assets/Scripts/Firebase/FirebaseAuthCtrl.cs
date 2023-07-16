using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Include Facebook namespace
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseAuthCtrl : MonoBehaviour
{
    public FirebaseAuth auth {get; private set;}
    public FirebaseUser user {get; private set;}

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
        if(auth.CurrentUser != null)
        {
            user = auth.CurrentUser;
            GetUserInfo();
            UIManager.Instance?.LogTextDebug("User login (UID:" + user.UserId +")");
        }
        else 
        {
            StartCoroutine(SignInAnoymousFirebase());
        }
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
        if(FB.IsLoggedIn) 
        {
            UIManager.Instance?.LogTextDebug("User already logged fb acount.");
            return;
        }
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
            return;
            //TODO: Show a text for player to try agin
        }
        var perms = new List<string>(){"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, FacebookAuthCallback);

        UIManager.Instance?.LogTextDebug("User linking fb acount...");
    }

    private void FacebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn) 
        {
            var accessToken = AccessToken.CurrentAccessToken;
            Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

            StartCoroutine(LinkUserInFirebase(credential));
        }
        else 
        {
            Debug.Log("User cancel login");
        }
    }

     private IEnumerator SignInAnoymousFirebase()
    {
        var task = auth.SignInAnonymouslyAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsCanceled) {
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
            yield break;
        }
        if (task.IsFaulted) {
            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
            yield break;
        }
        else
        {
            user = task.Result.User;
            GetUserInfo();
            UIManager.Instance?.LogTextDebug("User login (UID:" + user.UserId +")");
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        }
    }

    private IEnumerator SignInFirebase(Credential credential)
    {
        var task = auth.SignInAndRetrieveDataWithCredentialAsync(credential);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
            if (task.IsCanceled) {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                yield break;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                yield break;
            }
            else{
                //Apply new UserID
                user = task.Result.User;
                GetUserInfo();
                UIManager.Instance?.LogTextDebug("User signed in successfully: User name:" + user.DisplayName + "/n UserID:" + user.UserId);
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            }
            
    }
    private IEnumerator LinkUserInFirebase(Credential credential)
    {
        var task = auth.CurrentUser.LinkWithCredentialAsync(credential);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        // auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
        //Wait until the task completes
        if (task.IsCanceled) {
            Debug.LogError("LinkWithCredentialAsync was canceled.");
            yield break;
        }
        else if (task.IsFaulted) {
            UIManager.Instance?.LogTextDebug("User already link account, signing in to that account /n");
            StartCoroutine(SignInFirebase(credential));
            Debug.LogWarning("LinkWithCredentialAsync encountered an error: " + task.Exception);
            yield break;
        }
        else 
        {
            // Firebase.Auth.AuthResult result = task.Result;
            user = task.Result.User;
            GetUserInfo();
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        }
    }
    private void GetUserInfo()
    {
        if(FB.IsLoggedIn)
        {
            if(user.DisplayName == "")
            {
                FB.API ("/me?fields=name", HttpMethod.GET, result =>{
                    Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile {
                        DisplayName = result.ResultDictionary["name"].ToString(),};
                    user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => { UIManager.Instance?.UpdateProfileName(user.DisplayName);});
                });
            }
            else UIManager.Instance?.UpdateProfileName(user.DisplayName);
            FB.API ("/me/picture?type=square&height=100&width=100", HttpMethod.GET, UpdateProfilePictureFB);
        }
        else 
        {
            UIManager.Instance?.UpdateProfileName(user.DisplayName);
            UIManager.Instance?.UpdateProfilePic();
        }

        UpdateUserID();
    }

    private void UpdateProfilePictureFB(IGraphResult result)
    {
        if(result.Texture != null) 
        {
            Sprite newSprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(.5f, .5f));
            // if texture is png otherwise you can use tex.EncodeToJPG().
            byte[] texByte = result.Texture.EncodeToPNG ();

            // convert byte array to base64 string
            string base64Tex = System.Convert.ToBase64String (texByte);

            // write string to playerpref
            PlayerPrefs.SetString ("PlayerImg", base64Tex);
            PlayerPrefs.Save ();
        }
        UIManager.Instance?.UpdateProfilePic();
    }

    public void FirebaseAuthSignOut()
    {
        if(auth.CurrentUser != null)
        {
            FacebookLogOut();
            auth.SignOut();
            StartCoroutine(SignInAnoymousFirebase());
        }
    }
    public void FacebookLogOut()
    {
        if(FB.IsLoggedIn)
        {
            FB.LogOut();
        }
    }

    private void UpdateUserID()
    {
        DevtodevManager.Instance.Initialize(user.UserId);
        DevtodevManager.Instance.Tutorial(1);
    }
}


