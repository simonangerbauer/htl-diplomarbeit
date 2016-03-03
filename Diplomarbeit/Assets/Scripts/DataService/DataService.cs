using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataService;
using System;
using AssemblyCSharp;

public class DataService : MonoBehaviour {
    private static DataService _instance;
    public static DataService instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DataService>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public Player Player { get; set; }

    //Can be anything, used for sharing data between two scenes; e.g. CurrentMatch
    public System.Object SharedData {
        get; set;
    }

    private ServerConnection connection;

	// Use this for initialization
	void Start () {
        connection = new ServerConnection();
    }

    public void AuthenticateUser(string userId)
    {
        JSONObject obj = new JSONObject();
        obj.AddField("userid", userId);

        LoginResponse loginResponse = new LoginResponse(connection.MakeRequest("login", obj));
        //When unable to login
        if (loginResponse.hasFailed()) 
        {
            //Benutzer existiert noch nicht
            if (loginResponse.IsAllowedToCreate())
            {
                //Es dürfen neue Benutzer registriert werden
                RegisterResponse registerResponse = new RegisterResponse(connection.MakeRequest("register", obj));

                //When user registred successfully
                if (!registerResponse.HasFailed())
                {
                    loginResponse = new LoginResponse(connection.MakeRequest("login", obj));
                    if (!loginResponse.hasFailed())
                    {
                        this.Player = new Player();

                        var lastLoginTime = loginResponse.GetLastLoginTime();
                        this.Player.LastLoginTime = lastLoginTime;

                        var playerId = loginResponse.GetPlayerId();
                        this.Player.Id = playerId;

                        var oldHighScore = loginResponse.GetHighScore();
                        this.Player.Highscore = oldHighScore;

                        this.Player.identNo = FB.UserId;
                    }
                }
            }
        }
        else
        {
            //User successfully signed in
            this.Player = new Player();

            var lastLoginTime = loginResponse.GetLastLoginTime();
            this.Player.LastLoginTime = lastLoginTime;

            var playerId = loginResponse.GetPlayerId();
            this.Player.Id = playerId;

            var oldHighScore = loginResponse.GetHighScore();
            this.Player.Highscore = oldHighScore;

            this.Player.identNo = FB.UserId;
        }

        Debug.Log("Heyo");
    }

    internal List<Match> GetMatchesForPlayer()
    {
        JSONObject obj = new JSONObject();
        obj.AddField("userid", this.Player.Id);

        MatchResponse matchResponse = new MatchResponse(connection.MakeRequest("getMatches", obj));
        if (!matchResponse.hasFailed())
        {
            var matches = matchResponse.GetMatches();
            return matches;
        }
        else
        {
            Debug.Log("Es konnten keine Matches gefunden werden, weil ein Error aufgetaucht ist!");
            return null;
        }
    }

    internal void UpdatePlayerCoins(int currency)
    {
        JSONObject obj = new JSONObject();
        obj.AddField("userid", this.Player.Id);
        obj.AddField("new_coins", currency);

        UpdateSingleScoreResponse scoreResponse = new UpdateSingleScoreResponse(connection.MakeRequest("updateCoinAmount", obj));
        if (!scoreResponse.hasFailed())
        {
            this.Player.Coins = this.Player.Coins+currency;
            Debug.Log("Coins wurden erfolgreich auf " + this.Player.Coins + " erhöht!");
        }
        else
        {
            Debug.Log("Es gibt einen Fehler, die Coinanzahl wurde nicht verändert!");
        }
    }

    internal void UpdatePlayerScore(int newScore)
    {
        JSONObject obj = new JSONObject();
        obj.AddField("userid", this.Player.Id);
        obj.AddField("new_score", newScore);

        UpdateSingleScoreResponse scoreResponse = new UpdateSingleScoreResponse(connection.MakeRequest("updateSingleScore", obj));
        if (!scoreResponse.hasFailed())
        {
            this.Player.Highscore = newScore;
            Debug.Log("Score wurde erfolgreich auf " + this.Player.Highscore + " erhöht!");
        }
        else
        {
            Debug.Log("Es gibt einen Fehler, der Highscore wurde nicht verändert!");
        }
    }

    internal void HandleMatch(Match match)
    {
        JSONObject obj = new JSONObject();
        obj.AddField("userid", this.Player.Id);

        JSONObject jsonMatch = new JSONObject();

        jsonMatch.AddField("id", match.Id);
        jsonMatch.AddField("challenger_id",match.ChallengerId);
        jsonMatch.AddField("challenged_facebook_id", match.ChallengedFbId);
        jsonMatch.AddField("challenger_score", match.ChallengerScore);
        jsonMatch.AddField("challenged_score", match.ChallengedScore);

        obj.AddField("match", jsonMatch);

        HandleMatchResponse scoreResponse = new HandleMatchResponse(connection.MakeRequest("handleMatch", obj));
        if (!scoreResponse.hasFailed())
        {
            Debug.Log("Das Match ist bearbeitet worden!");
        }
        else
        {
            Debug.Log("Das Match ist nicht bearbeitet worden!");
        }
    }
}
