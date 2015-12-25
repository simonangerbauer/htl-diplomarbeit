using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public List<GameObject> MatchListItemPrefabs;

    // Use this for initialization
    void Start ()
    {
        PrepareMenuScene();
        CloseMultiplayerMenu();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        Application.LoadLevel(1);
    }

    public void OpenMultiplayerMenu()
    {
        if (FB.IsLoggedIn)
        {
            GameObject menu;
            menu = GetChildWithNameOfGameObject("MultiplayerMenu", GameObject.Find("Canvas"));
            menu.SetActive(true);
            List<Match> matches = ConnectionManager.instance.GetMatchesForPlayerId(FB.UserId);
            int scrollContentHeight = 0;
            foreach (Match m in matches)
            {
                GameObject childObject;
                if (m.Winner == "none") //TODO: Datenbank Matches
                {
                    if (m.ChallengerId == FB.UserId)
                        //Instantiate ChallengerItem
                        childObject = Instantiate(MatchListItemPrefabs[0]) as GameObject;
                    else
                        //Instantiate ChallengedItem
                        childObject = Instantiate(MatchListItemPrefabs[1]) as GameObject;
                }
                else
                {
                    if (m.Winner == FB.UserId)
                        //Instantiate WonItem
                        childObject = Instantiate(MatchListItemPrefabs[2]) as GameObject;
                    else
                        //Instantiate LostItem
                        childObject = Instantiate(MatchListItemPrefabs[3]) as GameObject;
                }
                if (childObject != null)
                {
                    GameObject scrollContent = GetChildWithNameOfGameObject("ScrollContent", GetChildWithNameOfGameObject("ScrollRect", GetChildWithNameOfGameObject("MultiplayerMenu", GameObject.Find("Canvas"))));
                    childObject.transform.SetParent(scrollContent.transform);
                    childObject.transform.localScale = Vector3.one;
                    childObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (scrollContentHeight + 45) * -1);
                    scrollContentHeight += 97;
                    scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, scrollContentHeight);
                    GameObject playButton = GetChildWithNameOfGameObject("Play", childObject);
                    if (playButton)
                    {
                        Button b = playButton.GetComponent<Button>();
                        b.onClick.AddListener(delegate { StartMatch(m.Id); });
                    }
                    //GetChildWithNameOfGameObject("Opponent",childObject).GetComponent<Image>().sprite = new Sprite(); //TODO:Load Facebook Image and replace new Sprite() with it
                    if (m.ChallengerId == FB.UserId)
                    {
                        GetChildWithNameOfGameObject("OpponentScore", childObject).GetComponent<Text>().text = "Score: " + m.ChallengedScore;
                        GetChildWithNameOfGameObject("PlayerScore", childObject).GetComponent<Text>().text = "Your Score: " + m.ChallengerScore;
                        GetChildWithNameOfGameObject("FacebookName", childObject).GetComponent<Text>().text = m.ChallengedId;//TODO:Load Facebook Name and replace PlayerId with it
                    }
                    else
                    {
                        GetChildWithNameOfGameObject("OpponentScore", childObject).GetComponent<Text>().text = "Score: " + m.ChallengerScore;
                        GetChildWithNameOfGameObject("PlayerScore", childObject).GetComponent<Text>().text = "Your Score: " + m.ChallengedScore;
                        GetChildWithNameOfGameObject("FacebookName", childObject).GetComponent<Text>().text = m.ChallengerId;//TODO:Load Facebook Name and replace PlayerId with it
                    }
                }
            }
        }
        else
        {
            LoginController login = GameObject.Find("LoginController").GetComponent<LoginController>();
            login.OnLoggedInDelegate += ShowLoggedInMenu;
            login.OnLoggedInDelegate += OpenMultiplayerMenu;
            login.InitAndLoginFacebook();
        }
    }
    public void ShowLoggedInMenu()
    {
        GameObject menu = GetChildWithNameOfGameObject("SignedInMenu", GameObject.Find("Canvas"));
        menu.SetActive(true);
    }
    public void CloseMultiplayerMenu()
    {
        GameObject menu;
        menu = GetChildWithNameOfGameObject("MultiplayerMenu", GameObject.Find("Canvas"));
        menu.SetActive(false);
    }
    private GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
    {
        Transform[] ts = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
            if (t.gameObject.name == child)
                return t.gameObject;
        return null;
    }
    public void StartMatch(string id)
    {
        //TODO: Matches üwalegn
        //ActiveMatch = ConnectionManager.instance.GetMatchForId(id);
        Application.LoadLevel(1);
    }
    public void PrepareMenuScene()
    {
        if (!FB.IsLoggedIn)
            GameObject.Find("SignedInMenu").SetActive(false);
        GetChildWithNameOfGameObject("Multiplayer", GetChildWithNameOfGameObject("Menu", GameObject.Find("Canvas"))).GetComponent<Button>().onClick.AddListener(delegate { OpenMultiplayerMenu(); });
    }
}
