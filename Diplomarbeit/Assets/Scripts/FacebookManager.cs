using UnityEngine;
using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
	void Awake()
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

	public void OnInitAndLogin(){
		FB.Login("email,publish_actions", LoginCallback); 

	}

	private void SetInit()                                                                       
	{                                                                                            
		Util.Log("SetInit");                                                                  
		enabled = true; // "enabled" is a property inherited from MonoBehaviour                  
		if (FB.IsLoggedIn)                                                                       
		{                                                                                        
			Util.Log("Already logged in");                                                    
			OnLoggedIn();                                                                        
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
		Util.Log("Logged in. ID: " + FB.UserId);

        //Try to get and set picture
        FB.API("me/picture", Facebook.HttpMethod.GET, GetPictureCallback);

        //Try to get and set name
        FB.API("me?fields=name", Facebook.HttpMethod.GET, GetNameCallback);

        //GameController.Instance.GetPlayerDataOrCreateNew(FB.UserId);
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
