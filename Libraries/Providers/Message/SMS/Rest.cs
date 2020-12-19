using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using RestSharp;
using RestSharp.Validation;

namespace SMS
{
    public class Rest
    {
        public abstract class CRestResult
        {
            /// <summary>
            /// Exception encountered during API request
            /// </summary>
            public RestException RestException { get; set; }
            /// <summary>
            /// The URI for this resource, relative to https://api.twilio.com
            /// </summary>
            public Uri Uri { get; set; }
        }

        /// <summary>
        /// Exceptions returned in the HTTP response body when something goes wrong.
        /// </summary>
        public class RestException
        {
            /// <summary>
            /// The HTTP status code for the exception.
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// (Conditional) The Url of Twilio's documentation for the error code.
            /// </summary>
            public string MoreInfo { get; set; }
            /// <summary>
            /// (Conditional) An error code to find help for the exception.
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// A more descriptive message regarding the exception.
            /// </summary>
            public string Message { get; set; }
        }

        //private RestClient client = null;

        //#region ProviderSms
        /// <summary>
        /// Send a new ProviderSms message to the specified recipients
        /// Makes a POST request to the SMSMessages List resource.
        /// </summary>
        /// <param name="from">The phone number to send the message from. Must be a Twilio-provided or ported local (not toll-free) number. Validated outgoing caller IDs cannot be used.</param>
        /// <param name="to">The phone number to send the message to. If using the Sandbox, this number must be a validated outgoing caller ID</param>
        /// <param name="body">The message to send. Must be 160 characters or less.</param>
        /// <param name="statusCallback">A Url that Twilio will POST to when your message is processed. Twilio will POST the SmsSid as well as SmsStatus=sent or SmsStatus=failed</param>

        // todo: fix when Origization.Client and result objects are is defined
        //public ??? SendSmsMessage(Origization.Client client, string to, string body)
        public bool Send(string toPhone, string textMessage)
        {
            // todo: get parameters from Orginization.client object
            string BaseUrl = "https://api.twilio.com";
            string APIVersion = "2010-04-01";
            string AccountSid = "ACb095f77308ab428fb502973df7b0e085";
            string AuthToken = "6660bc2741bd4e2be42960e14815bd8b";
            string FromNumber = "(949) 891-0981";
            string Resource = @"Accounts/{AccountSid}/SMS/Messages";

            RestClient client = new RestClient();
            client.Authenticator = new HttpBasicAuthenticator(
                AccountSid, 
                AuthToken);

            client.BaseUrl = string.Format("{0}/{1}", 
                BaseUrl, 
                APIVersion);

            var request = new RestRequest(Method.POST);
            request.Resource = Resource;
            request.AddParameter("From", FromNumber);
            request.AddParameter("To", toPhone);
            request.AddParameter("Body", textMessage);
            
            //todo: do we need a callback???
            //if (!string.IsNullOrEmpty(statusCallback)) request.AddParameter("StatusCallback", statusCallback);

            // todo: fix return when result object is defined
            //var response = client.Execute<SMSMessage>(request);
            //return response.Data;

            var response = client.Execute<Boolean>(request);
            return  true;
        }
        //#endregion
    }
}
