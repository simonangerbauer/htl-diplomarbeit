using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataService
{
    class LoginResponse : Response
    {
        private static string ALLOWED_TO_CREATE = "allowed_to_create";
        private static string STATUS_YES = "yes";
        private static string STATUS_NO = "no";

        private static string LAST_LOGIN_ON = "last_login_on";

        private static string PLAYER_ID = "player_id";

        private static string HIGH_SCORE = "high_score";

        private static string COINS = "coins";


        public LoginResponse(JSONObject response) : base(response)
        {

        }

        public bool IsAllowedToCreate()
        {
            var statusField = JsonResponse.GetField(LoginResponse.ALLOWED_TO_CREATE).str;
            return statusField.Equals(LoginResponse.STATUS_YES) ? true : false;
        }

        public DateTime GetLastLoginTime()
        {
            var dateField = JsonResponse.GetField(LoginResponse.LAST_LOGIN_ON).str;
            DateTime time = DateTime.Parse(dateField);
            return time;
        }

        public int GetPlayerId()
        {
            var playerId = Int32.Parse(JsonResponse.GetField(LoginResponse.PLAYER_ID).str);
            return playerId;
        }

        internal int GetHighScore()
        {
            var playerId = Int32.Parse(JsonResponse.GetField(LoginResponse.HIGH_SCORE).str);
            return playerId;
        }

        internal int GetCoins()
        {
            var playerId = Int32.Parse(JsonResponse.GetField(LoginResponse.COINS).str);
            return playerId;
        }
    }
}
