using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        GameController.Instance.GetPlayerDataOrCreateNew(FB.UserId);
    }        

}
