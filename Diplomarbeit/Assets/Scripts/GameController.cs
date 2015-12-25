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
			DontDestroyOnLoad (this.gameObject);
			DontDestroyOnLoad(this);
		} 
		else 
		{
			if(this != _instance)
				Destroy(this.gameObject);
				
		}
	}



	private int Distance = 0;
	private int Currency = 0;
	private Player PlayerData;
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
		//ChangeMovementState (true);
	}
	public void ResetMainScene()
	{
		this.Player = GameObject.Find ("FatCharacter");
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
			if(ActiveMatch != null)
			{
				GameObject cam = GameObject.Find ("Main Camera");
				GetChildWithNameOfGameObject("EnemySpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
				GetChildWithNameOfGameObject("ObstacleSpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
			}
		}
	}

	public void OpenMenu()
	{
		Application.LoadLevel (0);
	}

	public void Pause()
	{        
		Time.timeScale = 0.0f;
		//ChangeMovementState (false);
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
		//GameObject player = GameObject.Find ("Player");
		//player.GetComponent<Platformer2DUserControl> ().move = state;
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
        //CoinText.text = "" + this.PlayerData.Coins;
	}
	private GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
	{
		Transform[] ts = parent.transform.GetComponentsInChildren<Transform> (true);
		foreach (Transform t in ts)
			if (t.gameObject.name == child)
				return t.gameObject;
		return null;
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
}
