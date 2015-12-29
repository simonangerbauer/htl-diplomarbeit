using UnityEngine;
using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public delegate void OnLoggedInDelegate();
public delegate void OnFriendsLoadedDelegate(List<object> friendIds);

public class FacebookManager : MonoBehaviour {
	private static FacebookManager _instance;
	private FacebookManager()
	{
		
	}
	public static FacebookManager instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<FacebookManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

    public OnLoggedInDelegate OnLoggedInDelegate { get; set; }
    public OnFriendsLoadedDelegate OnFriendsLoadedDelegate { get; set; }
    public string Name { get; set; }

    void Start()
	{
		if (_instance == null) {
			_instance = this;

			DontDestroyOnLoad (this);
		} 
		else 
		{
			if(this != _instance)
				Destroy(this.gameObject);
		}

		FB.Init(SetInit, OnHideUnity);
	}
    public void OnInitAndLogin()
    {
		FB.Login("email,publish_actions", LoginCallback); 

	}
    public void ChallengeAFriend()
    {
        FB.AppRequest("I challenge you! Can you beat me?", null, null, null, 1, "", "Challenge a friend!", ChallengeCallback);
    }

    void ChallengeCallback(FBResult result)
    {
        Debug.Log(result.Text);
    }
	private void SetInit()                                                                       
	{                                                                                            
		Util.Log("SetInit");                                                                  
		enabled = true; // "enabled" is a property inherited from MonoBehaviour                  
		if (FB.IsLoggedIn)                                                                       
		{                                                                                        
			Util.Log("Already logged in");                                                    
			//OnLoggedIn();                                                                        
		}                                                                                        
	}                                                                                            
	
	private void OnHideUnity(bool isGameShown)                                                   
	{                                                                                            
		Util.Log("OnHideUnity");                                                              
		if (!isGameShown)                                                                        
		{                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		}                                                                                        
		else                                                                                     
		{                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}                                                                                        
	}

	void LoginCallback(FBResult result)                                                        
	{                                                                                          
		Util.Log("LoginCallback");                                                          
		
		if (FB.IsLoggedIn)                                                                     
		{                                                                                      
			OnLoggedIn();                                                       
		}                                                                                 
	}                                                                                          
	void OnLoggedIn()                                                                          
	{                                                                               
		Util.Log("Logged in. Unique UserID: " + FB.UserId);

        //GameController.Instance.GetPlayerDataOrCreateNew(FB.UserId);
        if(OnLoggedInDelegate != null)
            OnLoggedInDelegate();
    }
    public void GetPictureAndName()
    {
        //Try to get and set picture
        FB.API("me/picture", Facebook.HttpMethod.GET, GetPictureCallback);

        //Try to get and set name
        FB.API("me?fields=name", Facebook.HttpMethod.GET, GetNameCallback);
    }
	public void GetFriendUsers()
    {
        FB.API("/me/friends?fields=installed,name", HttpMethod.GET, GetFriendUsersCallback);
    }
    private void GetFriendUsersCallback(FBResult result)
    {
        if (result.Error == null)
        {
            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.Text) as IDictionary;
            List<object> users = dict["data"] as List<object>;
            OnFriendsLoadedDelegate(users);
        }
        
    }
    public void InviteFriends()
    {
        FB.AppRequest(message: "This game is awesome. I want to challenge you. Come on, join me. now.", title: "Invite your friends to join you");
    }
    private void GetPictureCallback(FBResult result)
    {
        if (result.Error == null)
        {
            GameObject signedInMenu = GetChildWithNameOfGameObject("SignedInMenu", GameObject.Find("Canvas"));
            GameObject profilePicture = GetChildWithNameOfGameObject("Player_ProfilePic", signedInMenu);

            var img = profilePicture.GetComponent<Image>();
            img.sprite = Sprite.Create(result.Texture, new Rect(0,0, 50, 50), new Vector2());
        }
    }

    private void GetNameCallback(FBResult result)
    {
        if (result.Error == null)
        {
            GameObject signedInMenu = GetChildWithNameOfGameObject("SignedInMenu", GameObject.Find("Canvas"));
            GameObject name = GetChildWithNameOfGameObject("Player_Name", signedInMenu);

            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.Text) as IDictionary;
            var fbname = dict["name"].ToString();

            var txt = name.GetComponent<Text>();
            txt.text = fbname;
        }
    }

    private GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
    {
        Transform[] ts = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
            if (t.gameObject.name == child)
                return t.gameObject;
        return null;
    }

}
