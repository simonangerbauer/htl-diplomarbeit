using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataService
{
    class UpdateSingleScoreResponse : Response
    {
        private static String SUCCESSFULLY_UPDATED = "successfully_updated";
        private static string STATUS_YES = "yes";
        private static string STATUS_NO = "no";


        public UpdateSingleScoreResponse(JSONObject response) : base(response)
        {

        }

        public bool HasFailed()
        {
            var statusField = JsonResponse.GetField(UpdateSingleScoreResponse.SUCCESSFULLY_UPDATED).str;
            return statusField.Equals(UpdateSingleScoreResponse.STATUS_NO) ? true : false;
        }

    }
}
