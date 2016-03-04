using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace Assets.Scripts.DataService
{
    class ServerConnection
    {
        private String url = "http://huntandrun.htl-perg.ac.at:8080/HuntAndRun/";

        public JSONObject MakeRequest(string urlPath,JSONObject obj)
        {
            var decodedJson = obj.ToString();
            var jsonString = decodedJson;

            var encoding = new System.Text.UTF8Encoding();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Content-Type", "application/json");

            var finalUrlPath = url + (urlPath != "" ? urlPath : "");

            WWW www = new WWW(finalUrlPath, encoding.GetBytes(jsonString), dict);

            while (!www.isDone) { }

            var textResponse = www.text;

            JSONObject response = new JSONObject(textResponse);

            Debug.Log(www.text);

            return response;
        }

        //public void EstablishConnection()
        //{

        //    JSONObject obj = new JSONObject();

        //    obj.AddField("userid", "28838929917");
        //    obj.AddField("auth", "aspdoifjaspdoifup21");

        //    var decodedJson = obj.ToString();

        //    var jsonString = decodedJson;//"{\"seiler\":0}";

        //    var encoding = new System.Text.UTF8Encoding();
        //    Dictionary<string, string> dict = new Dictionary<string, string>();

        //    dict.Add("Content-Type", "application/json");

        //    foreach (KeyValuePair<String, String> entry in dict)
        //    {
        //        Console.WriteLine("Dictionary Entry: {0} = {1}", entry.Key, entry.Value);
        //    }

        //    WWW www = new WWW(url, encoding.GetBytes(jsonString), dict);

        //    while (!www.isDone) { }


        //    var text = www.text;
        //    //JSONObject obj = new JSONObject(www.text);
        //    Debug.Log("Getting this shit now");
        //    Debug.Log(www.text);
        //}
    }
}
