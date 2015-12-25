using UnityEngine;
using System.Collections;
using AssemblyCSharp;



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
    }

    public Player Player { get; set; }
    public OnLoggedInDelegate OnLoggedInDelegate { get; set; }

    void Start ()
    {
	    //TODO: Create Temp Player
	}

    public void InitAndLoginFacebook()
    {
        FacebookManager.instance.OnLoggedInDelegate += FacebookLoginCallback;
        FacebookManager.instance.OnInitAndLogin();
    }
    private void FacebookLoginCallback()
    {
        Player = new Player();
        Player.Id = FB.UserId;
        Player.Name = FacebookManager.instance.Name;
        GetPlayerData();

        OnLoggedInDelegate();
    }
    private void GetPlayerData()
    {
        //this.PlayerData = ConnectionManager.instance.GetPlayerDataForId(id);
        //TODO: Datenbankverbindung aufbauen und Daten holen
        
    }
}
