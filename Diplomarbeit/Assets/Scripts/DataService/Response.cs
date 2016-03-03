using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataService
{
    public class Response
    {
        private static string STATUS = "status";
        private static string STATUS_OK = "ok";
        private static string STATUS_FAILED = "failed";

        public JSONObject JsonResponse { get; set; }

        public Response(JSONObject response){
            this.JsonResponse = response;
        }

        public bool hasFailed()
        {
            var statusField = JsonResponse.GetField(Response.STATUS).str;
            return statusField.Equals(Response.STATUS_FAILED) ? true:false;
        }
    }
}
