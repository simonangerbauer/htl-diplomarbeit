using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ShowPauseMenu()
    {
        ChangeMenuStateOfMenuWithName(true, "PauseMenu");
    }
    public void ShowGameOverMenu(int Distance, int Currency)
    {
        ChangeMenuStateOfMenuWithName(true, "GameOver");
        GameObject gameOverMenu = GetChildWithNameOfGameObject("GameOver", GameObject.Find("Canvas"));
        GetChildWithNameOfGameObject("Score", gameOverMenu).GetComponent<Text>().text = "Your Score: " + (Distance + Currency * 10);
        GetChildWithNameOfGameObject("Coins", gameOverMenu).GetComponent<Text>().text = "Coins: " + Currency;
        GetChildWithNameOfGameObject("Distance", gameOverMenu).GetComponent<Text>().text = "Distance: " + Distance;
    }
    public void DisplayMultiplayerScore(bool winner, int challengerScore)
    {
        GameObject gameOverMenu = GetChildWithNameOfGameObject("GameOver", GameObject.Find("Canvas"));
        GetChildWithNameOfGameObject("OpponentScore", gameOverMenu).GetComponent<Text>().text = "Opponent Score: " + challengerScore;
        if (winner)
        {
            GetChildWithNameOfGameObject("MatchState", gameOverMenu).GetComponent<Text>().text = "You Won!";
        }
        else
        {
            GetChildWithNameOfGameObject("MatchState", gameOverMenu).GetComponent<Text>().text = "You Lost..";
        }
    }
    public void HideMenus()
    {
        ChangeMenuStateOfMenuWithName(false, "GameOver");
        ChangeMenuStateOfMenuWithName(false, "PauseMenu");
    }
    public void OpenMenu()
    {
        Application.LoadLevel(0);
    }
    public void StartGame()
    {
        Application.LoadLevel(1);
    }
    public void ChangeMenuStateOfMenuWithName(bool state, string menu)
    {
        GameObject menuobject = GetChildWithNameOfGameObject(menu, GameObject.Find("Canvas"));
        menuobject.SetActive(state);
        GameObject dimmingScreen = GetChildWithNameOfGameObject("DimmingScreen", GameObject.Find("Canvas"));
        dimmingScreen.SetActive(state);
    }
    public GameObject GetChildWithNameOfGameObject(string child, GameObject parent)
    {
        Transform[] ts = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
            if (t.gameObject.name == child)
                return t.gameObject;
        return null;
    }
    public void ChangeMenuInteractivity(bool interactive)
    {
        GameObject menu = GetChildWithNameOfGameObject("Menu", GameObject.Find("Canvas"));
        foreach (Transform child in menu.transform)
        {
            Button b = child.GetComponent<Button>();
            if (b != null)
            {
                b.interactable = interactive;
            }
        }
    }
}
