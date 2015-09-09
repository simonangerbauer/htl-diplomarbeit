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
	}



	protected string status = "Ready";
	
	protected string lastResponse = "";

	protected void Callback(FBResult result)
	{
		// Some platforms return the empty string instead of null.
		if (!String.IsNullOrEmpty (result.Error))
		{
			lastResponse = "Error Response:\n" + result.Error;
		}
		else if (!String.IsNullOrEmpty (result.Text))
		{
			lastResponse = "Success Response:\n" + result.Text;
		}
		else if (result.Texture != null)
		{
			lastResponse = "Success Response: texture\n";
		}
		else
		{
			lastResponse = "Empty Response\n";
		}
	}

	
	#region FB.Init() example
	
	public void CallFBInit()
	{
		Time.timeScale = 0.0f;
		FB.Init(OnInitComplete, OnHideUnity);
	}
	
	private void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		if (FB.IsInitialized)
			CallFBLogin ();
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
		if (isGameShown) {
			Time.timeScale = 1.0f;
		} else {
			Time.timeScale = 0.0f;
		}
	}

	#endregion
	
	#region FB.Login() example
	
	public void CallFBLogin()
	{
		GameController.Instance.ChangeMenuInteractivity (false);
		FB.Login("public_profile,email,user_friends", LoginCallback);
	}
	
	private void CallFBLoginForPublish()
	{
		// It is generally good behavior to split asking for read and publish
		// permissions rather than ask for them all at once.
		//
		// In your own game, consider postponing this call until the moment
		// you actually need it.
		FB.Login("publish_actions", LoginCallback);
	}
	
	void LoginCallback(FBResult result)
	{
		Time.timeScale = 1.0f;
		GameController.Instance.ChangeMenuInteractivity (true);
		Debug.Log (result.Text);
		if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn)
		{
			lastResponse = "Login cancelled by Player";
		}
		else
		{
			lastResponse = "Login was successful!";
			GameController.Instance.GetPlayerDataOrCreateNew(FB.UserId);
		}
	}
	
	private void CallFBLogout()
	{
		FB.Logout();
	}
	#endregion



	#region FB.ActivateApp() example
	
	private void CallFBActivateApp()
	{
		FB.ActivateApp();
		Callback(new FBResult("Check Insights section for your app in the App Dashboard under \"Mobile App Installs\""));
	}
	
	#endregion
	
	#region FB.AppRequest() Friend Selector
	
	public string FriendSelectorTitle = "";
	public string FriendSelectorMessage = "Derp";
	private string[] FriendFilterTypes = new string[] { "None (default)", "app_users", "app_non_users" };
	private int FriendFilterSelection = 0;
	private List<string> FriendFilterGroupNames = new List<string>();
	private List<string> FriendFilterGroupIDs = new List<string>();
	private int NumFriendFilterGroups = 0;
	public string FriendSelectorData = "{}";
	public string FriendSelectorExcludeIds = "";
	public string FriendSelectorMax = "";
	
	private void CallAppRequestAsFriendSelector()
	{
		// If there's a Max Recipients specified, include it
		int? maxRecipients = null;
		if (FriendSelectorMax != "")
		{
			try
			{
				maxRecipients = Int32.Parse(FriendSelectorMax);
			}
			catch (Exception e)
			{
				status = e.Message;
			}
		}
		
		// include the exclude ids
		string[] excludeIds = (FriendSelectorExcludeIds == "") ? null : FriendSelectorExcludeIds.Split(',');
		
		// Filter groups
		List<object> FriendSelectorFilters = new List<object>();
		if (FriendFilterSelection > 0)
		{
			FriendSelectorFilters.Add(FriendFilterTypes[FriendFilterSelection]);
		}
		if (NumFriendFilterGroups > 0)
		{
			for (int i = 0; i < NumFriendFilterGroups; i++)
			{
				FriendSelectorFilters.Add(
					new FBAppRequestsFilterGroup(
					FriendFilterGroupNames[i],
					FriendFilterGroupIDs[i].Split(',').ToList()
					)
					);
			}
		}
		
		FB.AppRequest(
			FriendSelectorMessage,
			null,
			(FriendSelectorFilters.Count > 0) ? FriendSelectorFilters : null,
			excludeIds,
			maxRecipients,
			FriendSelectorData,
			FriendSelectorTitle,
			Callback
			);
	}
	#endregion
	
	#region FB.AppRequest() Direct Request
	
	public string DirectRequestTitle = "";
	public string DirectRequestMessage = "Herp";
	private string DirectRequestTo = "";
	
	private void CallAppRequestAsDirectRequest()
	{
		if (DirectRequestTo == "")
		{
			throw new ArgumentException("\"To Comma Ids\" must be specificed", "to");
		}
		FB.AppRequest(
			DirectRequestMessage,
			DirectRequestTo.Split(','),
			null,
			null,	
			null,
			"",
			DirectRequestTitle,
			Callback
			);
	}
	
	#endregion
	
	#region FB.Feed() example
	
	public string FeedToId = "";
	public string FeedLink = "";
	public string FeedLinkName = "";
	public string FeedLinkCaption = "";
	public string FeedLinkDescription = "";
	public string FeedPicture = "";
	public string FeedMediaSource = "";
	public string FeedActionName = "";
	public string FeedActionLink = "";
	public string FeedReference = "";
	public bool IncludeFeedProperties = false;
	private Dictionary<string, string[]> FeedProperties = new Dictionary<string, string[]>();
	
	private void CallFBFeed()
	{
		Dictionary<string, string[]> feedProperties = null;
		if (IncludeFeedProperties)
		{
			feedProperties = FeedProperties;
		}
		FB.Feed(
			toId: FeedToId,
			link: FeedLink,
			linkName: FeedLinkName,
			linkCaption: FeedLinkCaption,
			linkDescription: FeedLinkDescription,
			picture: FeedPicture,
			mediaSource: FeedMediaSource,
			actionName: FeedActionName,
			actionLink: FeedActionLink,
			reference: FeedReference,
			properties: feedProperties,
			callback: Callback
			);
	}
	
	#endregion
	
	#region FB.Canvas.Pay() example
	
	public string PayProduct = "";
	
	private void CallFBPay()
	{
		FB.Canvas.Pay(PayProduct);
	}
	
	#endregion
	
	#region FB.API() example
	
	public void CallFBAPI(string query)
	{
		FB.API(query, Facebook.HttpMethod.GET, Callback);
	}
	
	#endregion
	
	#region FB.GetDeepLink() example
	
	private void CallFBGetDeepLink()
	{
		FB.GetDeepLink(Callback);
	}
	
	#endregion
	
	#region FB.AppEvent.LogEvent example
	
	public void CallAppEventLogEvent()
	{
		FB.AppEvents.LogEvent(
			Facebook.FBAppEventName.UnlockedAchievement,
			null,
			new Dictionary<string,object>() {
			{ Facebook.FBAppEventParameterName.Description, "Clicked 'Log AppEvent' button" }
		}
		);
		Callback(new FBResult(
			"You may see results showing up at https://www.facebook.com/insights/" +
			FB.AppId +
			"?section=AppEvents"
			)
		         );
	}
	
	#endregion
	
	#region FB.Canvas.SetResolution example
	
	public string Width = "800";
	public string Height = "600";
	public bool CenterHorizontal = true;
	public bool CenterVertical = false;
	public string Top = "10";
	public string Left = "10";
	
	public void CallCanvasSetResolution()
	{
		int width;
		if (!Int32.TryParse(Width, out width))
		{
			width = 800;
		}
		int height;
		if (!Int32.TryParse(Height, out height))
		{
			height = 600;
		}
		float top;
		if (!float.TryParse(Top, out top))
		{
			top = 0.0f;
		}
		float left;
		if (!float.TryParse(Left, out left))
		{
			left = 0.0f;
		}
		if (CenterHorizontal && CenterVertical)
		{
			FB.Canvas.SetResolution(width, height, false, 0, FBScreen.CenterVertical(), FBScreen.CenterHorizontal());
		}
		else if (CenterHorizontal)
		{
			FB.Canvas.SetResolution(width, height, false, 0, FBScreen.Top(top), FBScreen.CenterHorizontal());
		}
		else if (CenterVertical)
		{
			FB.Canvas.SetResolution(width, height, false, 0, FBScreen.CenterVertical(), FBScreen.Left(left));
		}
		else
		{
			FB.Canvas.SetResolution(width, height, false, 0, FBScreen.Top(top), FBScreen.Left(left));
		}
	}
	
	#endregion
	
	#region GUI

	
	private IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();
		
		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();
		
		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
		wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
		
		FB.API("me/photos", Facebook.HttpMethod.POST, Callback, wwwForm);
	}
	
	#endregion
}
