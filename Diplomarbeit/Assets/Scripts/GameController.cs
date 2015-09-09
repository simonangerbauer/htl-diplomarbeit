using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityStandardAssets._2D;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
	private static GameController _instance;
	private GameController()
	{

	}
	public static GameController Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<GameController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	void Awake()
	{
		if (_instance == null) {
			GameController temp = GameObject.FindObjectOfType<GameController>();
			_instance = temp;
			if (Level == 0) {
				CloseMultiplayerMenu();
				InitAndLoginFacebook();
			}
			DontDestroyOnLoad (this.gameObject);
			DontDestroyOnLoad(this);
		} 
		else 
		{
			if(this != _instance)
				Destroy(this.gameObject);
				
		}
	}


	public List<GameObject> MatchListItemPrefabs;

	private int Distance = 0;
	private int Currency = 0;
	private Player PlayerData;
	private int Level = 0;
	private GameObject Player;
	private Text ScoreText;
	private Text CoinText;
	private Match ActiveMatch;

	void Update()
	{
		if (this.Player != null)
		{
			Distance = (int)Player.transform.position.x;
			UpdateScoreText ();
		}
	}
	public void PrepareMainScene()
	{
		ChangeMenuStateOfMenuWithName (false, "GameOver");
		ChangeMenuStateOfMenuWithName (false, "PauseMenu");
		Time.timeScale = 1.0f;
		ChangeMovementState (true);
	}
	public void ResetMainScene()
	{
		this.Player = GameObject.Find ("Player");
		this.Currency = 0;
		this.Distance = 0;
		this.ScoreText = GetChildWithNameOfGameObject("Score", GameObject.Find ("Canvas")).GetComponent<Text>();
        this.CoinText = GetChildWithNameOfGameObject("Coin", GetChildWithNameOfGameObject("Coins", GameObject.Find("Canvas"))).GetComponent<Text>();

	}

	public void GameOver()
	{
		ChangeMenuStateOfMenuWithName (true, "GameOver");
		GameObject gameOverMenu = GetChildWithNameOfGameObject ("GameOver", GameObject.Find ("Canvas"));
		GetChildWithNameOfGameObject("Score", gameOverMenu).GetComponent<Text> ().text = "Your Score: " + (Distance + Currency *10);
		GetChildWithNameOfGameObject ("Coins", gameOverMenu).GetComponent<Text> ().text = "Coins: " + Currency;
		GetChildWithNameOfGameObject("Distance", gameOverMenu).GetComponent<Text> ().text = "Distance: " +Distance;
		Time.timeScale = 0.0f;
		if ((Distance + (Currency * 10)) > PlayerData.Highscore)
			PlayerData.Highscore = (Distance + (Currency * 10));

		if (ActiveMatch != null) 
		{
			//ActiveMatch.ChallengerScore = Distance + Currency * 10;
			//if(ActiveMatch.ChallengerId != PlayerData.Id)
			//{
				GetChildWithNameOfGameObject("OpponentScore",gameOverMenu).GetComponent<Text>().text = "Opponent: 1253";
				//if(ActiveMatch.ChallengerScore > ActiveMatch.ChallengedScore)
				//	GetChildWithNameOfGameObject("MatchState", gameOverMenu).GetComponent<Text>().text = "You Lost..";
				//else
					GetChildWithNameOfGameObject("MatchState", gameOverMenu).GetComponent<Text>().text = "You Won!";
			//}
		}
		

		ConnectionManager.instance.UpdatePlayerData (this.PlayerData);
	}
	void OnLevelWasLoaded(int level)
	{
		if (level == 1) 
		{
			ResetMainScene();
			PrepareMainScene();
			Level = 1;
			if(ActiveMatch != null)
			{
				GameObject cam = GameObject.Find ("Main Camera");
				GetChildWithNameOfGameObject("EnemySpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
				GetChildWithNameOfGameObject("ObstacleSpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
			}
		}
		if (level == 0) 
		{
			ActiveMatch = null;
			PrepareMenuScene();
			CloseMultiplayerMenu();
			Level = 0;
		}
	}
	public void StartGame()
	{
		this.ActiveMatch = null;
		Application.LoadLevel (1);
	}
	public void OpenMenu()
	{
		Application.LoadLevel (0);
	}
	public 	void OpenMultiplayerMenu()
	{
		if (FB.IsLoggedIn) {
			GameObject menu;
			menu = GetChildWithNameOfGameObject ("MultiplayerMenu", GameObject.Find ("Canvas"));
			menu.SetActive (true);
			List<Match> matches = ConnectionManager.instance.GetMatchesForPlayerId (FB.UserId);
			int scrollContentHeight = 0;
			foreach (Match m in matches) 
			{
				GameObject childObject;
				if(m.Winner == "none")
				{
					if(m.ChallengerId == FB.UserId)
					{
						//Instantiate ChallengerItem
						childObject = Instantiate(MatchListItemPrefabs[0]) as GameObject;
					}
					else
					{
						//Instantiate ChallengedItem
						childObject = Instantiate(MatchListItemPrefabs[1]) as GameObject;
					}
				}
				else
				{
					if(m.Winner == FB.UserId)
					{
						//Instantiate WonItem
						childObject = Instantiate(MatchListItemPrefabs[2]) as GameObject;
					}
					else
					{
						//Instantiate LostItem
						childObject = Instantiate(MatchListItemPrefabs[3]) as GameObject;
					}
				}
				if(childObject != null)
				{
					GameObject scrollContent = GetChildWithNameOfGameObject("ScrollContent", GetChildWithNameOfGameObject("ScrollRect",GetChildWithNameOfGameObject("MultiplayerMenu",GameObject.Find ("Canvas"))));
					childObject.transform.SetParent(scrollContent.transform);
					childObject.transform.localScale = Vector3.one;
					//childObject.GetComponent<RectTransform>().offsetMax = new Vector2(0,scrollContentHeight*-1);
					//childObject.GetComponent<RectTransform>().offsetMin = new Vector2(0,scrollContentHeight*-1);
					childObject.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0,(scrollContentHeight+	45)*-1);
					scrollContentHeight += 97;
					scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, scrollContentHeight);
					GameObject playButton = GetChildWithNameOfGameObject("Play", childObject);
					if(playButton)
					{
						Button b = playButton.GetComponent<Button>();
						b.onClick.AddListener(delegate{ StartMatch(m.Id);});
					}
					//GetChildWithNameOfGameObject("Opponent",childObject).GetComponent<Image>().sprite = new Sprite(); //TODO:Load Facebook Image and replace new Sprite() with it
					if(m.ChallengerId == FB.UserId)
					{
						GetChildWithNameOfGameObject("OpponentScore",childObject).GetComponent<Text>().text = "Score: "+m.ChallengedScore;
						GetChildWithNameOfGameObject("PlayerScore",childObject).GetComponent<Text>().text = "Your Score: "+m.ChallengerScore;
						GetChildWithNameOfGameObject("FacebookName",childObject).GetComponent<Text>().text = m.ChallengedId;//TODO:Load Facebook Name and replace PlayerId with it
					}
					else
					{
						GetChildWithNameOfGameObject("OpponentScore",childObject).GetComponent<Text>().text = "Score: "+m.ChallengerScore;
						GetChildWithNameOfGameObject("PlayerScore",childObject).GetComponent<Text>().text = "Your Score: "+m.ChallengedScore;
						GetChildWithNameOfGameObject("FacebookName",childObject).GetComponent<Text>().text = m.ChallengerId;//TODO:Load Facebook Name and replace PlayerId with it
					}
				}
			}
		}
		else
			InitAndLoginFacebook ();
		
	}
	public 	void CloseMultiplayerMenu()
	{	
		GameObject menu;
		menu = GetChildWithNameOfGameObject ("MultiplayerMenu", GameObject.Find ("Canvas"));
		menu.SetActive (false);
	}
	public void Pause()
	{        
		Time.timeScale = 0.0f;
		ChangeMovementState (false);
		ChangeMenuStateOfMenuWithName (true, "PauseMenu");
	}
	public void ChangeMenuStateOfMenuWithName(bool state, string menu)
	{
		GameObject menuobject = GetChildWithNameOfGameObject(menu, GameObject.Find ("Canvas"));
		menuobject.SetActive (state);
		GameObject dimmingScreen = GetChildWithNameOfGameObject ("DimmingScreen", GameObject.Find ("Canvas"));
		dimmingScreen.SetActive (state);
	}
	public void ChangeMovementState(bool state)
	{
		GameObject player = GameObject.Find ("Player");
		player.GetComponent<Platformer2DUserControl> ().move = state;
	}
	public void ChangeDistance(int amount)
	{
		Distance += amount;
		UpdateScoreText ();
	}
	public void ChangeCurrency(int amount)
	{
		Currency += amount;
		this.PlayerData.Coins += amount;
		UpdateScoreText ();
	}
	public void UpdateScoreText()
	{
		ScoreText.text = "Score: " + (Distance + Currency * 10);
        CoinText.text = "" + this.PlayerData.Coins;
	}
	private GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
	{
		Transform[] ts = parent.transform.GetComponentsInChildren<Transform> (true);
		foreach (Transform t in ts)
			if (t.gameObject.name == child)
				return t.gameObject;
		return null;
	}
	public void GetPlayerDataOrCreateNew(string id)
	{
		this.PlayerData = ConnectionManager.instance.GetPlayerDataForId (id);
	}
	public void InitAndLoginFacebook()
	{
		if (!FB.IsInitialized)
			FacebookManager.instance.CallFBInit ();
		else if (!FB.IsLoggedIn) {
			FacebookManager.instance.CallFBLogin ();
		} else 
		{
			GetPlayerDataOrCreateNew(FB.UserId);
		}
			
	}
	public void StartMatch(string id)
	{
		ActiveMatch = ConnectionManager.instance.GetMatchForId(id);
		Application.LoadLevel (1);
	}
	public void ChangeMenuInteractivity(bool interactive)
	{
		GameObject menu = GetChildWithNameOfGameObject ("Menu", GameObject.Find ("Canvas"));
		foreach (Transform child in menu.transform) {
			Button b = child.GetComponent<Button>();
			if(b != null)
			{
				b.interactable = interactive;
			}
		}
	}

	public void PrepareMenuScene()
	{
		GetChildWithNameOfGameObject("Multiplayer",GetChildWithNameOfGameObject ("Menu", GameObject.Find ("Canvas"))).GetComponent<Button>().onClick.AddListener(delegate{OpenMultiplayerMenu();});
	}
}
