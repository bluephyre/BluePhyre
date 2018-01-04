using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace BluePhyre.Infrastructure.Apis.Linode
{
    public class LinodeApi
    {
        const string BaseUrl = "https://api.linode.com/v4";

        private readonly string _token;

        public LinodeApi(string token)
        {
            _token = token;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(BaseUrl);
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token, "Bearer");

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var exception = new ApplicationException(message, response.ErrorException);
                throw exception;
            }
            return response.Data;
        }

        public List<DomainRecord> Domains()
        {
            var request = new RestRequest();
            request.Resource = "/domains";
            request.RootElement = "data";

            return Execute<List<DomainRecord>>(request);
        }
    }

    public class DomainRecord
    {
        public long Id { get; set; }
        public string Domain { get; set; }
    }
}
