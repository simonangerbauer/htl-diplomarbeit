using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using Assets.Scripts.DataService;

public class LoginController : MonoBehaviour {
    private static LoginController _instance;
    public static LoginController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LoginController>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);

        }
        if (FB.IsLoggedIn)
            FacebookManager.instance.GetPictureAndName();
    }

    public Player Player { get; set; }
    public OnLoggedInDelegate OnLoggedInDelegate { get; set; }

    void Start ()
    {
        Player = new Player { Id = 123, Coins = 12, Highscore = 123, Matches = new List<Match>(), Name = "Koal", Powerups = new List<Powerup>()};
	    //TODO: Create Temp Player
	}

    public void InitAndLoginFacebook()
    {
        if(!FB.IsLoggedIn)
        {
            FacebookManager.instance.OnLoggedInDelegate += FacebookLoginCallback;
            FacebookManager.instance.OnInitAndLogin();
        }
        
    }
    private void FacebookLoginCallback()
    {
        DataService.instance.AuthenticateUser(FB.UserId);

        Player = new Player();
        //Player.Id = user.Id+"";
        Player.Name = FacebookManager.instance.Name;
        OnLoggedInDelegate += GameObject.Find("MainMenuController").GetComponent<MainMenuController>().ShowLoggedInMenu;
        OnLoggedInDelegate();
        FacebookManager.instance.GetPictureAndName();

        
    }
}
