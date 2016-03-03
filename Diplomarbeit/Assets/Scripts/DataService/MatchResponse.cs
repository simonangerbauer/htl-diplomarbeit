using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataService
{
    class MatchResponse:Response
    {
        private static string MATCHES = "matches";

        private static string ID_CHALLENGE = "id_challenge";
        private static string CHALLENGER_ID = "user1";
        private static string CHALLENGED_ID = "user2";
        private static string SEED = "seed";
        private static string CHALLENGED_SCORE = "challenged_score";
        private static string CHALLENGER_SCORE = "challenger_score";
        private static string CHALLENGER_FB_ID = "challenger_fb_id";
        private static string CHALLENGED_FB_ID = "challenged_fb_id";


        public MatchResponse(JSONObject response) : base(response)
        {

        }

        public List<Match> GetMatches()
        {
            List <Match> list = new List<Match>();

            var jsonMatch = JsonResponse.GetField(MatchResponse.MATCHES);
            var jsonMatchesList = jsonMatch.list;

            foreach (JSONObject obj in jsonMatchesList)
            {
                Match tempMatch = new Match();
                tempMatch.ChallengedId = (int)obj[CHALLENGED_ID].n;
                tempMatch.ChallengerId = (int)obj[CHALLENGER_ID].n;
                tempMatch.ChallengedScore = (int)obj[CHALLENGED_SCORE].n;
                tempMatch.ChallengerScore = (int)obj[CHALLENGER_SCORE].n;
                tempMatch.Id = (int)obj[ID_CHALLENGE].n;
                tempMatch.Seed = (int)obj[SEED].n;
                tempMatch.ChallengedFbId = obj[CHALLENGED_FB_ID].str;
                tempMatch.ChallengerFbId = obj[CHALLENGER_FB_ID].str;

                list.Add(tempMatch);
            }

            return list;
        }
    }
}
