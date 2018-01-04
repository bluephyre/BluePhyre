using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace BluePhyre.Infrastructure.Apis.DnSimple
{
    public class DnSimpleApi
    {

        const string BaseUrl = "https://api.dnsimple.com/v2";

        readonly string _username;
        readonly string _password;

        public DnSimpleApi(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(BaseUrl);
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var exception = new ApplicationException(message, response.ErrorException);
                throw exception;
            }
            return response.Data;
        }

        public List<Account> Accounts()
        {
            var request = new RestRequest();
            request.Resource = "/accounts";
            request.RootElement = "data";

            //request.AddParameter("CallSid", callSid, ParameterType.UrlSegment);

            return Execute<List<Account>>(request);
        }

        public List<Zone> Zones(long accountId)
        {
            var request = new RestRequest();
            request.Resource = "/{accountId}/zones";
            request.RootElement = "data";

            request.AddParameter("accountId", accountId, ParameterType.UrlSegment);
            request.AddParameter("per_page", 100, ParameterType.QueryString);

            return Execute<List<Zone>>(request);
        }

    }

    public class Zone
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public bool Reverse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Account
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PlanIdentifier { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }


}
