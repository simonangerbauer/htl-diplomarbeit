using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public List<GameObject> MatchListItemPrefabs;
    public GameObject ChallengeAFriendItem;

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
            if(matches.Count > 0)
            {
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
                        GameObject scrollContent = GetChildWithNameOfGameObject("ScrollContent", GetChildWithNameOfGameObject("ScrollRect", menu));
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
                        Image UserImage = GetChildWithNameOfGameObject("Opponent", childObject).GetComponent<Image>();
                        var opponentId = "";
                        if (m.ChallengerId == FB.UserId)
                        {
                            opponentId = m.ChallengedId;
                            GetChildWithNameOfGameObject("OpponentScore", childObject).GetComponent<Text>().text = "Score: " + m.ChallengedScore;
                            GetChildWithNameOfGameObject("PlayerScore", childObject).GetComponent<Text>().text = "Your Score: " + m.ChallengerScore;
                        }
                        else
                        {
                            opponentId = m.ChallengerId;
                            GetChildWithNameOfGameObject("OpponentScore", childObject).GetComponent<Text>().text = "Score: " + m.ChallengerScore;
                            GetChildWithNameOfGameObject("PlayerScore", childObject).GetComponent<Text>().text = "Your Score: " + m.ChallengedScore;
                        }
                        FB.API(opponentId + "?fields=name", Facebook.HttpMethod.GET, delegate (FBResult result)
                        {
                            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.Text) as IDictionary;
                            var fbname = dict["name"].ToString();
                            GetChildWithNameOfGameObject("FacebookName", childObject).GetComponent<Text>().text = fbname;
                        });
                        FB.API(opponentId + "/picture", Facebook.HttpMethod.GET, delegate (FBResult result)
                        {
                            if (result.Error == null)
                                UserImage.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 50, 50), new Vector2());
                        });
                    }
                }
            
            }
            else
            {
                GetChildWithNameOfGameObject("EmptyText", menu).SetActive(true);
            }
        }
        else
        {
            LoginController login = LoginController.Instance;
            login.OnLoggedInDelegate += OpenMultiplayerMenu;
            login.InitAndLoginFacebook();
        }
    }
    public void LoadFriendsForListMenu()
    {
        if (FB.IsLoggedIn)
        {
            FacebookManager.instance.OnFriendsLoadedDelegate += ShowFriendListMenu;
            FacebookManager.instance.GetFriendUsers();
        }
        else
        {
            LoginController login = LoginController.Instance;
            login.OnLoggedInDelegate += LoadFriendsForListMenu;
            login.InitAndLoginFacebook();
        }
    }
    public void InviteFriends()
    {
        FacebookManager.instance.InviteFriends();
    }
    public void ShowFriendListMenu(List<object> friends)
    {
        GameObject menu;
        menu = GetChildWithNameOfGameObject("FriendListMenu", GameObject.Find("Canvas"));
        menu.SetActive(true);
        int scrollContentHeight = 0;
        foreach (IDictionary f in friends)
        {
            GameObject childObject = Instantiate(ChallengeAFriendItem) as GameObject;

            if (childObject != null)
            {
                GameObject scrollContent = GetChildWithNameOfGameObject("ScrollContent", GetChildWithNameOfGameObject("ScrollRect", menu));
                childObject.transform.SetParent(scrollContent.transform);
                childObject.transform.localScale = Vector3.one;
                childObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (scrollContentHeight + 45) * -1);
                scrollContentHeight += 97;
                scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, scrollContentHeight);
                GameObject playButton = GetChildWithNameOfGameObject("Play", childObject);
                if (playButton)
                {
                    Button b = playButton.GetComponent<Button>();
                    b.onClick.AddListener(delegate { StartMatch(f["id"].ToString()); });
                }
                Image UserImage = GetChildWithNameOfGameObject("Opponent", childObject).GetComponent<Image>();
                FB.API(f["id"]+"/picture", Facebook.HttpMethod.GET, delegate (FBResult result)
                    {
                        if (result.Error == null)
                            UserImage.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 50, 50), new Vector2());
                    });
                GetChildWithNameOfGameObject("FacebookName", childObject).GetComponent<Text>().text = f["name"].ToString();
            }
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
    public void CloseFriendListMenu()
    {
        GameObject menu;
        menu = GetChildWithNameOfGameObject("FriendListMenu", GameObject.Find("Canvas"));
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
