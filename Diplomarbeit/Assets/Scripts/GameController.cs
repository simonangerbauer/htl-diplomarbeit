using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityStandardAssets._2D;

public class GameController : MonoBehaviour {
	private static GameController _instance;
	public static GameController instance
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
			_instance = this;
			DontDestroyOnLoad (this);
		} 
		else 
		{
			if(this != _instance)
				Destroy(this.gameObject);
		}
	}

	public GameObject Player;
	public Text ScoreText;
	private int Distance = 0;
	private int Currency = 0;

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
		ChangeMovementState (true);
	}
	public void ResetMainScene()
	{
		this.Player = GameObject.Find ("Player");
		this.Currency = 0;
		this.Distance = 0;
		this.ScoreText = GetChildWithNameOfGameObject("Score", GameObject.Find ("Canvas")).GetComponent<Text>();
	}

	public void GameOver()
	{
		ChangeMovementState (false);
		ChangeMenuStateOfMenuWithName (true, "GameOver");
		GameObject gameOverMenu = GetChildWithNameOfGameObject ("GameOver", GameObject.Find ("Canvas"));
		GetChildWithNameOfGameObject("Score", gameOverMenu).GetComponent<Text> ().text = "Score: " + (Distance + Currency *100);
		GetChildWithNameOfGameObject ("Coins", gameOverMenu).GetComponent<Text> ().text = "Coins: " + Currency;
		GetChildWithNameOfGameObject("Distance", gameOverMenu).GetComponent<Text> ().text = "Distance: " +Distance;

	}
	void OnLevelWasLoaded(int level)
	{
		if (level == 1) 
		{
			ResetMainScene();
			PrepareMainScene();
		}
	}
	public void StartGame()
	{
		Application.LoadLevel (1);
	}
	public void OpenMenu()
	{
		Application.LoadLevel (0);
	}
	public void StartMultiplayer()
	{

	}
	public void Pause()
	{
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
		GetChildWithNameOfGameObject ("ObstacleSpawner", GameObject.Find ("Main Camera")).GetComponent<Spawner> ().spawnObjects = state;
		GetChildWithNameOfGameObject ("EnemySpawner", GameObject.Find ("Main Camera")).GetComponent<Spawner> ().spawnObjects = state;
	}
	public void ChangeDistance(int amount)
	{
		Distance += amount;
		UpdateScoreText ();
	}
	public void ChangeCurrency(int amount)
	{
		Currency += amount;
		UpdateScoreText ();
	}
	public void UpdateScoreText()
	{
		ScoreText.text = "Score: " + (Distance + Currency * 100);
	}
	private GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
	{
		Transform[] ts = parent.transform.GetComponentsInChildren<Transform> (true);
		foreach (Transform t in ts)
			if (t.gameObject.name == child)
				return t.gameObject;
		return null;
	}

}
