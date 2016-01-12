using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataService : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var encoding = new System.Text.UTF8Encoding();

        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Content-Type", "application/json");

        JSONObject obj = new JSONObject();

        obj.AddField("userid", "28838929917");
        obj.AddField("auth", "aspdoifjaspdoifup21");

        var decodedJson = obj.ToString();

        var jsonString = decodedJson;//"{\"seiler\":0}";

        WWW www = new WWW("http://127.0.0.1:8080/HuntAndRun/login",encoding.GetBytes(jsonString),dict);

        StartCoroutine(WaitForRequest(www));
        while (!www.isDone) { }


        var text = www.text;
        //JSONObject obj = new JSONObject(www.text);
        Debug.Log("Getting this shit now");
        Debug.Log(www.text);

        //JSONObject obj = new JSONObject(www.text);


    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}
