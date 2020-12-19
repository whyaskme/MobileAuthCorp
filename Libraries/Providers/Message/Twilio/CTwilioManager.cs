using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Validation;
using System.Xml.Linq;

namespace Twilio
{
    /*
        Possible POST or PUT Response Status Codes
            200 OK: The request was successful, we updated the resource and the response body contains the representation.
            201 CREATED: The request was successful, we created a new resource and the response body contains the representation.
            400 BAD REQUEST: The data given in the POST or PUT failed validation. Inspect the response body for details.
            401 UNAUTHORIZED: The supplied credentials, if any, are not sufficient to create or update the resource.
            404 NOT FOUND: You know this one.
            405 METHOD NOT ALLOWED: You can't POST or PUT to the resource.
            500 SERVER ERROR: We couldn't create or update the resource. Please try again.
    */
    public class CTwilioManager
    {
        #region Fields

        private RestClient client = null;

        #endregion

        #region Properties

        public string BaseUrl { get; private set; }
        public string APIVersion { get; private set; }
        public string AccountSid { get; private set; }
        public string AuthToken { get; private set; }
        public string From { get; private set; }

        #endregion

        #region Constructors

        public CTwilioManager()
        {
            BaseUrl = "https://api.twilio.com";
            APIVersion = "2010-04-01";
            AccountSid = "ACb095f77308ab428fb502973df7b0e085";
            AuthToken = "6660bc2741bd4e2be42960e14815bd8b";
            From = "(949) 891-0981";

            client = new RestClient();
            client.Authenticator = new HttpBasicAuthenticator(AccountSid, AuthToken);
            client.BaseUrl = string.Format("{0}/{1}", BaseUrl, APIVersion);

            // if acting on a subaccount, use request.AddUrlSegment("AccountSid", "value")
            // to override for that request.
            client.AddDefaultUrlSegment("AccountSid", AccountSid); 
        }

        #endregion

        #region ProviderSms

        /// <summary>
        /// Send a new ProviderSms message to the specified recipients.
        /// Makes a POST request to the SMSMessages List resource.
        /// </summary>
        /// <param name="from">The phone number to send the message from. Must be a Twilio-provided or ported local (not toll-free) number. Validated outgoing caller IDs cannot be used.</param>
        /// <param name="to">The phone number to send the message to. If using the Sandbox, this number must be a validated outgoing caller ID</param>
        /// <param name="body">The message to send. Must be 160 characters or less.</param>
        public SMSMessage SendSmsMessage(string to, string body)
        {
            return SendSmsMessage(to, body, string.Empty);
        }

        /// <summary>
        /// Send a new ProviderSms message to the specified recipients
        /// Makes a POST request to the SMSMessages List resource.
        /// </summary>
        /// <param name="from">The phone number to send the message from. Must be a Twilio-provided or ported local (not toll-free) number. Validated outgoing caller IDs cannot be used.</param>
        /// <param name="to">The phone number to send the message to. If using the Sandbox, this number must be a validated outgoing caller ID</param>
        /// <param name="body">The message to send. Must be 160 characters or less.</param>
        /// <param name="statusCallback">A Url that Twilio will POST to when your message is processed. Twilio will POST the SmsSid as well as SmsStatus=sent or SmsStatus=failed</param>
        public SMSMessage SendSmsMessage(string to, string body, string statusCallback)
        {
            Validate.IsValidLength(body, 160);
            Require.Argument("from", this.From);
            Require.Argument("to", to);
            Require.Argument("body", body);

            var request = new RestRequest(Method.POST);
            request.Resource = "Accounts/{AccountSid}/SMS/Messages";
            //request.RootElement = "TwilioResponse";
            request.AddParameter("From", this.From);
            request.AddParameter("To", to);
            request.AddParameter("Body", body);
            if (!string.IsNullOrEmpty(statusCallback)) request.AddParameter("StatusCallback", statusCallback);

            var response = client.Execute<SMSMessage>(request);
            return  response.Data;
        }

        #endregion

    }
}
