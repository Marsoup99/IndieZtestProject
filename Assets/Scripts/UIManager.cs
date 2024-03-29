using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set;}
    
    [SerializeField] private GameObject UIgameobject;
    [Header("User profile")]
    [SerializeField] private Image userProfileImg;
    [SerializeField] private TextMeshProUGUI userName;

    [Header("Log text")]
    [SerializeField] private TextMeshProUGUI textLog;

    [Header("Ready text")]
    [SerializeField] private TextMeshProUGUI rewardVideoText;
    [SerializeField] private TextMeshProUGUI interstitialText;
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Debug.LogWarning("there more than one UIManager");
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void ShowUI()
    {
        UIgameobject.SetActive(true);
    }
    public void UpdateProfileName(string name)
    {
        if(name == "")
            userName.SetText("Anoymous");
        else
            userName.SetText(name);
    }

    public void UpdateProfilePic()
    {
        string base64Tex = PlayerPrefs.GetString ("PlayerImg", null);

        if (!string.IsNullOrEmpty (base64Tex)) {
            // convert it to byte array
            byte[] texByte = System.Convert.FromBase64String (base64Tex);
            Texture2D tex = new Texture2D (2, 2);

            //load texture from byte array
            if (tex.LoadImage (texByte)) {
                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
                userProfileImg.sprite = newSprite;
            }
        }
    }

    public void LogTextDebug(string text)
    {
        textLog.text = text;
    }

    public void InterstitialText(bool ready)
    {
        if(ready)
        {
            interstitialText.text = "Ready";
        }
        else interstitialText.text = "Not Ready";
    } 
    public void RewardVideoText(bool ready)
    {
        if(ready)
        {
            rewardVideoText.text = "Ready";
        }
        else rewardVideoText.text = "Not Ready";
    } 

    public void InitIronSource()
    {
        IronSourceManager.Instance.InitIronSource();
    }
}
