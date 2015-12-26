using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityStandardAssets._2D;
using AssemblyCSharp;

public class GameController : MonoBehaviour {
	private int Distance = 0;
	private int Currency = 0;
    public float Multiplicator = 1.0f;
	private Player Player;
	public GameObject PlayerGameObject;
	public Text ScoreText;
	public Text CoinText;
	public Match ActiveMatch;
    public GameMenuController GameMenuController;
    private int deltaDistance;
    void Start()
    {
        this.Player = LoginController.Instance.Player;
        PrepareMainScene();
        if (ActiveMatch != null)
        {
            GameObject cam = GameObject.Find("Main Camera");
            GetChildWithNameOfGameObject("EnemySpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
            GetChildWithNameOfGameObject("ObstacleSpawner", cam).GetComponent<Spawner>().Seed = ActiveMatch.Seed;
        }
        deltaDistance =(int) PlayerGameObject.transform.position.x;
    }
	void Update()
	{
        Debug.Log(PlayerGameObject.transform.position.x);
		Distance = (int)(PlayerGameObject.transform.position.x*Multiplicator) - deltaDistance;
        UpdateScoreText();
    }
	public void PrepareMainScene()
	{
        Time.timeScale = 1.0f;
        GameMenuController.HideMenus();
	}

	public void GameOver()
	{
        GameMenuController.ShowGameOverMenu(Distance, Currency);
        
        Time.timeScale = 0.0f;
		if ((Distance + (Currency * 10)) > Player.Highscore)
			Player.Highscore = (Distance + (Currency * 10));

		if (ActiveMatch != null) 
		{
            if (ActiveMatch.ChallengerId != Player.Id)
            {
                ActiveMatch.ChallengedScore = Distance + Currency * 10;
                if (ActiveMatch.ChallengerScore > ActiveMatch.ChallengedScore)
                {
                    GameMenuController.DisplayMultiplayerScore(false, ActiveMatch.ChallengerScore);
                    ActiveMatch.Winner = ActiveMatch.ChallengerId;
                }
                else
                {
                    GameMenuController.DisplayMultiplayerScore(true, ActiveMatch.ChallengerScore);
                    ActiveMatch.Winner = ActiveMatch.ChallengedId;
                }  
            }
            else
                ActiveMatch.ChallengerScore = Distance + Currency * 10;
        }
        //TODO: Datenbankanbindung GameOver (Matches & Playerscore)
		ConnectionManager.instance.UpdatePlayerData (this.Player);
	}

	public void Pause()
	{        
		Time.timeScale = 0.0f;
        GameMenuController.ShowPauseMenu();
	}
	public void ChangeDistance(int amount)
	{
		Distance += amount;
        UpdateScoreText();
    }
	public void ChangeCurrency(int amount)
	{
		Currency += amount;
	}
	public void UpdateScoreText()
	{
		ScoreText.text = "Score: " + (Distance + Currency * 10);
        CoinText.text = (this.Player.Coins+Currency).ToString();
	}
    public GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
    {
        Transform[] ts = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
            if (t.gameObject.name == child)
                return t.gameObject;
        return null;
    }
}
