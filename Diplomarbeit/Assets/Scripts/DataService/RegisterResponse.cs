using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataService
{
    class RegisterResponse : Response
    {
        private static string SUCCESSFULLY_CREATED = "successfully_created";
        private static string STATUS_YES = "yes";
        private static string STATUS_NO = "no";


        public RegisterResponse(JSONObject response) : base(response)
        {

        }

        public bool HasFailed()
        {
            var statusField = JsonResponse.GetField(RegisterResponse.SUCCESSFULLY_CREATED).str;
            return statusField.Equals(RegisterResponse.STATUS_NO) ? true : false;
        }

    }
}
