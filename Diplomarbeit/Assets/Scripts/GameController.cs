using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
	public int Distance = 0;
	public int Currency = 0;

	void Update()
	{
		if (this.Player != null)
		{
			Distance = (int)Player.transform.position.x;
			UpdateScoreText ();
		}
	}

	public void GameOver()
	{
		Application.LoadLevel (1);

	}
	void OnLevelWasLoaded(int level)
	{
		if (level == 1 || level == 0) 
		{
			Transform[] ts = GameObject.Find ("Canvas").transform.GetComponentsInChildren<Transform> (true);
			foreach (Transform t in ts)
				if (t.gameObject.name == "Score")
					this.ScoreText = t.gameObject.GetComponent<Text> ();
			UpdateScoreText ();
			this.Player = GameObject.Find("Player");
			this.Distance = 0;
			this.Currency = 0;
		} 
	}
	public void StartGame()
	{
		Application.LoadLevel (0);
	}
	public void OpenMenu()
	{
		Application.LoadLevel (2);
	}
	public void StartMultiplayer()
	{

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

}
